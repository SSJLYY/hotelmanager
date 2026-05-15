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
 *模块说明：员工信息类
 */
using SqlSugar;
using System;

namespace EOM.TSHotelManagement.Domain
{
    /// <summary>
    /// 员工信息 (Employee Information)
    /// </summary>
    [SugarTable("employee")]
    public class Employee : Personnel //SoftDeleteEntity
    {
        /// <summary>
        /// 编号 (ID)
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号 (ID)")]
        public int Id { get; set; }

        /// <summary>
        /// 员工账号/工号 (Employee Account/ID)
        /// </summary>
        [SugarColumn(ColumnName = "employee_number", IsPrimaryKey = true, IsNullable = false, Length = 128, ColumnDescription = "员工账号/工号 (Employee Account/ID)")]
        public string EmployeeId { get; set; }

        /// <summary>
        /// 所属部门 (Department)
        /// </summary>
        [SugarColumn(ColumnName = "employee_department", IsNullable = false, Length = 128, ColumnDescription = "所属部门 (Department)")]
        public string Department { get; set; }

        /// <summary>
        /// 员工职位 (Employee Position)
        /// </summary>
        [SugarColumn(ColumnName = "employee_postion", IsNullable = false, Length = 128, ColumnDescription = "员工职位 (Employee Position)")]
        public string Position { get; set; }

        /// <summary>
        /// 员工密码 (Employee Password)
        /// </summary>
        [SugarColumn(ColumnName = "employee_password", IsNullable = false, Length = 256, ColumnDescription = "员工密码 (Employee Password)")]
        public string Password { get; set; }

        /// <summary>
        /// 员工入职时间 (Hire Date)
        /// </summary>
        [SugarColumn(ColumnName = "hire_time", IsNullable = false, ColumnDescription = "员工入职时间 (Hire Date)")]
        public DateOnly HireDate { get; set; }

        /// <summary>
        /// 员工面貌 (Political Affiliation)
        /// </summary>
        [SugarColumn(ColumnName = "employee_political", IsNullable = false, Length = 128, ColumnDescription = "员工面貌 (Political Affiliation)")]
        public string PoliticalAffiliation { get; set; }

        /// <summary>
        /// 禁用标记
        /// </summary>
        [SugarColumn(ColumnName = "enable_mk", IsNullable = false, ColumnDescription = "禁用标记")]
        public int IsEnable { get; set; } = 1;

        /// <summary>
        /// 初始化标记
        /// </summary>
        [SugarColumn(ColumnName = "initialize_mk", IsNullable = false, ColumnDescription = "初始化标记")]
        public int IsInitialize { get; set; }
    }
}
