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
 *模块说明：客户类型类
 */
using SqlSugar;

namespace EOM.TSHotelManagement.Domain
{
    /// <summary>
    /// 客户类型
    /// </summary>
    [SqlSugar.SugarTable("custo_type")]
    public class CustoType : SoftDeleteEntity
    {
        /// <summary>
        /// 编号 (ID)
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号 (ID)")]
        public int Id { get; set; }

        /// <summary>
        /// 客户类型 (Customer Type)
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "custo_type", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "客户类型 (Customer Type)")]
        public int CustomerType { get; set; }

        /// <summary>
        /// 客户类型名称 (Customer Type Name)
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "type_name", IsNullable = false, Length = 256, ColumnDescription = "客户类型名称 (Customer Type Name)")]
        public string CustomerTypeName { get; set; }

        /// <summary>
        /// 优惠折扣 (Discount)
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "discount", IsNullable = false, Length = 18, DecimalDigits = 2, ColumnDescription = "优惠折扣 (Discount)")]
        public decimal Discount { get; set; } = 0M;

    }
}
