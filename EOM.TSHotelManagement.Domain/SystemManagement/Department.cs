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
 *模块说明：部门实体类
 */
using SqlSugar;
using System;

namespace EOM.TSHotelManagement.Domain
{
    /// <summary>
    /// 部门表 (Department Table)
    /// </summary>
    [SugarTable("department")]
    public class Department : SoftDeleteEntity
    {
        /// <summary>
        /// 编号 (ID)
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号 (ID)")]
        public int Id { get; set; }

        /// <summary>
        /// 部门编号 (Department Number)
        /// </summary>
        [SugarColumn(ColumnName = "dept_no", IsNullable = false, Length = 128, ColumnDescription = "部门编号 (Department Number)")]
        public string DepartmentNumber { get; set; }

        /// <summary>
        /// 部门名称 (Department Name)
        /// </summary>
        [SugarColumn(ColumnName = "dept_name", IsNullable = false, Length = 256, ColumnDescription = "部门名称 (Department Name)")]
        public string DepartmentName { get; set; }

        /// <summary>
        /// 部门描述 (Department Description)
        /// </summary>
        [SugarColumn(ColumnName = "dept_desc", IsNullable = true, Length = 500, ColumnDescription = "部门描述 (Department Description)")]
        public string DepartmentDescription { get; set; }

        /// <summary>
        /// 创建时间(部门) (Department Creation Date)
        /// </summary>
        [SugarColumn(ColumnName = "dept_date", IsNullable = true, ColumnDescription = "创建时间(部门) (Department Creation Date)")]
        public DateOnly DepartmentCreationDate { get; set; }

        /// <summary>
        /// 部门主管 (Department Leader)
        /// </summary>
        [SugarColumn(ColumnName = "dept_leader", IsNullable = false, Length = 128, ColumnDescription = "部门主管 (Department Leader)")]
        public string DepartmentLeader { get; set; }

        /// <summary>
        /// 部门主管姓名 (Department Leader Name)
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string LeaderName { get; set; }

        /// <summary>
        /// 上级部门编号 (Parent Department Number)
        /// </summary>
        [SugarColumn(ColumnName = "dept_parent", IsNullable = true, Length = 128, ColumnDescription = "上级部门编号 (Parent Department Number)")]
        public string ParentDepartmentNumber { get; set; }

        /// <summary>
        /// 上级部门名称 (Parent Department Name)
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string ParentDepartmentName { get; set; }
    }
}
