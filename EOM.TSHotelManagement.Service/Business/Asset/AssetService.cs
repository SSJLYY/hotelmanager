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
using SqlSugar;

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 资产信息接口实现类
    /// </summary>
    public class AssetService : IAssetService
    {
        /// <summary>
        /// 资产信息
        /// </summary>
        private readonly GenericRepository<Asset> assetRepository;

        /// <summary>
        /// 部门
        /// </summary>
        private readonly GenericRepository<Department> deptRepository;

        /// <summary>
        /// 员工
        /// </summary>
        private readonly GenericRepository<Domain.Employee> employeeRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetRepository"></param>
        /// <param name="deptRepository"></param>
        /// <param name="employeeRepository"></param>
        public AssetService(GenericRepository<Asset> assetRepository, GenericRepository<Department> deptRepository, GenericRepository<Domain.Employee> employeeRepository)
        {
            this.assetRepository = assetRepository;
            this.deptRepository = deptRepository;
            this.employeeRepository = employeeRepository;
        }

        /// <summary>
        /// 添加资产信息
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public BaseResponse AddAssetInfo(CreateAssetInputDto asset)
        {
            try
            {
                if (assetRepository.IsAny(a => a.AssetNumber == asset.AssetNumber))
                {
                    return new BaseResponse() { Message = LocalizationHelper.GetLocalizedString("the asset number already exists.", "资产编号已存在"), Code = BusinessStatusCode.InternalServerError };
                }
                var entity = EntityMapper.Map<CreateAssetInputDto, Asset>(asset);
                var result = assetRepository.Insert(entity);
                if (!result)
                {
                    return new BaseResponse() { Message = LocalizationHelper.GetLocalizedString("insert asset failed.", "资产添加失败"), Code = BusinessStatusCode.InternalServerError };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse() { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();
        }

        /// <summary>
        /// 查询资产信息
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<ReadAssetOutputDto> SelectAssetInfoAll(ReadAssetInputDto asset)
        {
            //查询所有部门信息
            List<Department> depts = deptRepository.GetList();
            //查询所有员工信息
            List<Domain.Employee> employees = employeeRepository.GetList();

            var where = SqlFilterBuilder.BuildExpression<Asset, ReadAssetInputDto>(asset, nameof(Asset.AcquisitionDate));

            where = where.And(a => a.IsDelete == asset.IsDelete);

            int count = 0;
            List<Asset> assets = new List<Asset>();
            if (!asset.IgnorePaging)
            {
                assets = assetRepository.AsQueryable().Where(where.ToExpression()).OrderBy(a => a.AssetNumber)
                    .ToPageList((int)asset.Page, (int)asset.PageSize, ref count);
            }
            else
            {
                assets = assetRepository.AsQueryable().Where(where.ToExpression()).OrderBy(a => a.AssetNumber).ToList();
                count = assets.Count;
            }

            var mapped = EntityMapper.MapList<Asset, ReadAssetOutputDto>(assets);

            mapped.ForEach(source =>
            {
                var dept = depts.SingleOrDefault(a => a.DepartmentNumber.Equals(source.DepartmentCode));
                source.DepartmentName = dept == null ? "" : dept.DepartmentName;
                var worker = employees.SingleOrDefault(a => a.EmployeeId.Equals(source.AcquiredByEmployeeId));
                source.AcquiredByName = worker == null ? "" : worker.Name;
                source.AssetValueFormatted = source.AssetValue == 0 ? "" : Decimal.Parse(source.AssetValue.ToString()).ToString("#,##0.00").ToString();
            });

            return new ListOutputDto<ReadAssetOutputDto>
            {
                Data = new PagedData<ReadAssetOutputDto>
                {
                    Items = mapped,
                    TotalCount = count
                }
            };
        }

        /// <summary>
        /// 更新资产信息
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public BaseResponse UpdAssetInfo(UpdateAssetInputDto asset)
        {
            try
            {
                if (!assetRepository.IsAny(a => a.AssetNumber == asset.AssetNumber))
                {
                    return new BaseResponse() { Message = LocalizationHelper.GetLocalizedString("asset number does not exist.", "资产编号不存在"), Code = BusinessStatusCode.InternalServerError };
                }
                var entity = EntityMapper.Map<UpdateAssetInputDto, Asset>(asset);
                var result = assetRepository.Update(entity);
                if (!result)
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse() { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();
        }

        /// <summary>
        /// 删除资产信息
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public BaseResponse DelAssetInfo(DeleteAssetInputDto asset)
        {
            try
            {
                if (asset?.DelIds == null || !asset.DelIds.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("Parameters Invalid", "参数错误")
                    };
                }

                var delIds = DeleteConcurrencyHelper.GetDeleteIds(asset);
                var assets = assetRepository.GetList(a => delIds.Contains(a.Id));

                if (!assets.Any())
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("Asset Information Not Found", "资产信息未找到")
                    };
                }

                if (DeleteConcurrencyHelper.HasDeleteConflict(asset, assets, a => a.Id, a => a.RowVersion))
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }

                var result = assetRepository.SoftDeleteRange(assets);

                if (!result)
                {
                    return new BaseResponse
                    {
                        Code = BusinessStatusCode.InternalServerError,
                        Message = LocalizationHelper.GetLocalizedString("Delete failure", "删除失败")
                    };
                }

                return new BaseResponse
                {
                    Code = BusinessStatusCode.Success,
                    Message = LocalizationHelper.GetLocalizedString("Delete successful", "删除成功")
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse() { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
        }
    }
}
