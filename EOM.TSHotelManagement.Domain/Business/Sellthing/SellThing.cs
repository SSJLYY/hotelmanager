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
 *模块说明：商品信息类
 */
//using System.ComponentModel.DataAnnotations.Schema;

using SqlSugar;

namespace EOM.TSHotelManagement.Domain
{
    /// <summary>
    /// 商品信息表 (Product Information)
    /// </summary>
    [SugarTable("sellthing", "商品信息表 (Product Information)")]
    public class SellThing : SoftDeleteEntity
    {
        /// <summary>
        /// 编号 (ID)
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号 (ID)")]
        public int Id { get; set; }

        /// <summary>
        /// 商品编号 (Product Number)
        /// </summary>
        [SugarColumn(
            ColumnName = "sell_no",
            IsPrimaryKey = true,
            ColumnDescription = "商品唯一编号 (Unique Product ID)",
            IsNullable = false,
            Length = 128
        )]
        public string ProductNumber { get; set; }

        /// <summary>
        /// 商品名称 (Product Name)
        /// </summary>
        [SugarColumn(
            ColumnName = "sell_name",
            ColumnDescription = "商品名称（如『西湖龙井茶叶500g』）",
            IsNullable = false,
            Length = 500
        )]
        public string ProductName { get; set; }

        /// <summary>
        /// 商品价格 (Product Price)
        /// </summary>
        [SugarColumn(
            ColumnName = "sell_price",
            Length = 18,
            ColumnDescription = "商品单价（单位：元）",
            IsNullable = false,
            DecimalDigits = 2
        )]
        public decimal ProductPrice { get; set; }

        /// <summary>
        /// 商品价格描述（格式化显示，如￥100.00） (Formatted Price)
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string ProductPriceFormatted { get; set; }

        /// <summary>
        /// 规格型号 (Specification)
        /// </summary>
        [SugarColumn(
            ColumnName = "specification",
            ColumnDescription = "规格描述（如『500g/罐，陶瓷包装』）",
            IsNullable = true,
            Length = 1000
        )]
        public string Specification { get; set; }

        /// <summary>
        /// 库存数量 (Stock Quantity)
        /// </summary>
        [SugarColumn(
            ColumnName = "stock",
            ColumnDescription = "当前库存数量（单位：件/个）",
            IsNullable = false,
            DecimalDigits = 0,
            DefaultValue = "0"
        )]
        public int Stock { get; set; }
    }

}
