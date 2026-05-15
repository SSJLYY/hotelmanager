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
 *模块说明：房间类型类
 */

using SqlSugar;

namespace EOM.TSHotelManagement.Domain
{
    /// <summary>
    /// 房间类型配置表 (Room Type Configuration)
    /// </summary>
    [SugarTable("room_type", "房间类型配置表 (Room Type Configuration)")]
    public class RoomType : SoftDeleteEntity
    {
        /// <summary>
        /// 编号 (ID)
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号 (ID)")]
        public int Id { get; set; }

        /// <summary>
        /// 类型编号 (Room Type ID)
        /// </summary>
        [SugarColumn(
            ColumnName = "room_type",
            IsPrimaryKey = true,
            ColumnDescription = "房间类型唯一编号 (Unique Room Type ID)",
            IsNullable = false
        )]

        public int RoomTypeId { get; set; }

        /// <summary>
        /// 房间类型名称 (Room Type Name)
        /// </summary>
        [SugarColumn(
            ColumnName = "room_name",
            ColumnDescription = "房间类型名称 (如标准间/豪华套房)",
            IsNullable = false,
            Length = 200
        )]

        public string RoomTypeName { get; set; }

        /// <summary>
        /// 房间租金 (Room Rent)
        /// </summary>
        [SugarColumn(
            ColumnName = "room_rent",
            Length = 18,
            ColumnDescription = "每日租金（单位：元） (Price per Day in CNY)",
            IsNullable = false,
            DecimalDigits = 2
        )]

        public decimal RoomRent { get; set; }

        /// <summary>
        /// 房间押金 (Room Deposit)
        /// </summary>
        [SugarColumn(
            ColumnName = "room_deposit",
            Length = 18,
            ColumnDescription = "入住押金（单位：元） (Deposit Amount in CNY)",
            IsNullable = false,
            DecimalDigits = 2,
            DefaultValue = "0.00"
        )]

        public decimal RoomDeposit { get; set; }

        [SugarColumn(
            ColumnName = "pricing_items_json",
            ColumnDataType = "text",
            IsNullable = true,
            ColumnDescription = "Additional Pricing Items JSON"
        )]
        public string PricingItemsJson { get; set; }

        /// <summary>
        /// 删除标记描述 (Delete Mark Description)（不存储到数据库）
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string DeleteMarkDescription { get; set; }
    }

}
