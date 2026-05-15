namespace EOM.TSHotelManagement.Contract
{
    public class ReadCustoTypeOutputDto : BaseOutputDto
    {
        public int? Id { get; set; }
        public int CustomerType { get; set; }
        public string CustomerTypeName { get; set; }
        public decimal Discount { get; set; }
    }
}
