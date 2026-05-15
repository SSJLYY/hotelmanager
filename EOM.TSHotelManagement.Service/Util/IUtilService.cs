using EOM.TSHotelManagement.Contract;

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 工具类接口
    /// </summary>
    public interface IUtilService
    {
        /// <summary>
        /// 查询身份证号码
        /// </summary>
        /// <param name="readCardCodeInputDto"></param>
        /// <returns></returns>
        SingleOutputDto<ReadCardCodeOutputDto> SelectCardCode(ReadCardCodeInputDto readCardCodeInputDto);

        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="opr"></param>
        /// <returns></returns>
        BaseResponse AddLog(CreateOperationLogInputDto opr);

        /// <summary>
        /// 查询所有操作日志
        /// </summary>
        /// <returns></returns>
        ListOutputDto<ReadOperationLogOutputDto> SelectOperationlogAll(ReadOperationLogInputDto readOperationLogInputDto);

        /// <summary>
        /// 查询所有请求日志
        /// </summary>
        /// <param name="readRequestLogInputDto"></param>
        /// <returns></returns>
        ListOutputDto<ReadRequestLogOutputDto> SelectRequestlogAll(ReadRequestLogInputDto readRequestLogInputDto);

        /// <summary>
        /// 删除指定时间范围的请求日志
        /// </summary>
        /// <returns></returns>
        BaseResponse DeleteRequestlogByRange(ReadRequestLogInputDto readRequestLogInputDto);

        /// <summary>
        /// 删除指定时间范围的操作日志
        /// </summary>
        /// <returns></returns>
        BaseResponse DeleteOperationlogByRange(ReadOperationLogInputDto readOperationLogInputDto);

        /// <summary>
        /// 删除操作日志
        /// </summary>
        /// <returns></returns>
        BaseResponse DeleteOperationlog(DeleteOperationLogInputDto deleteOperationLogInputDto);
    }
}
