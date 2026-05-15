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
 *模块说明：履历类
 */
using SqlSugar;
using System;

namespace EOM.TSHotelManagement.Domain
{
    /// <summary>
    /// 员工履历 (Employee History)
    /// </summary>
    [SugarTable("employee_history")]
    public class EmployeeHistory : SoftDeleteEntity
    {
        /// <summary>
        /// 编号 (ID)
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号 (ID)")]
        public int Id { get; set; }

        /// <summary>
        /// 履历编号
        /// </summary>
        [SugarColumn(ColumnName = "history_number", ColumnDescription = "履历编号", Length = 128, IsNullable = false, IsPrimaryKey = true)]
        public string HistoryNumber { get; set; }
        /// <summary>
        /// 员工工号 (Employee ID)
        /// </summary>
        [SugarColumn(ColumnName = "employee_number", ColumnDescription = "员工工号 (Employee ID)", Length = 128, IsNullable = false)]
        public string EmployeeId { get; set; }

        /// <summary>
        /// 开始时间 (Start Date)
        /// </summary>
        [SugarColumn(ColumnName = "start_date", ColumnDescription = "开始时间 (Start Date)", IsNullable = false)]
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// 结束时间 (End Date)
        /// </summary>
        [SugarColumn(ColumnName = "end_date", ColumnDescription = "结束时间 (End Date)", IsNullable = false)]
        public DateOnly EndDate { get; set; }

        /// <summary>
        /// 职位 (Position)
        /// </summary>
        [SugarColumn(ColumnName = "position", ColumnDescription = "职位 (Position)", Length = 128, IsNullable = true)]
        public string Position { get; set; }

        /// <summary>
        /// 公司 (Company)
        /// </summary>
        [SugarColumn(ColumnName = "company", ColumnDescription = "公司 (Company)", Length = 256, IsNullable = true)]
        public string Company { get; set; }
    }
}
