namespace EOM.TSHotelManagement.Contract.Menu
{
    /// <summary>
    /// 菜单视图模型 (Menu View Model)
    /// </summary>
    public class MenuViewModel
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
        /// 子菜单 (Child Menus)
        /// </summary>
        public List<MenuViewModel> Children { get; set; } = new List<MenuViewModel>();
    }
}
