namespace EOM.TSHotelManagement.Contract
{
    public class BaseInputDto : BaseAuditDto
    {
        /// <summary>
        /// 删除标识
        /// </summary>
        public int? IsDelete { get; set; } = 0;
        /// <summary>
        /// 行版本（乐观锁）
        /// </summary>
        public long? RowVersion { get; set; }
    }
}
