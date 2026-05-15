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
 *模块说明：任命公告类
 */
using SqlSugar;
using System;

namespace EOM.TSHotelManagement.Domain
{
    /// <summary>
    /// 任命公告 (Appointment AppointmentNotice)
    /// </summary>
    [SugarTable("appointment_notice", "任命公告 (Appointment AppointmentNotice)")]
    public class AppointmentNotice : SoftDeleteEntity
    {
        /// <summary>
        /// 编号 (ID)
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号 (ID)")]
        public int Id { get; set; }

        /// <summary>
        /// 公告编号 (AppointmentNotice Number)
        /// </summary>
        [SugarColumn(ColumnName = "notice_no", IsPrimaryKey = true, IsNullable = false, Length = 128, ColumnDescription = "公告编号 (AppointmentNotice Number)")]
        public string NoticeNumber { get; set; }

        /// <summary>
        /// 公告主题 (AppointmentNotice Theme)
        /// </summary>
        [SugarColumn(ColumnName = "notice_theme", IsNullable = false, Length = 256, ColumnDescription = "公告主题 (AppointmentNotice Theme)")]
        public string NoticeTheme { get; set; }

        /// <summary>
        /// 公告类型 (AppointmentNotice Type)
        /// </summary>
        [SugarColumn(ColumnName = "notice_type", IsNullable = false, Length = 150, ColumnDescription = "公告类型 (AppointmentNotice Type)")]
        public string NoticeType { get; set; }

        /// <summary>
        /// 公告类型(描述) (AppointmentNotice Type Description)
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string NoticeTypeDescription { get; set; }

        /// <summary>
        /// 公告时间 (AppointmentNotice Time)
        /// </summary>
        [SugarColumn(ColumnName = "notice_time", IsNullable = false, ColumnDescription = "公告时间 (AppointmentNotice Time)")]
        public DateTime NoticeTime { get; set; }

        /// <summary>
        /// 公告正文 (AppointmentNotice Content)
        /// </summary>
        [SugarColumn(ColumnName = "notice_content", IsNullable = true, ColumnDataType = "text", ColumnDescription = "公告正文 (AppointmentNotice Content)")]
        public string NoticeContent { get; set; }

        /// <summary>
        /// 发文部门 (Issuing Department)
        /// </summary>
        [SugarColumn(ColumnName = "notice_department", IsNullable = false, Length = 128, ColumnDescription = "发文部门 (Issuing Department)")]
        public string IssuingDepartment { get; set; }
    }
}
