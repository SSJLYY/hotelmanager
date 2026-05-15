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
 *模块说明：预约类
 */
using SqlSugar;
using System;

namespace EOM.TSHotelManagement.Domain
{
    /// <summary>
    /// 预约信息表 (Reservation Information)
    /// </summary>
    [SugarTable("reser", "预约信息表 (Reservation Information)")]
    public class Reser : SoftDeleteEntity
    {
        /// <summary>
        /// 编号 (ID)
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号 (ID)")]
        public int Id { get; set; }

        /// <summary>
        /// 预约编号 (Reservation ID)
        /// </summary>
        [SugarColumn(
            ColumnName = "reser_number",
            IsPrimaryKey = true,
            ColumnDescription = "预约唯一编号 (Unique Reservation ID)",
            IsNullable = false,
            Length = 128
        )]
        public string ReservationId { get; set; }

        /// <summary>
        /// 客户名称 (Customer Name)
        /// </summary>
        [SugarColumn(
            ColumnName = "custo_name",
            ColumnDescription = "客户姓名 (Customer Full Name)",
            IsNullable = false,
            Length = 200
        )]
        public string CustomerName { get; set; }

        /// <summary>
        /// 预约电话 (Reservation Phone Number)
        /// </summary>
        [SugarColumn(
            ColumnName = "custo_tel",
            ColumnDescription = "客户联系电话 (Contact Phone Number)",
            IsNullable = false,
            Length = 256
        )]
        public string ReservationPhoneNumber { get; set; }

        /// <summary>
        /// 预约渠道 (Reservation Channel)
        /// </summary>
        [SugarColumn(
            ColumnName = "reser_way",
            ColumnDescription = "预约来源渠道 (如官网/APP/第三方平台)",
            IsNullable = false,
            Length = 50
        )]
        public string ReservationChannel { get; set; }

        /// <summary>
        /// 预约房号 (Reservation Room Number)
        /// </summary>
        [SugarColumn(
            ColumnName = "reser_room",
            ColumnDescription = "预定房间编号 (关联房间表)",
            IsNullable = false,
            Length = 128,
            IndexGroupNameList = new[] { "IX_reser_room" }
        )]
        public string ReservationRoomNumber { get; set; }

        [SugarColumn(ColumnName = "room_id", ColumnDescription = "Room ID", IsNullable = true)]
        public int? RoomId { get; set; }

        /// <summary>
        /// 预约起始日期 (Reservation Start Date)
        /// </summary>
        [SugarColumn(
            ColumnName = "reser_date",
            ColumnDescription = "入住日期（格式：yyyy-MM-dd） (Check-In Date)",
            IsNullable = false
        )]
        public DateOnly ReservationStartDate { get; set; }

        /// <summary>
        /// 预约结束日期 (Reservation End Date)
        /// </summary>
        [SugarColumn(
            ColumnName = "reser_end_date",
            ColumnDescription = "离店日期（格式：yyyy-MM-dd） (Check-Out Date)",
            IsNullable = false
        )]
        public DateOnly ReservationEndDate { get; set; }

        /// <summary>
        /// 预约状态 (Reservation Status)
        /// </summary>
        [SugarColumn(
            ColumnName = "reser_status",
            ColumnDescription = "预约状态（0-待确认/1-已确认/2-已取消）",
            DefaultValue = "0"
        )]
        public int ReservationStatus { get; set; }

        /// <summary>
        /// 备注信息 (Remarks)
        /// </summary>
        [SugarColumn(
            ColumnName = "remarks",
            ColumnDescription = "特殊需求备注 (Special Requests)",
            IsNullable = true,
            Length = 500
        )]
        public string Remarks { get; set; }
    }
}
