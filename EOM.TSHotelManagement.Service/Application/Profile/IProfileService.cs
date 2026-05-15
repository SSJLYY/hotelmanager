using EOM.TSHotelManagement.Contract;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace EOM.TSHotelManagement.Service
{
    public interface IProfileService
    {
        SingleOutputDto<CurrentProfileOutputDto> GetCurrentProfile(string serialNumber);

        Task<SingleOutputDto<UploadAvatarOutputDto>> UploadAvatar(string serialNumber, UploadAvatarInputDto inputDto, IFormFile file);

        BaseResponse ChangePassword(string serialNumber, ChangePasswordInputDto inputDto);
    }
}
