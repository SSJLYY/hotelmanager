namespace EOM.TSHotelManagement.Contract
{
    public class ReadAdministratorTypeInputDto : ListInputDto
    {
        public int? Id { get; set; }
        public string TypeName { get; set; }
    }
}
