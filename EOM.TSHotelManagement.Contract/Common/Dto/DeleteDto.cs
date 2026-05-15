using System.Collections.Generic;

namespace EOM.TSHotelManagement.Contract
{
    public abstract class DeleteDto
    {
        public List<DeleteItemDto> DelIds { get; set; }
    }

    public class DeleteItemDto
    {
        public int Id { get; set; }
        public int RowVersion { get; set; }
    }
}
