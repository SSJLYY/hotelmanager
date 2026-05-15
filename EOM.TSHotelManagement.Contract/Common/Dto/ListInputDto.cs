namespace EOM.TSHotelManagement.Contract
{
    public class ListInputDto : BaseInputDto
    {
        private int _page = 1;
        private int _pageSize = 15;

        /// <summary>
        /// 页数
        /// </summary>
        public int Page
        {
            get => _page;
            set => _page = value > 0 ? value : 1;
        }
        /// <summary>
        /// 总数
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > 0 ? value : 15;
        }
        /// <summary>
        /// 忽略分页
        /// </summary>
        public bool IgnorePaging { get; set; } = false;
    }
}
