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
 *模块说明：打卡考勤类
 */
using SqlSugar;
using System;

namespace EOM.TSHotelManagement.Domain
{
    /// <summary>
    /// 员工打卡考勤 (Employee Check-in/Check-out Record)
    /// </summary>
    [SugarTable("employee_check", "员工打卡考勤 (Employee Check-in/Check-out Record)")]
    public class EmployeeCheck : SoftDeleteEntity
    {
        /// <summary>
        /// 编号 (ID)
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号 (ID)")]
        public int Id { get; set; }

        /// <summary>
        /// 打卡编号
        /// </summary>
        [SugarColumn(ColumnName = "check_number", Length = 128, ColumnDescription = "打卡编号", IsPrimaryKey = true, IsNullable = false)]
        public string CheckNumber { get; set; }
        /// <summary>
        /// 员工工号 (Employee ID)
        /// </summary>
        [SugarColumn(ColumnName = "employee_number", Length = 128, ColumnDescription = "员工工号 (Employee ID)", IsNullable = false)]
        public string EmployeeId { get; set; }

        /// <summary>
        /// 打卡时间 (Check-in/Check-out Time)
        /// </summary>
        [SugarColumn(ColumnName = "check_time", ColumnDescription = "打卡时间 (Check-in/Check-out Time)", IsNullable = false)]
        public DateTime CheckTime { get; set; }

        /// <summary>
        /// 打卡方式 (Check-in/Check-out Method)
        /// </summary>
        [SugarColumn(ColumnName = "check_way", Length = 128, ColumnDescription = "打卡方式 (Check-in/Check-out Method)", IsNullable = true)]
        public string CheckMethod { get; set; }

        /// <summary>
        /// 打卡状态 (Check-in/Check-out Status)
        /// </summary>
        [SugarColumn(ColumnName = "check_state", ColumnDescription = "打卡状态 (Check-in/Check-out Status)", IsNullable = false)]
        public int CheckStatus { get; set; }
    }
}
