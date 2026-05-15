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
 *模块说明：员工奖惩类
 */
using SqlSugar;
using System;

namespace EOM.TSHotelManagement.Domain
{
    /// <summary>
    /// 员工奖惩 (Employee Rewards/Punishments)
    /// </summary>
    [SugarTable("reward_punishment", "员工奖惩 (Employee Rewards/Punishments)")]
    public class EmployeeRewardPunishment : SoftDeleteEntity
    {
        /// <summary>
        /// 编号 (ID)
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号 (ID)")]
        public int Id { get; set; }

        /// <summary>
        /// 奖惩编号 (Reward/Punishment ID)
        /// </summary>
        [SugarColumn(ColumnName = "reward_punishment_id", IsPrimaryKey = true, IsNullable = false, Length = 128, ColumnDescription = "奖惩编号 (Reward/Punishment ID)")]
        public string RewardPunishmentId { get; set; }
        /// <summary>
        /// 员工工号 (Employee ID)
        /// </summary>
        [SugarColumn(ColumnName = "employee_number", IsNullable = false, Length = 128, ColumnDescription = "员工工号 (Employee ID)")]
        public string EmployeeId { get; set; }

        /// <summary>
        /// 奖惩信息 (Reward/Punishment Information)
        /// </summary>
        [SugarColumn(ColumnName = "reward_punishment_information", IsNullable = false, Length = 256, ColumnDescription = "奖惩信息 (Reward/Punishment Information)")]
        public string RewardPunishmentInformation { get; set; }

        /// <summary>
        /// 奖惩类型 (Reward/Punishment Type)
        /// </summary>
        [SugarColumn(ColumnName = "reward_punishment_type", IsNullable = false, Length = 128, ColumnDescription = "奖惩类型 (Reward/Punishment Type)")]
        public string RewardPunishmentType { get; set; }

        /// <summary>
        /// 奖惩操作人 (Reward/Punishment Operator)
        /// </summary>
        [SugarColumn(ColumnName = "reward_punishment_operator", IsNullable = false, Length = 128, ColumnDescription = "奖惩操作人 (Reward/Punishment Operator)")]
        public string RewardPunishmentOperator { get; set; }

        /// <summary>
        /// 操作人姓名 (Operator Name)
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string OperatorName { get; set; }

        /// <summary>
        /// 奖惩时间 (Reward/Punishment Time)
        /// </summary>
        [SugarColumn(ColumnName = "reward_punishment_time", IsNullable = false, ColumnDescription = "奖惩时间 (Reward/Punishment Time)")]
        public DateOnly RewardPunishmentTime { get; set; }

        /// <summary>
        /// 类型名称 (Type Name)
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string RewardPunishmentTypeName { get; set; }
    }
}
