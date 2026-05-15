using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EOM.TSHotelManagement.Common
{
    /// <summary>
    /// 通用的根据 DTO 自动生成 SqlSugar `Expressionable<T>` 过滤表达式的构建器。
    /// 用法：`var where = SqlFilterBuilder.BuildExpression&lt;Entity, Dto&gt;(dto);`
    /// 支持：字符串精确匹配优先，失败降级到模糊匹配、数值/布尔/枚举精确匹配、日期/数值范围（属性名含 Start/End/From/To/Min/Max 的自动识别）。
    /// 对于包含 DateRangeDto 属性的 DTO，支持：
    /// - 指定单个日期字段范围查询：BuildExpression&lt;Entity, Dto&gt;(dto, "DateField")
    /// - 多个日期字段范围查询：通过 dto.DateRangeDto.Ranges 字典设置多个字段的范围
    /// - 自定义映射字典：BuildExpression&lt;Entity, Dto&gt;(dto, null, propertyMapping) 或 BuildExpression&lt;Entity, Dto&gt;(dto, dateFieldName, propertyMapping)
    /// </summary>
    public static class SqlFilterBuilder
    {
        private static readonly string[] RangeSuffixesStart = new[] { "Start", "From", "Min" };
        private static readonly string[] RangeSuffixesEnd = new[] { "End", "To", "Max" };

        private static readonly (string Start, string End)[] DateRangePairs = new[]
        {
            ("Start", "End"),
            ("StartDate", "EndDate"),
            ("BeginDate", "EndDate"),
            ("FromDate", "ToDate")
        };

        public static Expressionable<TEntity> BuildExpression<TEntity, TDto>(TDto dto, bool skipZeroForNumeric = false)
            where TEntity : class, new()
        {
            return BuildExpression<TEntity, TDto>(dto, null, null, skipZeroForNumeric);
        }

        public static Expressionable<TEntity> BuildExpression<TEntity, TDto>(TDto dto, string dateFieldName, bool skipZeroForNumeric = false)
            where TEntity : class, new()
        {
            return BuildExpression<TEntity, TDto>(dto, dateFieldName, null,skipZeroForNumeric);
        }

        public static Expressionable<TEntity> BuildExpression<TEntity, TDto>(TDto dto, Dictionary<string, string> propertyMapping, bool skipZeroForNumeric = false)
            where TEntity : class, new()
        {
            return BuildExpression<TEntity, TDto>(dto, null, propertyMapping, skipZeroForNumeric);
        }

        /// <summary>
        /// 构建查询表达式，支持自定义DTO字段和实体字段的映射
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TDto">DTO类型</typeparam>
        /// <param name="dto">查询DTO</param>
        /// <param name="dateFieldName">日期字段名称（可选）</param>
        /// <param name="propertyMapping">属性映射字典，key为DTO属性名，value为实体属性名（可选）</param>
        /// <param name="skipZeroForNumeric">是否跳过数值类型的零值（默认false，不跳过）</param>
        /// <returns>表达式构建器</returns>
        public static Expressionable<TEntity> BuildExpression<TEntity, TDto>(
            TDto dto,
            string dateFieldName,
            Dictionary<string, string> propertyMapping,
            bool skipZeroForNumeric = false)
            where TEntity : class, new()
        {
            var where = Expressionable.Create<TEntity>();
            if (dto == null) return where;

            var dtoType = typeof(TDto);
            var entityType = typeof(TEntity);
            var dtoProps = dtoType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // 获取映射字典，如果为null则使用空字典
            var mapping = propertyMapping ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Find DateRangeDto property in dto
            DateRangeDto dateRangeDto = null;
            var dateRangeProp = dtoProps.FirstOrDefault(p => p.PropertyType == typeof(DateRangeDto));
            if (dateRangeProp != null)
            {
                dateRangeDto = dateRangeProp.GetValue(dto) as DateRangeDto;
            }

            if (dateRangeDto == null)
            {
                var startDateProp = dtoProps.FirstOrDefault(p => p.Name.Contains("StartDate", StringComparison.OrdinalIgnoreCase));
                var endDateProp = dtoProps.FirstOrDefault(p => p.Name.Contains("EndDate", StringComparison.OrdinalIgnoreCase));
                if (startDateProp != null && endDateProp != null)
                {
                    var startValue = startDateProp.GetValue(dto);
                    var endValue = endDateProp.GetValue(dto);
                    if (startValue != null && endValue != null)
                    {
                        DateTime? startDt = null;
                        DateTime? endDt = null;
                        if (startValue is DateTime dtStart)
                        {
                            startDt = dtStart;
                        }
                        else if (startValue is DateOnly dStart)
                        {
                            startDt = dStart.ToDateTime(TimeOnly.MinValue);
                        }
                        if (endValue is DateTime dtEnd)
                        {
                            endDt = dtEnd;
                        }
                        else if (endValue is DateOnly dEnd)
                        {
                            endDt = dEnd.ToDateTime(TimeOnly.MinValue);
                        }
                        if (startDt.HasValue && endDt.HasValue)
                        {
                            dateRangeDto = new DateRangeDto
                            {
                                Start = startDt,
                                End = endDt
                            };
                        }
                    }
                }
            }

            // Handle date range if dateFieldName is specified and dateRangeDto exists
            if (!string.IsNullOrEmpty(dateFieldName) && dateRangeDto != null)
            {
                var targetProp = entityType.GetProperty(dateFieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (targetProp != null && dateRangeDto.Start.HasValue)
                {
                    if (TryBuildComparisonExpression<TEntity>(targetProp, dateRangeDto.Start.Value, ComparisonType.GreaterOrEqual, out var startExpr))
                    {
                        where = where.And(startExpr);
                    }
                }
                if (targetProp != null && dateRangeDto.End.HasValue)
                {
                    if (TryBuildComparisonExpression<TEntity>(targetProp, dateRangeDto.End.Value, ComparisonType.LessOrEqual, out var endExpr))
                    {
                        where = where.And(endExpr);
                    }
                }
            }

            // Handle date range for entity's date range pair fields if dateRangeDto exists and no specific dateFieldName
            if (dateRangeDto != null && string.IsNullOrEmpty(dateFieldName))
            {
                var (startProp, endProp) = FindDateRangePair(entityType);
                if (startProp != null && endProp != null)
                {
                    // 重叠条件：data.Start <= query.End AND data.End >= query.Start
                    if (dateRangeDto.End.HasValue)
                    {
                        if (TryBuildComparisonExpression<TEntity>(startProp, dateRangeDto.End.Value, ComparisonType.LessOrEqual, out var startExpr))
                        {
                            where = where.And(startExpr);
                        }
                    }
                    if (dateRangeDto.Start.HasValue)
                    {
                        if (TryBuildComparisonExpression<TEntity>(endProp, dateRangeDto.Start.Value, ComparisonType.GreaterOrEqual, out var endExpr))
                        {
                            where = where.And(endExpr);
                        }
                    }
                }
            }

            // 收集所有非字符串类型的条件
            var nonStringConditions = Expressionable.Create<TEntity>();

            foreach (var dprop in dtoProps)
            {
                var rawValue = dprop.GetValue(dto);
                if (rawValue == null) continue;

                // 移除了IsDefaultValue检查，因为它会导致数值0被跳过
                // 如果需要跳过零值，可以通过skipZeroForNumeric参数控制
                if (skipZeroForNumeric && IsNumericZeroValue(rawValue))
                {
                    continue;
                }

                var dname = dprop.Name;

                // Skip DateRangeDto property as it's already handled
                if (dprop.PropertyType == typeof(DateRangeDto)) continue;

                // set null if rawValue == 1900/01/01
                if (dprop.PropertyType == typeof(DateOnly) && (DateOnly)rawValue == DateOnly.MinValue) rawValue = null;

                // Always skip Start and End as they are handled separately for overlap
                if (dname == "Start" || dname == "End") continue;

                // 检查映射字典
                PropertyInfo entityProp = null;
                if (mapping.TryGetValue(dname, out var mappedPropName))
                {
                    // 如果存在映射，则使用映射后的属性名
                    entityProp = entityType.GetProperty(mappedPropName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                    // 注意：如果使用了映射字典，则不进行范围属性检测，直接进行精确匹配
                    if (entityProp != null)
                    {
                        // 非字符串类型 -> 精确匹配
                        if (entityProp.PropertyType != typeof(string))
                        {
                            if (rawValue == null) continue;
                            if (TryBuildEqualityExpression<TEntity>(entityProp, rawValue, out var eqExpr))
                            {
                                nonStringConditions = nonStringConditions.And(eqExpr);
                            }
                        }
                        // 字符串类型稍后处理
                        continue;
                    }
                }
                else
                {
                    // 如果没有映射，继续原有的处理逻辑

                    // Handle range properties: e.g. CreateDateStart/CreateDateEnd => CreateDate >= Start, <= End
                    var rangeBase = GetRangeBaseName(dname, out var rangeType);
                    if (rangeBase != null)
                    {
                        var targetProp = entityType.GetProperty(rangeBase, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        if (targetProp != null)
                        {
                            if (TryBuildComparisonExpression<TEntity>(targetProp, rawValue, rangeType == RangeType.Start ? ComparisonType.GreaterOrEqual : ComparisonType.LessOrEqual, out var expr))
                            {
                                where = where.And(expr);
                            }
                        }
                        continue;
                    }

                    // Normal property name match
                    entityProp = entityType.GetProperty(dname, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                }

                if (entityProp == null) continue;

                // 非字符串类型 -> 精确匹配
                if (entityProp.PropertyType != typeof(string))
                {
                    if (rawValue == null) continue;
                    if (TryBuildEqualityExpression<TEntity>(entityProp, rawValue, out var eqExpr))
                    {
                        nonStringConditions = nonStringConditions.And(eqExpr);
                    }
                    continue;
                }
            }

            // 先应用非字符串类型的条件
            if (nonStringConditions != null)
            {
                var nonStringExpr = nonStringConditions.ToExpression();
                if (nonStringExpr != null)
                {
                    where = where.And(nonStringExpr);
                }
            }

            // 处理字符串类型的条件，先尝试精确匹配
            var stringExactConditions = Expressionable.Create<TEntity>();
            var stringContainsConditions = Expressionable.Create<TEntity>();
            var hasStringProperties = false;

            foreach (var dprop in dtoProps)
            {
                var rawValue = dprop.GetValue(dto);
                if (rawValue == null) continue;

                // 移除了IsDefaultValue检查
                // 如果需要跳过空字符串，可以通过检查字符串是否为空来控制
                if (skipZeroForNumeric && IsNumericZeroValue(rawValue))
                {
                    continue;
                }

                var dname = dprop.Name;

                // Skip DateRangeDto property as it's already handled
                if (dprop.PropertyType == typeof(DateRangeDto)) continue;

                // Always skip Start and End as they are handled separately for overlap
                if (dname == "Start" || dname == "End") continue;

                PropertyInfo entityProp = null;
                if (mapping.TryGetValue(dname, out var mappedPropName))
                {
                    // 如果存在映射，则使用映射后的属性名
                    entityProp = entityType.GetProperty(mappedPropName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                }
                else
                {
                    // 检查是否是范围属性
                    var rangeBase = GetRangeBaseName(dname, out var rangeType);
                    if (rangeBase != null)
                    {
                        // 如果是范围属性，已经在上面处理过，跳过字符串处理
                        continue;
                    }

                    // 普通属性名匹配
                    entityProp = entityType.GetProperty(dname, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                }

                if (entityProp == null) continue;

                // 字符串类型 -> 先精确匹配
                if (entityProp.PropertyType == typeof(string))
                {
                    hasStringProperties = true;
                    var s = rawValue as string;
                    if (!string.IsNullOrEmpty(s))
                    {
                        // 精确匹配条件
                        var exactExpr = BuildStringExactExpression<TEntity>(entityProp.Name, s);
                        if (exactExpr != null)
                        {
                            stringExactConditions = stringExactConditions.And(exactExpr);
                        }

                        // 模糊匹配条件
                        var containsExpr = BuildStringContainsExpression<TEntity>(entityProp.Name, s);
                        if (containsExpr != null)
                        {
                            stringContainsConditions = stringContainsConditions.And(containsExpr);
                        }
                    }
                }
            }

            // 如果有字符串属性，添加字符串条件
            if (hasStringProperties)
            {
                // 创建一个表达式：先尝试精确匹配，如果精确匹配没有结果，则使用模糊匹配
                // 通过 OR 连接精确和模糊条件，但精确匹配优先级更高
                var exactExpr = stringExactConditions.ToExpression();
                var containsExpr = stringContainsConditions.ToExpression();

                if (exactExpr != null && containsExpr != null)
                {
                    var param = exactExpr.Parameters[0];
                    var combinedBody = Expression.OrElse(exactExpr.Body, containsExpr.Body);
                    var combinedStringExpr = Expression.Lambda<Func<TEntity, bool>>(combinedBody, param);
                    where = where.And(combinedStringExpr);
                }
                else if (exactExpr != null)
                {
                    where = where.And(exactExpr);
                }
                else if (containsExpr != null)
                {
                    where = where.And(containsExpr);
                }
            }

            return where;
        }

        private static bool IsNumericZeroValue(object value)
        {
            if (value == null) return false;

            var type = value.GetType();
            if (!type.IsValueType) return false;

            // 检查是否是数值类型
            if (type == typeof(int) && (int)value == 0) return true;
            if (type == typeof(long) && (long)value == 0) return true;
            if (type == typeof(short) && (short)value == 0) return true;
            if (type == typeof(byte) && (byte)value == 0) return true;
            if (type == typeof(decimal) && (decimal)value == 0) return true;
            if (type == typeof(float) && (float)value == 0) return true;
            if (type == typeof(double) && (double)value == 0) return true;
            if (type == typeof(uint) && (uint)value == 0) return true;
            if (type == typeof(ulong) && (ulong)value == 0) return true;
            if (type == typeof(ushort) && (ushort)value == 0) return true;
            if (type == typeof(sbyte) && (sbyte)value == 0) return true;

            // 检查可空数值类型
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                var underlyingValue = Convert.ChangeType(value, underlyingType);
                return IsNumericZeroValue(underlyingValue);
            }

            return false;
        }

        private static (PropertyInfo Start, PropertyInfo End) FindDateRangePair(Type entityType)
        {
            foreach (var (startName, endName) in DateRangePairs)
            {
                var startProp = entityType.GetProperty(startName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                var endProp = entityType.GetProperty(endName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (startProp != null && endProp != null)
                {
                    return (startProp, endProp);
                }
            }
            return (null, null);
        }

        private enum RangeType { Start, End }

        private static string GetRangeBaseName(string propName, out RangeType? rangeType)
        {
            rangeType = null;
            foreach (var s in RangeSuffixesStart)
            {
                if (propName.EndsWith(s, StringComparison.OrdinalIgnoreCase))
                {
                    rangeType = RangeType.Start;
                    return propName.Substring(0, propName.Length - s.Length);
                }
            }
            foreach (var s in RangeSuffixesEnd)
            {
                if (propName.EndsWith(s, StringComparison.OrdinalIgnoreCase))
                {
                    rangeType = RangeType.End;
                    return propName.Substring(0, propName.Length - s.Length);
                }
            }
            return null;
        }

        private enum ComparisonType { GreaterOrEqual, LessOrEqual }

        private static bool TryBuildComparisonExpression<TEntity>(PropertyInfo entityProp, object value, ComparisonType cmpType, out Expression<Func<TEntity, bool>> expr)
        {
            expr = null;
            var entityType = typeof(TEntity);
            var param = Expression.Parameter(entityType, "x");
            Expression member = null;
            try
            {
                member = Expression.PropertyOrField(param, entityProp.Name);
            }
            catch
            {
                return false;
            }

            // Convert constant to target type (handle nullable types)
            var targetType = Nullable.GetUnderlyingType(entityProp.PropertyType) ?? entityProp.PropertyType;
            object constVal;
            try
            {
                if (targetType == typeof(DateOnly))
                {
                    if (value is DateTime dt)
                    {
                        constVal = DateOnly.FromDateTime(dt);
                    }
                    else if (value is DateOnly d)
                    {
                        constVal = d;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    constVal = Convert.ChangeType(value, targetType);
                }
            }
            catch
            {
                // cannot convert
                return false;
            }

            var constant = Expression.Constant(constVal, targetType);

            // if member is nullable, access Value
            if (Nullable.GetUnderlyingType(entityProp.PropertyType) != null)
            {
                var hasValue = Expression.Property(member, "HasValue");
                var valueAccess = Expression.Property(member, "Value");

                BinaryExpression compare = cmpType == ComparisonType.GreaterOrEqual
                    ? Expression.GreaterThanOrEqual(valueAccess, constant)
                    : Expression.LessThanOrEqual(valueAccess, constant);
                var notHasValue = Expression.Not(hasValue);
                var orExpression = Expression.OrElse(notHasValue, compare);
                expr = Expression.Lambda<Func<TEntity, bool>>(orExpression, param);
                return true;
            }

            BinaryExpression binary = cmpType == ComparisonType.GreaterOrEqual
                ? Expression.GreaterThanOrEqual(member, constant)
                : Expression.LessThanOrEqual(member, constant);

            expr = Expression.Lambda<Func<TEntity, bool>>(binary, param);
            return true;
        }

        private static Expression<Func<TEntity, bool>> BuildStringExactExpression<TEntity>(string propName, string value)
        {
            var entityType = typeof(TEntity);
            var param = Expression.Parameter(entityType, "x");

            try
            {
                var member = Expression.PropertyOrField(param, propName);
                var notNull = Expression.NotEqual(member, Expression.Constant(null, typeof(string)));
                var equal = Expression.Equal(member, Expression.Constant(value));
                var body = Expression.AndAlso(notNull, equal);
                return Expression.Lambda<Func<TEntity, bool>>(body, param);
            }
            catch
            {
                return null;
            }
        }

        private static Expression<Func<TEntity, bool>> BuildStringContainsExpression<TEntity>(string propName, string value)
        {
            var entityType = typeof(TEntity);
            var param = Expression.Parameter(entityType, "x");

            try
            {
                var member = Expression.PropertyOrField(param, propName);
                var notNull = Expression.NotEqual(member, Expression.Constant(null, typeof(string)));
                var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var call = Expression.Call(member, method, Expression.Constant(value));
                var body = Expression.AndAlso(notNull, call);
                return Expression.Lambda<Func<TEntity, bool>>(body, param);
            }
            catch
            {
                return null;
            }
        }

        private static bool TryBuildEqualityExpression<TEntity>(PropertyInfo entityProp, object value, out Expression<Func<TEntity, bool>> expr)
        {
            expr = null;
            var entityType = typeof(TEntity);
            var param = Expression.Parameter(entityType, "x");
            Expression member = null;
            try
            {
                member = Expression.PropertyOrField(param, entityProp.Name);
            }
            catch
            {
                return false;
            }

            var targetType = Nullable.GetUnderlyingType(entityProp.PropertyType) ?? entityProp.PropertyType;
            object constVal;
            try
            {
                if (value.GetType() == targetType)
                {
                    constVal = value;
                }
                else if (targetType == typeof(DateOnly))
                {
                    if (value is DateTime dt)
                    {
                        constVal = DateOnly.FromDateTime(dt);
                    }
                    else if (value is DateOnly d)
                    {
                        constVal = d;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    constVal = Convert.ChangeType(value, targetType);
                }
            }
            catch
            {
                return false;
            }

            var constant = Expression.Constant(constVal, targetType);

            // If entity member is nullable, compare Value
            if (Nullable.GetUnderlyingType(entityProp.PropertyType) != null)
            {
                var hasValue = Expression.Property(member, "HasValue");
                var valueAccess = Expression.Property(member, "Value");
                var equal = Expression.Equal(valueAccess, constant);
                var body = Expression.AndAlso(hasValue, equal);
                expr = Expression.Lambda<Func<TEntity, bool>>(body, param);
                return true;
            }

            var eq = Expression.Equal(member, constant);
            expr = Expression.Lambda<Func<TEntity, bool>>(eq, param);
            return true;
        }
    }
}