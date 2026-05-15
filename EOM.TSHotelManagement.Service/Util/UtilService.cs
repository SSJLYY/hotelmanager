using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Data;
using EOM.TSHotelManagement.Domain;
using jvncorelib.EntityLib;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 工具接口实现类
    /// </summary>
    public class UtilService : IUtilService
    {
        /// <summary>
        /// 卡片代码
        /// </summary>
        private readonly GenericRepository<CardCode> cardCodesRepository;

        /// <summary>
        /// 操作日志
        /// </summary>
        private readonly GenericRepository<OperationLog> operationLogRepository;

        /// <summary>
        /// 请求日志
        /// </summary>
        private readonly GenericRepository<RequestLog> requestLogRepository;

        private readonly ILogger<UtilService> logger;

        public UtilService(GenericRepository<CardCode> cardCodesRepository, GenericRepository<OperationLog> operationLogRepository, GenericRepository<RequestLog> requestLogRepository, ILogger<UtilService> logger)
        {
            this.cardCodesRepository = cardCodesRepository;
            this.operationLogRepository = operationLogRepository;
            this.requestLogRepository = requestLogRepository;
            this.logger = logger;
        }

        /// <summary>
        /// 查询身份证号码
        /// </summary>
        /// <param name="readCardCodeInputDto"></param>
        /// <returns></returns>
        public SingleOutputDto<ReadCardCodeOutputDto> SelectCardCode(ReadCardCodeInputDto readCardCodeInputDto)
        {
            if (!readCardCodeInputDto.IdentityCardNumber.IsNullOrEmpty())
            {
                var cardid = readCardCodeInputDto.IdentityCardNumber.Substring(0, 6).ToString();
                var cardcodes = cardCodesRepository.GetFirst(a => a.AreaCode == cardid);
                var source = EntityMapper.Map<CardCode, ReadCardCodeOutputDto>(cardcodes);
                return new SingleOutputDto<ReadCardCodeOutputDto>() { Data = source };
            }
            return new SingleOutputDto<ReadCardCodeOutputDto>();
        }

        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="opr"></param>
        /// <returns></returns>
        public BaseResponse AddLog(CreateOperationLogInputDto opr)
        {
            try
            {
                var log = EntityMapper.Map<CreateOperationLogInputDto, OperationLog>(opr);
                var result = operationLogRepository.Insert(log);
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "添加操作日志失败");
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString("Add Operation Log Failure", "添加操作日志失败"), Code = BusinessStatusCode.InternalServerError };
            }
        }

        /// <summary>
        /// 查询所有操作日志
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<ReadOperationLogOutputDto> SelectOperationlogAll(ReadOperationLogInputDto readOperationLogInputDto)
        {
            List<OperationLog> operationLogs = new List<OperationLog>();

            var where = SqlFilterBuilder.BuildExpression<OperationLog, ReadOperationLogInputDto>(readOperationLogInputDto, nameof(OperationLog.OperationTime));

            var count = 0;
            if (!readOperationLogInputDto.IgnorePaging)
            {
                operationLogs = operationLogRepository.AsQueryable().Where(where.ToExpression()).OrderByDescending(a => a.OperationTime).ToPageList(readOperationLogInputDto.Page, readOperationLogInputDto.PageSize, ref count);
            }
            else
            {
                operationLogs = operationLogRepository.AsQueryable().Where(where.ToExpression()).OrderByDescending(a => a.OperationTime).ToList();
                count = operationLogs.Count;
            }

            operationLogs.ForEach(source =>
            {
                source.LogLevelName = source.LogLevel == (int)Common.LogLevel.Normal ? LocalizationHelper.GetLocalizedString("INFO", "常规操作") : source.LogLevel == (int)Common.LogLevel.Warning ? LocalizationHelper.GetLocalizedString("WARNING", "敏感操作") : LocalizationHelper.GetLocalizedString("ERROR", "严重操作");
            });

            var result = EntityMapper.MapList<OperationLog, ReadOperationLogOutputDto>(operationLogs);

            return new ListOutputDto<ReadOperationLogOutputDto>
            {
                Data = new PagedData<ReadOperationLogOutputDto>
                {
                    Items = result,
                    TotalCount = count
                }
            };
        }

        /// <summary>
        /// 查询所有请求日志
        /// </summary>
        /// <param name="readRequestLogInputDto"></param>
        /// <returns></returns>
        public ListOutputDto<ReadRequestLogOutputDto> SelectRequestlogAll(ReadRequestLogInputDto readRequestLogInputDto)
        {
            List<RequestLog> requestLogs = new List<RequestLog>();

            var where = SqlFilterBuilder.BuildExpression<RequestLog, ReadRequestLogInputDto>(readRequestLogInputDto, nameof(RequestLog.RequestTime));

            var count = 0;
            if (!readRequestLogInputDto.IgnorePaging)
            {
                requestLogs = requestLogRepository.AsQueryable().Where(where.ToExpression()).OrderByDescending(a => a.RequestTime).ToPageList(readRequestLogInputDto.Page, readRequestLogInputDto.PageSize, ref count);
            }
            else
            {
                requestLogs = requestLogRepository.AsQueryable().Where(where.ToExpression()).OrderByDescending(a => a.RequestTime).ToList();
                count = requestLogs.Count;
            }

            var result = EntityMapper.MapList<RequestLog, ReadRequestLogOutputDto>(requestLogs);

            return new ListOutputDto<ReadRequestLogOutputDto>
            {
                Data = new PagedData<ReadRequestLogOutputDto>
                {
                    Items = result,
                    TotalCount = count
                }
            };
        }

        /// <summary>
        /// 删除指定时间范围的请求日志
        /// </summary>
        /// <returns></returns>
        public BaseResponse DeleteRequestlogByRange(ReadRequestLogInputDto readRequestLogInputDto)
        {
            try
            {
                var where = SqlFilterBuilder.BuildExpression<RequestLog, ReadRequestLogInputDto>(readRequestLogInputDto ?? new ReadRequestLogInputDto());
                var logsToDelete = requestLogRepository.AsQueryable().Where(where.ToExpression()).ToList();
                if (logsToDelete.Any())
                {
                    requestLogRepository.Delete(logsToDelete);
                }
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
        }

        /// <summary>
        /// 删除指定时间范围的操作日志
        /// </summary>
        /// <returns></returns>
        public BaseResponse DeleteOperationlogByRange(ReadOperationLogInputDto readOperationLogInputDto)
        {
            try
            {
                var where = SqlFilterBuilder.BuildExpression<OperationLog, ReadOperationLogInputDto>(readOperationLogInputDto ?? new ReadOperationLogInputDto());

                var logsToDelete = operationLogRepository.AsQueryable().Where(where.ToExpression()).ToList();

                if (logsToDelete.Any())
                {
                    operationLogRepository.Delete(logsToDelete);
                }
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
        }

        /// <summary>
        /// 删除操作日志
        /// </summary>
        /// <returns></returns>
        public BaseResponse DeleteOperationlog(DeleteOperationLogInputDto deleteOperationLogInputDto)
        {
            try
            {
                if (deleteOperationLogInputDto?.DelIds == null || !deleteOperationLogInputDto.DelIds.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("Parameters Invalid", "参数错误")
                    };
                }

                var delIds = DeleteConcurrencyHelper.GetDeleteIds(deleteOperationLogInputDto);
                var operationLogs = operationLogRepository.GetList(a => delIds.Contains(a.Id));

                if (!operationLogs.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("Operation Log Not Found", "操作日志未找到")
                    };
                }

                if (DeleteConcurrencyHelper.HasDeleteConflict(deleteOperationLogInputDto, operationLogs, a => a.Id, a => a.RowVersion))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                operationLogRepository.Delete(operationLogs);
                return new BaseResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting room type");
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString("Delete Operation Log Failure", "删除操作日志失败"), Code = BusinessStatusCode.InternalServerError };
            }
        }
    }
}
