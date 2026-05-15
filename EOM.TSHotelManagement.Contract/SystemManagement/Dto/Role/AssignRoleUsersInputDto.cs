namespace EOM.TSHotelManagement.Contract.SystemManagement.Dto.Role
{
    /// <summary>
    /// 为角色分配管理员（全量覆盖式）
    /// </summary>
    public class AssignRoleUsersInputDto : BaseInputDto
    {
        /// <summary>
        /// 角色编码
        /// </summary>
        public string RoleNumber { get; set; } = null!;

        /// <summary>
        /// 管理员用户编码集合（Administrator.Number）
        /// </summary>
        public List<string> UserNumbers { get; set; } = new List<string>();
    }
}