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
    /// 酒店宣传联动内容接口实现类
    /// </summary>
    public class PromotionContentService : IPromotionContentService
    {
        /// <summary>
        /// 跑马灯
        /// </summary>
        private readonly GenericRepository<PromotionContent> fontsRepository;
        private readonly ILogger<PromotionContentService> logger;

        public PromotionContentService(GenericRepository<PromotionContent> fontsRepository, ILogger<PromotionContentService> logger)
        {
            this.fontsRepository = fontsRepository;
            this.logger = logger;
        }

        /// <summary>
        /// 查询所有宣传联动内容
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<ReadPromotionContentOutputDto> SelectPromotionContentAll(ReadPromotionContentInputDto readPromotionContentInputDto)
        {
            readPromotionContentInputDto ??= new ReadPromotionContentInputDto();

            var count = 0;
            var where = SqlFilterBuilder.BuildExpression<PromotionContent, ReadPromotionContentInputDto>(readPromotionContentInputDto);
            var query = fontsRepository.AsQueryable();
            var whereExpression = where.ToExpression();
            if (whereExpression != null)
            {
                query = query.Where(whereExpression);
            }

            List<PromotionContent> data;
            if (!readPromotionContentInputDto.IgnorePaging)
            {
                var page = readPromotionContentInputDto.Page > 0 ? readPromotionContentInputDto.Page : 1;
                var pageSize = readPromotionContentInputDto.PageSize > 0 ? readPromotionContentInputDto.PageSize : 15;
                data = query.ToPageList(page, pageSize, ref count);
            }
            else
            {
                data = query.ToList();
                count = data.Count;
            }

            List<ReadPromotionContentOutputDto> mapped;
            var useParallelProjection = readPromotionContentInputDto.IgnorePaging && data.Count >= 200;
            if (useParallelProjection)
            {
                var dtoArray = new ReadPromotionContentOutputDto[data.Count];
                System.Threading.Tasks.Parallel.For(0, data.Count, i =>
                {
                    var source = data[i];
                    dtoArray[i] = new ReadPromotionContentOutputDto
                    {
                        Id = source.Id,
                        PromotionContentNumber = source.PromotionContentNumber,
                        PromotionContentMessage = source.PromotionContentMessage,
                        DataInsUsr = source.DataInsUsr,
                        DataInsDate = source.DataInsDate,
                        DataChgUsr = source.DataChgUsr,
                        DataChgDate = source.DataChgDate,
                        RowVersion = source.RowVersion,
                        IsDelete = source.IsDelete
                    };
                });
                mapped = dtoArray.ToList();
            }
            else
            {
                mapped = new List<ReadPromotionContentOutputDto>(data.Count);
                data.ForEach(source =>
                {
                    mapped.Add(new ReadPromotionContentOutputDto
                    {
                        Id = source.Id,
                        PromotionContentNumber = source.PromotionContentNumber,
                        PromotionContentMessage = source.PromotionContentMessage,
                        DataInsUsr = source.DataInsUsr,
                        DataInsDate = source.DataInsDate,
                        DataChgUsr = source.DataChgUsr,
                        DataChgDate = source.DataChgDate,
                        RowVersion = source.RowVersion,
                        IsDelete = source.IsDelete
                    });
                });
            }

            return new ListOutputDto<ReadPromotionContentOutputDto>
            {
                Data = new PagedData<ReadPromotionContentOutputDto>
                {
                    Items = mapped,
                    TotalCount = count
                }
            };
        }

        /// <summary>
        /// 查询所有宣传联动内容(跑马灯)
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<ReadPromotionContentOutputDto> SelectPromotionContents()
        {
            var Data = fontsRepository.AsQueryable().Where(a => a.IsDelete != 1).ToList();
            var mapped = EntityMapper.MapList<PromotionContent, ReadPromotionContentOutputDto>(Data);
            return new ListOutputDto<ReadPromotionContentOutputDto>
            {
                Data = new PagedData<ReadPromotionContentOutputDto>
                {
                    Items = mapped,
                    TotalCount = mapped.Count
                }
            };
        }

        /// <summary>
        /// 添加宣传联动内容
        /// </summary>
        /// <param name="createPromotionContentInputDto"></param>
        /// <returns></returns>
        public BaseResponse AddPromotionContent(CreatePromotionContentInputDto createPromotionContentInputDto)
        {
            try
            {
                var entity = EntityMapper.Map<CreatePromotionContentInputDto, PromotionContent>(createPromotionContentInputDto);
                fontsRepository.Insert(entity);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, LocalizationHelper.GetLocalizedString("Insert Promotion Content Failed", "宣传联动内容添加失败"));
                return new BaseResponse(BusinessStatusCode.InternalServerError, LocalizationHelper.GetLocalizedString("Insert Promotion Content Failed", "宣传联动内容添加失败"));
            }

            return new BaseResponse(BusinessStatusCode.Success, LocalizationHelper.GetLocalizedString("Insert Promotion Content Success", "宣传联动内容添加成功"));
        }

        /// <summary>
        /// 删除宣传联动内容
        /// </summary>
        /// <param name="deletePromotionContentInputDto"></param>
        /// <returns></returns>
        public BaseResponse DeletePromotionContent(DeletePromotionContentInputDto deletePromotionContentInputDto)
        {
            try
            {
                if (deletePromotionContentInputDto?.DelIds == null || !deletePromotionContentInputDto.DelIds.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("Parameters Invalid", "参数错误")
                    };
                }

                var delIds = DeleteConcurrencyHelper.GetDeleteIds(deletePromotionContentInputDto);
                var promotionContents = fontsRepository.GetList(a => delIds.Contains(a.Id));

                if (!promotionContents.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("Promotion Content Not Found", "宣传联动内容未找到")
                    };
                }

                if (DeleteConcurrencyHelper.HasDeleteConflict(deletePromotionContentInputDto, promotionContents, a => a.Id, a => a.RowVersion))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                var result = fontsRepository.SoftDeleteRange(promotionContents);

                if (result)
                {
                    return new BaseResponse(BusinessStatusCode.Success, LocalizationHelper.GetLocalizedString("Delete Promotion Content Success", "宣传联动内容删除成功"));
                }
                else
                {
                    return new BaseResponse(BusinessStatusCode.InternalServerError, LocalizationHelper.GetLocalizedString("Delete Promotion Content Failed", "宣传联动内容删除失败"));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, LocalizationHelper.GetLocalizedString("Delete Promotion Content Failed", "宣传联动内容删除失败"));
                return new BaseResponse(BusinessStatusCode.InternalServerError, LocalizationHelper.GetLocalizedString("Delete Promotion Content Failed", "宣传联动内容删除失败"));
            }
        }

        /// <summary>
        /// 更新宣传联动内容
        /// </summary>
        /// <param name="updatePromotionContentInputDto"></param>
        /// <returns></returns>
        public BaseResponse UpdatePromotionContent(UpdatePromotionContentInputDto updatePromotionContentInputDto)
        {
            try
            {
                var entity = EntityMapper.Map<UpdatePromotionContentInputDto, PromotionContent>(updatePromotionContentInputDto);
                var result = fontsRepository.Update(entity);
                if (!result)
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, LocalizationHelper.GetLocalizedString("Update Promotion Content Failed", "宣传联动内容更新失败"));
                return new BaseResponse(BusinessStatusCode.InternalServerError, LocalizationHelper.GetLocalizedString("Update Promotion Content Failed", "宣传联动内容更新失败"));
            }

            return new BaseResponse(BusinessStatusCode.Success, LocalizationHelper.GetLocalizedString("Update Promotion Content Success", "宣传联动内容更新成功"));

        }
    }
}
