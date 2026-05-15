namespace EOM.TSHotelManagement.Contract.SystemManagement.Dto.Permission
{
    /// <summary>
    /// 角色授权读取结果（菜单与权限独立）
    /// </summary>
    public class ReadRoleGrantOutputDto : BaseOutputDto
    {
        public string RoleNumber { get; set; } = string.Empty;

        public List<string> PermissionNumbers { get; set; } = new();

        public List<int> MenuIds { get; set; } = new();
    }
}
