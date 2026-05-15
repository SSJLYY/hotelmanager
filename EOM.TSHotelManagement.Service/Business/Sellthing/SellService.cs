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
using jvncorelib.EntityLib;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 商品信息接口实现类
    /// </summary>
    public class SellService : ISellService
    {
        /// <summary>
        /// 商品信息
        /// </summary>
        private readonly GenericRepository<SellThing> sellThingRepository;

        /// <summary>
        /// 消费情况
        /// </summary>
        private readonly GenericRepository<Spend> spendRepository;
        private readonly ILogger<SellService> logger;

        public SellService(GenericRepository<SellThing> sellThingRepository, GenericRepository<Spend> spendRepository, ILogger<SellService> logger)
        {
            this.sellThingRepository = sellThingRepository;
            this.spendRepository = spendRepository;
            this.logger = logger;
        }

        /// <summary>
        /// 查询所有商品
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<ReadSellThingOutputDto> SelectSellThingAll(ReadSellThingInputDto sellThing)
        {
            sellThing ??= new ReadSellThingInputDto();

            var exp = Expressionable.Create<SellThing>();

            exp = exp.And(a => a.Stock > 0 && a.IsDelete == sellThing.IsDelete);

            //商品编号
            if (!sellThing.ProductNumber.IsNullOrEmpty())
            {
                exp = exp.And(a => a.ProductNumber.Contains(sellThing.ProductNumber));
            }

            //商品名称
            if (!sellThing.ProductName.IsNullOrEmpty())
            {
                exp = exp.And(a => a.ProductName.Contains(sellThing.ProductName));
            }

            //商品规格
            if (!sellThing.Specification.IsNullOrEmpty())
            {
                exp = exp.And(a => a.Specification.Contains(sellThing.Specification));
            }

            var count = 0;
            List<SellThing> sellThings;

            if (!sellThing.IgnorePaging)
            {
                var page = sellThing.Page > 0 ? sellThing.Page : 1;
                var pageSize = sellThing.PageSize > 0 ? sellThing.PageSize : 15;
                sellThings = sellThingRepository.AsQueryable().Where(exp.ToExpression()).ToPageList(page, pageSize, ref count);
            }
            else
            {
                sellThings = sellThingRepository.GetList(exp.ToExpression());
                count = sellThings.Count;
            }

            List<ReadSellThingOutputDto> result;
            var useParallelProjection = sellThing.IgnorePaging && sellThings.Count >= 200;
            if (useParallelProjection)
            {
                var dtoArray = new ReadSellThingOutputDto[sellThings.Count];
                System.Threading.Tasks.Parallel.For(0, sellThings.Count, i =>
                {
                    var source = sellThings[i];
                    dtoArray[i] = new ReadSellThingOutputDto
                    {
                        Id = source.Id,
                        ProductNumber = source.ProductNumber,
                        ProductName = source.ProductName,
                        ProductPrice = source.ProductPrice,
                        Specification = source.Specification,
                        Stock = source.Stock,
                        DataInsUsr = source.DataInsUsr,
                        DataInsDate = source.DataInsDate,
                        DataChgUsr = source.DataChgUsr,
                        DataChgDate = source.DataChgDate,
                        RowVersion = source.RowVersion,
                        IsDelete = source.IsDelete
                    };
                });
                result = dtoArray.ToList();
            }
            else
            {
                result = new List<ReadSellThingOutputDto>(sellThings.Count);
                sellThings.ForEach(source =>
                {
                    result.Add(new ReadSellThingOutputDto
                    {
                        Id = source.Id,
                        ProductNumber = source.ProductNumber,
                        ProductName = source.ProductName,
                        ProductPrice = source.ProductPrice,
                        Specification = source.Specification,
                        Stock = source.Stock,
                        DataInsUsr = source.DataInsUsr,
                        DataInsDate = source.DataInsDate,
                        DataChgUsr = source.DataChgUsr,
                        DataChgDate = source.DataChgDate,
                        RowVersion = source.RowVersion,
                        IsDelete = source.IsDelete
                    });
                });
            }

            return new ListOutputDto<ReadSellThingOutputDto>
            {
                Data = new PagedData<ReadSellThingOutputDto>
                {
                    Items = result,
                    TotalCount = count
                }
            };
        }

        /// <summary>
        /// 修改商品信息
        /// </summary>
        /// <param name="sellThing"></param>
        /// <returns></returns>
        public BaseResponse UpdateSellthing(UpdateSellThingInputDto sellThing)
        {
            try
            {
                var product = sellThingRepository.GetFirst(a => a.Id == sellThing.Id);
                if (product == null)
                {
                    return new BaseResponse(BusinessStatusCode.NotFound, LocalizationHelper.GetLocalizedString("Goods Information Not Found", "商品信息未找到"));
                }
                product.ProductName = sellThing.ProductName;
                product.ProductPrice = sellThing.ProductPrice;
                product.Stock = sellThing.Stock;
                product.Specification = sellThing.Specification;
                product.IsDelete = sellThing.IsDelete;
                product.RowVersion = sellThing.RowVersion ?? 0;
                var result = sellThingRepository.Update(product);
                if (!result)
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating sell thing: {Message}", ex.Message);
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();
        }

        /// <summary>
        /// 删除商品信息
        /// </summary>
        /// <param name="deleteSellThingInputDto"></param>
        /// <returns></returns>
        public BaseResponse DeleteSellthing(DeleteSellThingInputDto deleteSellThingInputDto)
        {
            try
            {
                if (deleteSellThingInputDto?.DelIds == null || !deleteSellThingInputDto.DelIds.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("Parameters Invalid", "参数错误")
                    };
                }

                var delIds = DeleteConcurrencyHelper.GetDeleteIds(deleteSellThingInputDto);
                var sellThings = sellThingRepository.GetList(a => delIds.Contains(a.Id));

                if (!sellThings.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("Goods Information Not Found", "商品信息未找到")
                    };
                }

                if (DeleteConcurrencyHelper.HasDeleteConflict(deleteSellThingInputDto, sellThings, a => a.Id, a => a.RowVersion))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                var result = sellThingRepository.SoftDeleteRange(sellThings);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting sell thing: {Message}", ex.Message);
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();
        }

        /// <summary>
        /// 根据商品名称和价格查询商品编号
        /// </summary>
        /// <param name="readSellThingInputDto"></param>
        /// <returns></returns>
        public SingleOutputDto<ReadSellThingOutputDto> SelectSellThingByNameAndPrice(ReadSellThingInputDto readSellThingInputDto)
        {
            SellThing sellThing = null;
            sellThing = sellThingRepository.GetFirst(a => a.ProductNumber == readSellThingInputDto.ProductNumber || (a.ProductName == readSellThingInputDto.ProductName
            && a.ProductPrice == Convert.ToDecimal(readSellThingInputDto.ProductPrice)));

            var source = EntityMapper.Map<SellThing, ReadSellThingOutputDto>(sellThing);

            return new SingleOutputDto<ReadSellThingOutputDto>() { Data = source };
        }

        /// <summary>
        /// 添加商品
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        public BaseResponse InsertSellThing(CreateSellThingInputDto st)
        {
            try
            {
                var entity = EntityMapper.Map<CreateSellThingInputDto, SellThing>(st);
                sellThingRepository.Insert(entity);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inserting sell thing: {Message}", ex.Message);
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();
        }
    }
}
