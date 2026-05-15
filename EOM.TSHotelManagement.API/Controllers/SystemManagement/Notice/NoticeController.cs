using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Service;
using Microsoft.AspNetCore.Mvc;

namespace EOM.TSHotelManagement.WebApi.Controllers
{
    /// <summary>
    /// 公告控制器
    /// </summary>
    public class NoticeController : ControllerBase
    {
        private readonly INoticeService noticeService;

        public NoticeController(INoticeService noticeService)
        {
            this.noticeService = noticeService;
        }

        #region 获取所有公告信息
        /// <summary>
        /// 获取所有公告信息
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [HttpGet]
        public ListOutputDto<ReadAppointmentNoticeOutputDto> SelectNoticeAll([FromQuery] ReadAppointmentNoticeInputDto inputDto)
        {
            return noticeService.SelectNoticeAll(inputDto);
        }
        #endregion

        /// <summary>
        /// 查询公告
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [HttpGet]
        public ReadAppointmentNoticeOutputDto SelectNoticeByNoticeNo([FromQuery] ReadAppointmentNoticeInputDto inputDto)
        {
            return noticeService.SelectNoticeByNoticeNo(inputDto);
        }

        #region 上传公告信息
        /// <summary>
        /// 上传公告信息
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse InsertNotice([FromBody] CreateAppointmentNoticeInputDto inputDto)
        {
            return noticeService.InsertNotice(inputDto);
        }
        #endregion
    }
}