namespace EOM.TSHotelManagement.Contract
{
    public class UpdateCustoTypeInputDto : BaseInputDto
    {
        public int CustomerType { get; set; }
        public string CustomerTypeName { get; set; }
        public decimal Discount { get; set; }
    }
}

