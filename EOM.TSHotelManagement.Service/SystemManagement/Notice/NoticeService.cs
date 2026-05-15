/*
 * MIT License
 *Copyright (c) 2021 易开元(Easy-Open-Meta)

 *Permission is hereby granted, free of charge, to any person obtaining a copy
 *of this software and associated documentation files (the "Software"), to deal
 *in the Software without restriction, including without limitation the rights
 *to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *copies of the Software, and to permit persons to whom the Software is
 *furnished to do so, subject to the following conditions:

 *The above copyright notice and this permission notice shall be included in all
 *copies or substantial portions of the Software.

 *THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *SOFTWARE.
 *
 */
using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Data;
using EOM.TSHotelManagement.Domain;
using Microsoft.Extensions.Logging;

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 公告信息接口实现类
    /// </summary>
    public class NoticeService : INoticeService
    {
        /// <summary>
        /// 公告
        /// </summary>
        private readonly GenericRepository<AppointmentNotice> noticeRepository;

        private readonly ILogger<NoticeService> logger;

        public NoticeService(GenericRepository<AppointmentNotice> noticeRepository, ILogger<NoticeService> logger)
        {
            this.noticeRepository = noticeRepository;
            this.logger = logger;
        }

        /// <summary>
        /// 获取所有公告信息
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<ReadAppointmentNoticeOutputDto> SelectNoticeAll(ReadAppointmentNoticeInputDto readAppointmentNoticeInputDto)
        {
            var ntc = new List<AppointmentNotice>();
            var where = SqlFilterBuilder.BuildExpression<AppointmentNotice, ReadAppointmentNoticeInputDto>(readAppointmentNoticeInputDto);
            var count = 0;

            if (!readAppointmentNoticeInputDto.IgnorePaging)
            {
                ntc = noticeRepository.AsQueryable().Where(where.ToExpression()).ToPageList(readAppointmentNoticeInputDto.Page, readAppointmentNoticeInputDto.PageSize, ref count);
            }
            else
            {
                ntc = noticeRepository.AsQueryable().Where(where.ToExpression()).ToList();
                count = ntc.Count;
            }
            
            ntc.ForEach(source =>
            {
                switch (source.NoticeType)
                {
                    case "PersonnelChanges":
                        source.NoticeTypeDescription = "人事变动";
                        break;
                    case "GeneralNotice":
                        source.NoticeTypeDescription = "普通公告";
                        break;
                }
            });
            var mapped = EntityMapper.MapList<AppointmentNotice, ReadAppointmentNoticeOutputDto>(ntc);
            return new ListOutputDto<ReadAppointmentNoticeOutputDto>
            {
                Data = new PagedData<ReadAppointmentNoticeOutputDto>
                {
                    Items = mapped,
                    TotalCount = count
                }
            };
        }

        /// <summary>
        /// 根据公告编号查找公告信息
        /// </summary>
        /// <param name="readAppointmentNoticeInputDto"></param>
        /// <returns></returns>
        public ReadAppointmentNoticeOutputDto SelectNoticeByNoticeNo(ReadAppointmentNoticeInputDto readAppointmentNoticeInputDto)
        {
            AppointmentNotice notice = new AppointmentNotice();
            notice = noticeRepository.GetFirst(a => a.NoticeNumber == readAppointmentNoticeInputDto.NoticeId);
            switch (notice.NoticeType)
            {
                case "PersonnelChanges":
                    notice.NoticeTypeDescription = "人事变动";
                    break;
                case "GeneralNotice":
                    notice.NoticeTypeDescription = "普通公告";
                    break;
            }
            var source = EntityMapper.Map<AppointmentNotice, ReadAppointmentNoticeOutputDto>(notice);

            return source;
        }

        /// <summary>
        /// 上传公告信息
        /// </summary>
        /// <param name="notice"></param>
        /// <returns></returns>
        public BaseResponse InsertNotice(CreateAppointmentNoticeInputDto notice)
        {
            try
            {
                var entity = EntityMapper.Map<CreateAppointmentNoticeInputDto, AppointmentNotice>(notice);
                var result = noticeRepository.Insert(entity);
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, LocalizationHelper.GetLocalizedString("insert appointment notice failed", "公告添加失败"));
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
        }

        /// <summary>
        /// 删除公告信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public BaseResponse DeleteNotice(DeleteAppointmentNoticeInputDto input)
        {
            try
            {
                if (input?.DelIds == null || !input.DelIds.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("Parameters Invalid", "参数错误")
                    };
                }

                var delIds = DeleteConcurrencyHelper.GetDeleteIds(input);
                var appointmentNotices = noticeRepository.GetList(a => delIds.Contains(a.Id));

                if (!appointmentNotices.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("Appointment Notice Not Found", "公告未找到")
                    };
                }

                if (DeleteConcurrencyHelper.HasDeleteConflict(input, appointmentNotices, a => a.Id, a => a.RowVersion))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                var result = noticeRepository.SoftDeleteRange(appointmentNotices);

                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, LocalizationHelper.GetLocalizedString("delete appointment notice failed", "删除公告失败"));
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString("delete appointment notice failed", "删除公告失败"), Code = BusinessStatusCode.InternalServerError };
            }
        }

        /// <summary>
        /// 更新公告信息
        /// </summary>
        /// <param name="updateAppointmentNoticeInputDto"></param>
        /// <returns></returns>
        public BaseResponse UpdateNotice(UpdateAppointmentNoticeInputDto updateAppointmentNoticeInputDto)
        {
            try
            {
                var entity = EntityMapper.Map<UpdateAppointmentNoticeInputDto, AppointmentNotice>(updateAppointmentNoticeInputDto);
                var result = noticeRepository.Update(entity);
                if (!result)
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, LocalizationHelper.GetLocalizedString("update appointment notice failed", "更新公告失败"));
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString("update appointment notice failed", "更新公告失败"), Code = BusinessStatusCode.InternalServerError };
            }
        }

    }
}
