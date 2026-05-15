using EOM.TSHotelManagement.Contract;
using System.Threading.Tasks;

namespace EOM.TSHotelManagement.Service
{
    public interface IQuartzAppService
    {
        Task<ListOutputDto<ReadQuartzJobOutputDto>> SelectQuartzJobList();
    }
}
