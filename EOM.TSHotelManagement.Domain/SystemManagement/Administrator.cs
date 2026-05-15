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
 *模块说明：管理员实体类
 */

using SqlSugar;

namespace EOM.TSHotelManagement.Domain
{
    /// <summary>
    /// 管理员实体类 (Administrator Entity)
    /// </summary>
    [SugarTable("administrator", "管理员实体类 (Administrator Entity)", true)]
    public class Administrator : Personnel //SoftDeleteEntity
    {
        /// <summary>
        /// 编号 (ID)
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号 (ID)")]
        public int Id { get; set; }

        /// <summary>
        /// 管理员账号 (Administrator Account)
        /// </summary>
        [SugarColumn(ColumnName = "admin_number", IsPrimaryKey = true, IsNullable = false, Length = 128, ColumnDescription = "管理员账号 (Administrator Account)")]

        public string Number { get; set; }

        /// <summary>
        /// 管理员账号 (Administrator Account)
        /// </summary>
        [SugarColumn(ColumnName = "admin_account", IsNullable = false, Length = 128, ColumnDescription = "管理员名称 (Administrator Name)")]

        public string Account { get; set; }

        /// <summary>
        /// 管理员密码 (Administrator Password)
        /// </summary>
        [SugarColumn(ColumnName = "admin_password", IsNullable = false, Length = 256, ColumnDescription = "管理员密码 (Administrator Password)")]

        public string Password { get; set; }

        /// <summary>
        /// 管理员类型 (Administrator Type)
        /// </summary>
        [SugarColumn(ColumnName = "admin_type", IsNullable = false, Length = 150, ColumnDescription = "管理员类型 (Administrator Type)")]

        public string Type { get; set; }

        /// <summary>
        /// 是否为超级管理员 (Is Super Administrator)
        /// </summary>
        [SugarColumn(ColumnName = "is_admin", IsNullable = false, ColumnDescription = "是否为超级管理员 (Is Super Administrator)")]
        public int IsSuperAdmin { get; set; }
    }
}
