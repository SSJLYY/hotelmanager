namespace EOM.TSHotelManagement.Contract
{
    public class BaseOutputDto : BaseAuditDto
    {
        public int? IsDelete { get; set; }
        /// <summary>
        /// 行版本（乐观锁）
        /// </summary>
        public long? RowVersion { get; set; }
    }
}
