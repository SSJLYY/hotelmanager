using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Data;
using EOM.TSHotelManagement.Domain;
using jvncorelib.EntityLib;
using SqlSugar;

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 权限服务实现
    /// </summary>
    public class PermissionAppService : IPermissionAppService
    {
        private readonly GenericRepository<Permission> permissionRepository;

        public PermissionAppService(GenericRepository<Permission> permissionRepository)
        {
            this.permissionRepository = permissionRepository;
        }

        /// <summary>
        /// 查询权限列表（支持条件过滤与分页/忽略分页）
        /// </summary>
        public ListOutputDto<ReadPermissionOutputDto> SelectPermissionList(ReadPermissionInputDto input)
        {
            try
            {
                input ??= new ReadPermissionInputDto();
                var where = SqlFilterBuilder.BuildExpression<Permission, ReadPermissionInputDto>(input);

                var list = new List<Permission>();
                var count = 0;

                if (!input.IgnorePaging)
                {
                    list = permissionRepository.AsQueryable()
                        .Where(where.ToExpression())
                        .ToPageList(input.Page, input.PageSize, ref count);
                }
                else
                {
                    list = permissionRepository.AsQueryable()
                        .Where(where.ToExpression())
                        .ToList();
                    count = list.Count;
                }

                var outputItems = list.Select(p => new ReadPermissionOutputDto
                {
                    Id = p.Id,
                    PermissionNumber = p.PermissionNumber,
                    PermissionName = p.PermissionName,
                    Module = p.Module,
                    MenuKey = p.MenuKey,
                    Description = p.Description,
                    RowVersion = p.RowVersion
                }).ToList();

                return new ListOutputDto<ReadPermissionOutputDto>
                {
                    Message = LocalizationHelper.GetLocalizedString("Query success", "查询成功"),
                    Data = new PagedData<ReadPermissionOutputDto>
                    {
                        Items = outputItems,
                        TotalCount = count
                    }
                };
            }
            catch (Exception ex)
            {
                return new ListOutputDto<ReadPermissionOutputDto>
                {
                    Code = BusinessStatusCode.InternalServerError,
                    Message = LocalizationHelper.GetLocalizedString($"Query failed: {ex.Message}", $"查询失败：{ex.Message}"),
                    Data = new PagedData<ReadPermissionOutputDto>
                    {
                        Items = new List<ReadPermissionOutputDto>(),
                        TotalCount = 0
                    }
                };
            }
        }
    }
}
