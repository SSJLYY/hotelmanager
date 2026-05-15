namespace EOM.TSHotelManagement.Contract
{
    /// <summary>
    /// 菜单
    /// </summary>
    public class MenuDto
    {
        /// <summary>
        /// 菜单主键 (Menu Key)
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 菜单标题 (Menu Title)
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 菜单路径 (Menu Path)
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 该菜单下允许的权限编码集合（如：department.create / department.update 等）
        /// </summary>
        public List<string> Permissions { get; set; } = new List<string>();

        /// <summary>
        /// 子菜单 (Child Menus)
        /// </summary>
        public List<MenuDto> Children { get; set; } = new List<MenuDto>();
    }
}
