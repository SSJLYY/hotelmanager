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
 *模块说明：监管统计类
 */
using SqlSugar;
using System;

namespace EOM.TSHotelManagement.Domain
{
    // <summary>
    /// 监管统计信息表 (Supervision Statistics)
    /// </summary>
    [SugarTable("supervision_statistics", "监管统计信息表 (Supervision Statistics)")]
    public class SupervisionStatistics : SoftDeleteEntity
    {
        /// <summary>
        /// 编号 (ID)
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号 (ID)")]
        public int Id { get; set; }

        /// <summary>
        /// 监管统计编号 (Supervision Statistics Number)
        /// </summary>
        [SugarColumn(
            ColumnName = "statistics_number",
            IsPrimaryKey = true,
            ColumnDescription = "监管记录唯一编号 (Unique Statistics ID)",
            IsNullable = false,
            Length = 128
        )]
        public string StatisticsNumber { get; set; }

        /// <summary>
        /// 监管部门编码 (Supervising Department Code)
        /// </summary>
        [SugarColumn(
            ColumnName = "supervising_department",
            ColumnDescription = "监管部门编码（关联部门表）",
            IsNullable = false,
            Length = 128
        )]
        public string SupervisingDepartment { get; set; }

        /// <summary>
        /// 监管部门名称 (Supervising Department Name)（不存储到数据库）
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string SupervisingDepartmentName { get; set; }

        /// <summary>
        /// 监管进度 (Supervision Progress)
        /// </summary>
        [SugarColumn(
            ColumnName = "supervision_progress",
            ColumnDescription = "监管进度描述（如：已完成/整改中）",
            IsNullable = false,
            Length = 200
        )]
        public string SupervisionProgress { get; set; }

        /// <summary>
        /// 监管损失金额 (Supervision Loss Amount)
        /// </summary>
        [SugarColumn(
            ColumnName = "supervision_loss",
            ColumnDescription = "监管造成的经济损失",
            IsNullable = false
        )]
        public string SupervisionLoss { get; set; }

        /// <summary>
        /// 监管评分 (Supervision Score)
        /// </summary>
        [SugarColumn(
            ColumnName = "supervision_score",
            ColumnDescription = "监管评分（范围：0-100分）",
            IsNullable = false,
            DefaultValue = "100"
        )]
        public int SupervisionScore { get; set; }

        /// <summary>
        /// 统计人员 (Supervision Statistician)
        /// </summary>
        [SugarColumn(
            ColumnName = "supervision_statistician",
            ColumnDescription = "统计责任人姓名",
            IsNullable = false,
            Length = 50
        )]
        public string SupervisionStatistician { get; set; }

        /// <summary>
        /// 监管建议 (Supervision Advice)
        /// </summary>
        [SugarColumn(
            ColumnName = "supervision_advice",
            ColumnDescription = "监管整改建议内容",
            IsNullable = true,
            Length = 1000
        )]
        public string SupervisionAdvice { get; set; }

        /// <summary>
        /// 监管时间 (Supervision Time)
        /// </summary>
        [SugarColumn(
            ColumnName = "supervision_time",
            ColumnDescription = "监管检查时间",
            IsNullable = false
        )]
        public DateTime SupervisionTime { get; set; }

        /// <summary>
        /// 整改状态 (Rectification Status)
        /// </summary>
        [SugarColumn(
            ColumnName = "rectification_status",
            ColumnDescription = "0-未整改/1-已整改",
            DefaultValue = "0"
        )]
        public int RectificationStatus { get; set; }
    }
}
