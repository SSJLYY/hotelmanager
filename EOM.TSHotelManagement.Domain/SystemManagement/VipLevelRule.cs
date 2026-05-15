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
 *模块说明：会员等级规则类
 */
using SqlSugar;

namespace EOM.TSHotelManagement.Domain
{
    /// <summary>
    /// 会员等级规则表 (VIP Level Rules)
    /// </summary>
    [SugarTable("vip_rule", "会员等级规则配置表 (VIP Level Rule Configuration)")]
    public class VipLevelRule : SoftDeleteEntity
    {
        /// <summary>
        /// 编号 (ID)
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号 (ID)")]
        public int Id { get; set; }


        /// <summary>
        /// 会员规则流水号 (VIP Rule Serial Number)
        /// </summary>
        [SugarColumn(
            ColumnName = "rule_id",
            IsPrimaryKey = true,
            ColumnDescription = "规则业务唯一编号 (如VIPR-2023-001)",
            IsNullable = false,
            Length = 128
        )]
        public string RuleSerialNumber { get; set; }

        /// <summary>
        /// 会员规则名称 (VIP Rule Name)
        /// </summary>
        [SugarColumn(
            ColumnName = "rule_name",
            ColumnDescription = "规则名称（如『黄金会员准入规则』）",
            IsNullable = false,
            Length = 200
        )]
        public string RuleName { get; set; }

        /// <summary>
        /// 预设消费总额 (Preset Total Spending)
        /// </summary>
        [SugarColumn(
            ColumnName = "rule_value",
            Length = 18,
            ColumnDescription = "累计消费金额阈值（单位：元）",
            IsNullable = false,
            DecimalDigits = 2
        )]
        public decimal RuleValue { get; set; }

        /// <summary>
        /// 会员等级ID (VIP Level ID)
        /// </summary>
        [SugarColumn(
            ColumnName = "type_id",
            ColumnDescription = "关联会员等级表 (Linked VIP Level)",
            IsNullable = false
        )]
        public int VipLevelId { get; set; }

        /// <summary>
        /// 会员等级名称 (VIP Level Name)（不存储到数据库）
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string VipLevelName { get; set; }
    }
}
