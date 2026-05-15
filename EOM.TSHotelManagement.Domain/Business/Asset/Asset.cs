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
 *模块说明：资产类
 */

using SqlSugar;
using System;
namespace EOM.TSHotelManagement.Domain
{
    /// <summary>
    /// 资产管理
    /// </summary>
    [SqlSugar.SugarTable("asset")]
    public class Asset : SoftDeleteEntity
    {
        /// <summary>
        /// 编号 (ID)
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号 (ID)")]
        public int Id { get; set; }

        /// <summary>
        /// 资产编号 (Asset Number)
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "asset_number", IsNullable = false, IsPrimaryKey = true, Length = 128, ColumnDescription = "资产编号")]

        public string AssetNumber { get; set; }

        /// <summary>
        /// 资产名称 (Asset Name)
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "asset_name", IsNullable = false, Length = 200, ColumnDescription = "资产名称")]

        public string AssetName { get; set; }

        /// <summary>
        /// 资产总值 (Asset Value)
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "asset_value", IsNullable = false, DecimalDigits = 2, Length = 18, ColumnDescription = "资产名称")]

        public decimal AssetValue { get; set; }

        /// <summary>
        /// 资产总值描述 (格式化后的字符串) (Asset Value Description - Formatted)
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]

        public string AssetValueFormatted { get; set; }

        /// <summary>
        /// 所属部门代码 (Department Code)
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "department_code", IsNullable = false, Length = 128, ColumnDescription = "所属部门代码 (Department Code)")]

        public string DepartmentCode { get; set; }

        /// <summary>
        /// 所属部门名称 (Department Name)
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string DepartmentName { get; set; }

        /// <summary>
        /// 入库时间 (购置日期) (Acquisition Date)
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "acquisition_date", IsNullable = false, ColumnDescription = "入库时间 (购置日期) (Acquisition Date)")]

        public DateOnly AcquisitionDate { get; set; }

        /// <summary>
        /// 资产来源 (Asset Source)
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "asset_source", IsNullable = false, Length = 500, ColumnDescription = "资产来源 (Asset Source)")]

        public string AssetSource { get; set; }

        /// <summary>
        /// 资产经办人 (员工ID) (Acquired By - Employee ID)
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "acquired_by_employee", IsNullable = false, Length = 128, ColumnDescription = "资产经办人 (员工ID) (Acquired By - Employee ID)")]

        public string AcquiredByEmployeeId { get; set; }

        /// <summary>
        /// 资产经办人姓名 (Acquired By - Employee Name)
        /// </summary>
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string AcquiredByName { get; set; }


    }
}
