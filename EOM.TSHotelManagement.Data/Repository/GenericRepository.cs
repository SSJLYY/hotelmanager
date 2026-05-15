using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System.Linq.Expressions;

namespace EOM.TSHotelManagement.Data
{
    public class GenericRepository<T> : SimpleClient<T> where T : class, new()
    {
        /// <summary>
        /// HTTP上下文访问器
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly JWTHelper _jWTHelper;

        private readonly ILogger<GenericRepository<T>> _log;

        public GenericRepository(ISqlSugarClient client, IHttpContextAccessor httpContextAccessor, JWTHelper jWTHelper, ILogger<GenericRepository<T>> log) : base(client)
        {
            base.Context = client;
            _httpContextAccessor = httpContextAccessor;
            _jWTHelper = jWTHelper;
            _log = log;
        }

        private string GetCurrentUser()
        {
            try
            {
                var authHeader = _httpContextAccessor?.HttpContext?.Request?.Headers["Authorization"];
                if (string.IsNullOrEmpty(authHeader)) return "System";

                var token = authHeader.ToString().Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);

                return _jWTHelper.GetSerialNumber(token);
            }
            catch
            {
                return "System";
            }
        }

        public override bool Insert(T entity)
        {
            if (entity is AuditEntity baseEntity)
            {
                var currentUser = GetCurrentUser();
                if (!baseEntity.DataInsDate.HasValue)
                    baseEntity.DataInsDate = DateTime.Now;
                if (string.IsNullOrEmpty(baseEntity.DataInsUsr))
                    baseEntity.DataInsUsr = currentUser;
                if (baseEntity.RowVersion <= 0)
                    baseEntity.RowVersion = 1;
            }
            return base.Insert(entity);
        }

        public override bool Update(T entity)
        {
            Expression<Func<T, bool>>? rowVersionWhere = null;

            if (entity is AuditEntity baseEntity)
            {
                var currentUser = GetCurrentUser();
                if (!baseEntity.DataChgDate.HasValue)
                    baseEntity.DataChgDate = DateTime.Now;
                if (string.IsNullOrEmpty(baseEntity.DataChgUsr))
                    baseEntity.DataChgUsr = currentUser;

                // 更新接口必须携带行版本，缺失时视为并发校验失败。
                if (baseEntity.RowVersion <= 0)
                    return false;

                var currentRowVersion = baseEntity.RowVersion;
                rowVersionWhere = BuildEqualsLambda(nameof(BaseEntity.RowVersion), currentRowVersion);
                baseEntity.RowVersion = currentRowVersion + 1;
            }

            var primaryKeys = base.Context.EntityMaintenance.GetEntityInfo<T>().Columns
                .Where(it => it.IsPrimarykey)
                .Select(it => it.PropertyName)
                .ToList();

            var primaryKeyWhere = BuildUpdateWhereExpression(entity, primaryKeys);
            if (primaryKeyWhere == null)
            {
                _log.LogWarning("Unable to build primary-key WHERE for entity type {EntityType}. Update aborted to avoid accidental mass update.", typeof(T).Name);
                return false;
            }

            var finalWhere = rowVersionWhere == null
                ? primaryKeyWhere
                : AndAlso(primaryKeyWhere, rowVersionWhere);

            return base.Context.Updateable(entity)
                .IgnoreColumns(true, false)
                .Where(finalWhere)
                .ExecuteCommand() > 0;
        }

        public override bool UpdateRange(List<T> updateObjs)
        {
            if (updateObjs == null || updateObjs.Count == 0)
            {
                return false;
            }

            foreach (var entity in updateObjs)
            {
                if (entity is AuditEntity baseEntity)
                {
                    var currentUser = GetCurrentUser();
                    if (!baseEntity.DataChgDate.HasValue)
                        baseEntity.DataChgDate = DateTime.Now;
                    if (string.IsNullOrEmpty(baseEntity.DataChgUsr))
                        baseEntity.DataChgUsr = currentUser;
                }
            }

            // For BaseEntity types, route through single-entity Update in a transaction
            // so optimistic-lock checks are consistently enforced.
            if (typeof(BaseEntity).IsAssignableFrom(typeof(T)))
            {
                var tranResult = base.Context.Ado.UseTran(() =>
                {
                    foreach (var entity in updateObjs)
                    {
                        if (!Update(entity))
                        {
                            _log.LogWarning("Optimistic concurrency check failed for entity of type {EntityType}. Update aborted.", typeof(T).Name);
                            throw new InvalidOperationException("Optimistic concurrency check failed.");
                        }
                    }
                });

                return tranResult.IsSuccess;
            }

            return base.Context.Updateable(updateObjs)
                .IgnoreColumns(ignoreAllNullColumns: true)
                .ExecuteCommand() > 0;
        }

        public bool SoftDelete(T entity)
        {
            if (entity is SoftDeleteEntity baseEntity)
            {
                var currentUser = GetCurrentUser();
                if (!baseEntity.DataChgDate.HasValue)
                    baseEntity.DataChgDate = DateTime.Now;
                if (string.IsNullOrEmpty(baseEntity.DataChgUsr))
                    baseEntity.DataChgUsr = currentUser;
            }

            var primaryKeys = base.Context.EntityMaintenance.GetEntityInfo<T>().Columns
                .Where(it => it.IsPrimarykey)
                .Select(it => it.PropertyName)
                .ToList();

            if (primaryKeys.Count <= 1)
            {
                return base.Context.Updateable(entity)
                    .IgnoreColumns(ignoreAllNullColumns: true, false, true)
                    .ExecuteCommand() > 0;
            }

            var idProperty = entity.GetType().GetProperty("Id");
            if (idProperty != null)
            {
                var idValue = Convert.ToInt64(idProperty.GetValue(entity));

                if (idValue == 0)
                {
                    var otherPrimaryKeys = primaryKeys.Where(pk => pk != "Id").ToList();

                    var parameter = Expression.Parameter(typeof(T), "it");
                    Expression whereExpression = null;

                    foreach (var key in otherPrimaryKeys)
                    {
                        var property = Expression.Property(parameter, key);
                        var value = entity.GetType().GetProperty(key).GetValue(entity);
                        var constant = Expression.Constant(value);
                        var equal = Expression.Equal(property, constant);

                        whereExpression = whereExpression == null
                            ? equal
                            : Expression.AndAlso(whereExpression, equal);
                    }

                    if (whereExpression != null)
                    {
                        var lambda = Expression.Lambda<Func<T, bool>>(whereExpression, parameter);

                        return base.Context.Updateable(entity)
                            .Where(lambda)
                            .IgnoreColumns(ignoreAllNullColumns: true)
                            .ExecuteCommand() > 0;
                    }
                }
            }

            return base.Context.Updateable(entity)
                .IgnoreColumns(ignoreAllNullColumns: true)
                .ExecuteCommand() > 0;
        }

        public bool SoftDeleteRange(List<T> entities)
        {
            if (entities == null || !entities.Any())
                return false;

            var currentUser = GetCurrentUser();
            var now = DateTime.Now;
            var hasBaseEntity = false;

            // 更新内存中的实体状态
            foreach (var entity in entities)
            {
                if (entity is SoftDeleteEntity baseEntity)
                {
                    hasBaseEntity = true;
                    baseEntity.IsDelete = 1;
                    baseEntity.DataChgDate = now;
                    baseEntity.DataChgUsr = currentUser;
                }
            }

            if (!hasBaseEntity)
                return false;

            // 分批次处理
            const int batchSize = 1000;
            var totalAffected = 0;

            for (int i = 0; i < entities.Count; i += batchSize)
            {
                var batch = entities.Skip(i).Take(batchSize).ToList();

                totalAffected += base.Context.Updateable(batch)
                    .UpdateColumns(new[] {
                nameof(SoftDeleteEntity.IsDelete),
                nameof(AuditEntity.DataChgUsr),
                nameof(AuditEntity.DataChgDate)
                    })
                    .ExecuteCommand();
            }

            return totalAffected > 0;
        }

        private static Expression<Func<T, bool>> BuildEqualsLambda(string propertyName, object propertyValue)
        {
            var parameter = Expression.Parameter(typeof(T), "it");
            var property = Expression.Property(parameter, propertyName);

            object? normalizedValue = propertyValue;
            var targetType = Nullable.GetUnderlyingType(property.Type) ?? property.Type;
            if (normalizedValue != null && normalizedValue.GetType() != targetType)
            {
                normalizedValue = Convert.ChangeType(normalizedValue, targetType);
            }

            var constant = Expression.Constant(normalizedValue, property.Type);
            var equal = Expression.Equal(property, constant);
            return Expression.Lambda<Func<T, bool>>(equal, parameter);
        }

        private static Expression<Func<T, bool>>? BuildPrimaryKeyWhereExpression(T entity, List<string> primaryKeys)
        {
            if (entity == null || primaryKeys == null || primaryKeys.Count == 0)
            {
                return null;
            }

            var parameter = Expression.Parameter(typeof(T), "it");
            Expression? whereExpression = null;

            foreach (var key in primaryKeys)
            {
                var value = entity.GetType().GetProperty(key)?.GetValue(entity);
                if (value == null)
                {
                    continue;
                }

                var property = Expression.Property(parameter, key);
                object normalizedValue = value;
                var targetType = Nullable.GetUnderlyingType(property.Type) ?? property.Type;
                if (normalizedValue.GetType() != targetType)
                {
                    normalizedValue = Convert.ChangeType(normalizedValue, targetType);
                }

                var constant = Expression.Constant(normalizedValue, property.Type);
                var equal = Expression.Equal(property, constant);

                whereExpression = whereExpression == null
                    ? equal
                    : Expression.AndAlso(whereExpression, equal);
            }

            return whereExpression == null
                ? null
                : Expression.Lambda<Func<T, bool>>(whereExpression, parameter);
        }

        private static Expression<Func<T, bool>>? BuildUpdateWhereExpression(T entity, List<string> primaryKeys)
        {
            if (entity == null || primaryKeys == null || primaryKeys.Count == 0)
            {
                return null;
            }

            // Prefer identity-style Id when provided.
            var idProperty = entity.GetType().GetProperty("Id");
            if (idProperty != null)
            {
                var idRawValue = idProperty.GetValue(entity);
                if (idRawValue != null)
                {
                    var idValue = Convert.ToInt64(idRawValue);
                    if (idValue > 0)
                    {
                        return BuildEqualsLambda("Id", idValue);
                    }
                }
            }

            // Fallback to non-Id primary keys when Id is absent/invalid.
            var nonIdPrimaryKeys = primaryKeys.Where(pk => !pk.Equals("Id", StringComparison.OrdinalIgnoreCase)).ToList();
            var fallbackWhere = BuildPrimaryKeyWhereExpression(entity, nonIdPrimaryKeys);
            if (fallbackWhere != null)
            {
                return fallbackWhere;
            }

            // Last chance: use all primary keys if available.
            return BuildPrimaryKeyWhereExpression(entity, primaryKeys);
        }

        private static Expression<Func<T, bool>> AndAlso(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(T), "it");
            var leftBody = new ReplaceParameterVisitor(left.Parameters[0], parameter).Visit(left.Body);
            var rightBody = new ReplaceParameterVisitor(right.Parameters[0], parameter).Visit(right.Body);
            var andBody = Expression.AndAlso(leftBody!, rightBody!);
            return Expression.Lambda<Func<T, bool>>(andBody, parameter);
        }

        private sealed class ReplaceParameterVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParameter;
            private readonly ParameterExpression _newParameter;

            public ReplaceParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _oldParameter ? _newParameter : base.VisitParameter(node);
            }
        }
    }
}
