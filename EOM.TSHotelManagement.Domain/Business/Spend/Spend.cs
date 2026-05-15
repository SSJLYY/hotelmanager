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
 *模块说明：消费信息类
 */
using SqlSugar;
using System;

namespace EOM.TSHotelManagement.Domain
{
    /// <summary>
    /// 消费信息 (Consumption Information)
    /// </summary>
    [SugarTable("customer_spend")]
    public class Spend : SoftDeleteEntity
    {
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号 (ID)")]
        public int Id { get; set; }
        /// <summary>
        /// 消费编号 (Consumption Number)
        /// </summary>
        [SugarColumn(ColumnName = "spend_number", IsPrimaryKey = true, ColumnDescription = "消费编号")]
        public string SpendNumber { get; set; }
        /// <summary>
        /// 房间编号 (Room Number)
        /// </summary>
        [SugarColumn(ColumnName = "room_no", ColumnDescription = "房间编号")]
        public string RoomNumber { get; set; }

        [SugarColumn(ColumnName = "room_id", ColumnDescription = "Room ID", IsNullable = true)]
        public int? RoomId { get; set; }

        /// <summary>
        /// 客户编号 (Customer Number)
        /// </summary>
        [SugarColumn(ColumnName = "custo_no", ColumnDescription = "客户编号")]
        public string CustomerNumber { get; set; }

        /// <summary>
        /// 商品编号 (Product Number)
        /// </summary>
        [SugarColumn(ColumnName = "product_number", ColumnDescription = "商品编号")]
        public string ProductNumber { get; set; }

        /// <summary>
        /// 商品名称 (Product Name)
        /// </summary>
        [SugarColumn(ColumnName = "spend_name", ColumnDescription = "商品名称")]
        public string ProductName { get; set; }

        /// <summary>
        /// 消费数量 (Consumption Quantity)
        /// </summary>
        [SugarColumn(ColumnName = "apend_quantity", ColumnDescription = "消费数量")]
        public int ConsumptionQuantity { get; set; }

        /// <summary>
        /// 商品单价 (Product Price)
        /// </summary>
        [SugarColumn(ColumnName = "spend_price", ColumnDescription = "商品单价")]
        public decimal ProductPrice { get; set; }

        /// <summary>
        /// 商品单价描述 (Product Price Description - Formatted)
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string ProductPriceFormatted { get; set; }

        /// <summary>
        /// 消费金额 (Consumption Amount)
        /// </summary>
        [SugarColumn(ColumnName = "spend_amount", ColumnDescription = "消费金额")]
        public decimal ConsumptionAmount { get; set; }

        /// <summary>
        /// 消费金额描述 (Consumption Amount Description - Formatted)
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string ConsumptionAmountFormatted { get; set; }

        /// <summary>
        /// 消费时间 (Consumption Time)
        /// </summary>
        [SugarColumn(ColumnName = "spend_time", ColumnDescription = "消费时间")]
        public DateTime ConsumptionTime { get; set; }

        /// <summary>
        /// 消费类型 (Consumption Type)
        /// </summary>
        [SugarColumn(ColumnName = "consumption_type", ColumnDescription = "消费类型", IsNullable = true)]
        public string ConsumptionType { get; set; }

        /// <summary>
        /// 消费类型 (Consumption Type Description)
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string ConsumptionTypeDescription { get; set; }

        /// <summary>
        /// 结算状态 (Settlement Status)
        /// </summary>
        [SugarColumn(ColumnName = "settlement_status", ColumnDescription = "结算状态")]
        public string SettlementStatus { get; set; }

        /// <summary>
        /// 结算状态描述 (Settlement Status Description)
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string SettlementStatusDescription { get; set; }
    }

}
