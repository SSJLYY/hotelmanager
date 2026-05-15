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
 *模块说明：菜单表
 */
using SqlSugar;
namespace EOM.TSHotelManagement.Domain
{
    /// <summary>
    /// 菜单表 (Menu Table)
    /// </summary>
    [SugarTable("menu", "菜单表 (Menu Table)", true)]
    public class Menu : SoftDeleteEntity
    {
        /// <summary>
        /// 编号 (ID)
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号 (ID)")]
        public int Id { get; set; }

        /// <summary>
        /// 备  注:菜单键 (Menu Key)
        /// 默认值:
        /// </summary>
        [SugarColumn(ColumnName = "key", IsNullable = true, ColumnDescription = "菜单键 (Menu Key)", Length = 256)]
        public string? Key { get; set; }

        /// <summary>
        /// 备  注:菜单标题 (Menu Title)
        /// 默认值:
        /// </summary>
        [SugarColumn(ColumnName = "title", IsNullable = false, ColumnDescription = "菜单标题 (Menu Title)", Length = 256)]
        public string Title { get; set; } = null!;

        /// <summary>
        /// 备  注:菜单路径 (Menu Path)
        /// 默认值:
        /// </summary>
        [SugarColumn(ColumnName = "path", IsNullable = false, ColumnDescription = "菜单路径 (Menu Path)", ColumnDataType = "text")]
        public string? Path { get; set; }

        /// <summary>
        /// 备  注:父级ID (Parent ID)
        /// 默认值:
        /// </summary>
        [SugarColumn(ColumnName = "parent", IsNullable = true, ColumnDescription = "父级ID (Parent ID)")]
        public int? Parent { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [SugarColumn(ColumnName = "icon", IsNullable = true, ColumnDescription = "图标", Length = 256)]
        public string Icon { get; set; }
    }

}
