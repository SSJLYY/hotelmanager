namespace EOM.TSHotelManagement.Contract
{
    public class BaseAuditDto : BaseDto
    {
        /// <summary>
        /// 资料创建人
        /// </summary>
        public string? DataInsUsr { get; set; }
        /// <summary>
        /// 资料创建时间
        /// </summary>
        public DateTime? DataInsDate { get; set; }
        /// <summary>
        /// 资料更新人
        /// </summary>
        public string? DataChgUsr { get; set; }
        /// <summary>
        /// 资料更新时间
        /// </summary>
        public DateTime? DataChgDate { get; set; }
    }
}
