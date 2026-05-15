namespace EOM.TSHotelManagement.Contract
{
    public class SingleOutputDto<T> : BaseResponse
    {
        /// <summary>
        /// 数据源
        /// </summary>
        public T Data { get; set; }
    }
}
