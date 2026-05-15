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
 *模块说明：水电信息类
 */
using SqlSugar;
using System;

namespace EOM.TSHotelManagement.Domain
{
    /// <summary>
    /// 水电信息
    /// </summary>
    [SqlSugar.SugarTable("energy_management", "水电信息")]
    public class EnergyManagement : SoftDeleteEntity
    {
        /// <summary>
        /// 编号 (ID)
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号 (ID)")]
        public int Id { get; set; }

        /// <summary>
        /// 信息编号 (Information ID)
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "information_number", IsPrimaryKey = true, ColumnDescription = "信息编号")]
        public string InformationId { get; set; }

        /// <summary>
        /// 房间编号 (Room Number)
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "room_no", Length = 128, IsNullable = false, ColumnDescription = "房间编号 (Room Number)")]
        public string RoomNumber { get; set; }

        [SqlSugar.SugarColumn(ColumnName = "room_id", IsNullable = true, ColumnDescription = "Room ID")]
        public int? RoomId { get; set; }

        /// <summary>
        /// 开始使用时间 (Start Date)
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "use_date", IsNullable = false, ColumnDescription = "开始使用时间 (Start Date)")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束使用时间 (End Date)
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "end_date", IsNullable = false, ColumnDescription = "结束使用时间 (End Date)")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 水费 (Water Usage)
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "water_use", IsNullable = false, Length = 18, DecimalDigits = 2, ColumnDescription = "水费 (Water Usage)")]
        public decimal WaterUsage { get; set; }

        /// <summary>
        /// 电费 (Power Usage)
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "power_use", IsNullable = false, Length = 18, DecimalDigits = 2, ColumnDescription = "电费 (Power Usage)")]
        public decimal PowerUsage { get; set; }

        /// <summary>
        /// 记录员 (Recorder)
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "recorder", DefaultValue = "Administrator", IsNullable = false, Length = 150, ColumnDescription = "记录员 (Recorder)")]
        public string Recorder { get; set; }

        /// <summary>
        /// 客户编号 (Customer Number)
        /// </summary>
        [SqlSugar.SugarColumn(ColumnName = "custo_no", IsNullable = false, Length = 128, ColumnDescription = "客户编号 (Customer Number)")]
        public string CustomerNumber { get; set; }
    }
}
