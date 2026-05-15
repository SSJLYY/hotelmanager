using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Common.Helper;
using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Data;
using EOM.TSHotelManagement.Domain;
using jvncorelib.EntityLib;
using Microsoft.Extensions.Logging;

namespace EOM.TSHotelManagement.Service.Business.News
{
    public class NewsService(GenericRepository<Domain.News> _newsRepository, ILogger<NewsService> _logger) : INewsService
    {

        /// <summary>
        /// 查询新闻列表
        /// </summary>
        /// <param name="readNewsInputDto"></param>
        /// <returns></returns>
        public ListOutputDto<ReadNewsOuputDto> SelectNews(ReadNewsInputDto readNewsInputDto)
        {
            var newsTypes = Enum.GetValues(typeof(NewsType))
                .Cast<NewsType>()
                .Select(e => new EnumDto
                {
                    Id = (int)e,
                    Name = e.ToString(),
                    Description = EnumHelper.GetEnumDescription(e)
                })
                .ToList();

            var newsStatuss = Enum.GetValues(typeof(NewsStatus))
                .Cast<NewsStatus>()
                .Select(e => new EnumDto
                {
                    Id = (int)e,
                    Name = e.ToString(),
                    Description = EnumHelper.GetEnumDescription(e)
                })
                .ToList();

            var where = SqlFilterBuilder.BuildExpression<Domain.News, ReadNewsInputDto>(readNewsInputDto);

            var count = 0;
            var newsList = new List<Domain.News>();

            if (readNewsInputDto.IgnorePaging)
            {
                newsList = _newsRepository.AsQueryable().Where(where.ToExpression()).ToList();
                count = newsList.Count;
            }
            else
            {
                newsList = _newsRepository.AsQueryable().Where(where.ToExpression()).ToPageList(readNewsInputDto.Page, readNewsInputDto.PageSize, ref count);
            }

            var mappedList = EntityMapper.MapList<Domain.News, ReadNewsOuputDto>(newsList);

            mappedList.ForEach(source =>
            {
                var newsType = newsTypes.SingleOrDefault(a => a.Name == source.NewsType);
                source.NewsTypeDescription = newsType?.Description ?? "";
                var newsStatus = newsStatuss.SingleOrDefault(a => a.Name == source.NewsStatus);
                source.NewsStatusDescription = newsStatus?.Description ?? "";
            });

            return new ListOutputDto<ReadNewsOuputDto>
            {
                Data = new PagedData<ReadNewsOuputDto>
                {
                    Items = mappedList,
                    TotalCount = count
                }
            };
        }

        /// <summary>
        /// 查询单条新闻
        /// </summary>
        /// <param name="readNewsInputDto"></param>
        /// <returns></returns>
        public SingleOutputDto<ReadNewsOuputDto> News(ReadNewsInputDto readNewsInputDto)
        {
            var news = _newsRepository.AsQueryable().Where(a => a.NewId == readNewsInputDto.NewId).Single();
            if (news.IsNullOrEmpty())
            {
                return new SingleOutputDto<ReadNewsOuputDto>
                {
                    Code = BusinessStatusCode.NotFound,
                    Message = "新闻未找到"
                };
            }

            var outputDto = EntityMapper.Map<Domain.News, ReadNewsOuputDto>(news);
            var newsType = Enum.GetValues(typeof(NewsType))
                .Cast<NewsType>()
                .Select(e => new EnumDto
                {
                    Id = (int)e,
                    Name = e.ToString(),
                    Description = EnumHelper.GetEnumDescription(e)
                })
                .SingleOrDefault(a => a.Name == outputDto.NewsType);
            outputDto.NewsTypeDescription = newsType?.Description ?? "";
            var newsStatus = Enum.GetValues(typeof(NewsStatus))
                .Cast<NewsStatus>()
                .Select(e => new EnumDto
                {
                    Id = (int)e,
                    Name = e.ToString(),
                    Description = EnumHelper.GetEnumDescription(e)
                })
                .SingleOrDefault(a => a.Name == outputDto.NewsStatus);
            outputDto.NewsStatusDescription = newsStatus?.Description ?? "";

            return new SingleOutputDto<ReadNewsOuputDto>
            {
                Data = outputDto
            };
        }

        /// <summary>
        /// 添加新闻
        /// </summary>
        /// <param name="addNewsInputDto"></param>
        /// <returns></returns>
        public BaseResponse AddNews(AddNewsInputDto addNewsInputDto)
        {
            var news = new Domain.News
            {
                NewId = Guid.NewGuid().ToString(),
                NewsTitle = addNewsInputDto.NewsTitle,
                NewsContent = addNewsInputDto.NewsContent,
                NewsType = addNewsInputDto.NewsType,
                NewsStatus = addNewsInputDto.NewsStatus,
                NewsLink = addNewsInputDto.NewsLink,
                NewsDate = addNewsInputDto.NewsDate,
                NewsImage = addNewsInputDto.NewsImage
            };
            try
            {
                var result = _newsRepository.Insert(news);
                if (!result)
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.InternalServerError,
                        Message = "新闻添加失败"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "添加新闻时发生异常");
                return new BaseResponse
                {
                    Code = BusinessStatusCode.InternalServerError,
                    Message = $"新闻添加失败: {ex.Message}"
                };
            }
            return new BaseResponse
            {
                Code = BusinessStatusCode.Success,
                Message = "新闻添加成功"
            };
        }

        /// <summary>
        /// 更新新闻
        /// </summary>
        /// <param name="updateNewsInputDto"></param>
        /// <returns></returns>
        public BaseResponse UpdateNews(UpdateNewsInputDto updateNewsInputDto)
        {
            var news = _newsRepository.AsQueryable().Where(a => a.NewId == updateNewsInputDto.NewId).Single();
            if (news.IsNullOrEmpty())
            {
                return new BaseResponse
                {
                    Code = BusinessStatusCode.NotFound,
                    Message = "新闻未找到"
                };
            }
            news.NewsTitle = updateNewsInputDto.NewsTitle;
            news.NewsContent = updateNewsInputDto.NewsContent;
            news.NewsType = updateNewsInputDto.NewsType;
            news.NewsStatus = updateNewsInputDto.NewsStatus;
            news.NewsLink = updateNewsInputDto.NewsLink;
            news.NewsDate = updateNewsInputDto.NewsDate;
            news.NewsImage = updateNewsInputDto.NewsImage;
            news.RowVersion = updateNewsInputDto.RowVersion ?? 0;
            try
            {
                var result = _newsRepository.Update(news);
                if (!result)
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新新闻时发生异常");
                return new BaseResponse
                {
                    Code = BusinessStatusCode.InternalServerError,
                    Message = $"新闻更新失败: {ex.Message}"
                };
            }
            return new BaseResponse
            {
                Code = BusinessStatusCode.Success,
                Message = "新闻更新成功"
            };
        }

        /// <summary>
        /// 删除新闻
        /// </summary>
        /// <param name="deleteNewsInputDto"></param>
        /// <returns></returns>
        public BaseResponse DeleteNews(DeleteNewsInputDto deleteNewsInputDto)
        {
            if (deleteNewsInputDto?.DelIds == null || !deleteNewsInputDto.DelIds.Any())
            {
                return new BaseResponse
                {
                    Code = BusinessStatusCode.BadRequest,
                    Message = LocalizationHelper.GetLocalizedString("Parameters Invalid", "参数错误")
                };
            }

            var delIds = DeleteConcurrencyHelper.GetDeleteIds(deleteNewsInputDto);
            var news = _newsRepository.GetList(a => delIds.Contains(a.Id));

            if (!news.Any())
            {
                return new BaseResponse
                {
                    Code = BusinessStatusCode.NotFound,
                    Message = LocalizationHelper.GetLocalizedString("News Information Not Found", "新闻信息未找到")
                };
            }

            if (DeleteConcurrencyHelper.HasDeleteConflict(deleteNewsInputDto, news, a => a.Id, a => a.RowVersion))
            {
                return BaseResponseFactory.ConcurrencyConflict();
            }
            
            try
            {
                var result = _newsRepository.SoftDeleteRange(news);

                if (result)
                {
                    return new BaseResponse(BusinessStatusCode.Success, LocalizationHelper.GetLocalizedString("Delete News Success", "新闻信息删除成功"));
                }
                else
                {
                    return new BaseResponse(BusinessStatusCode.InternalServerError, LocalizationHelper.GetLocalizedString("Delete News Failed", "新闻信息删除失败"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除新闻时发生异常");
                return new BaseResponse
                {
                    Code = BusinessStatusCode.InternalServerError,
                    Message = $"新闻删除失败: {ex.Message}"
                };
            }
        }
    }
}
