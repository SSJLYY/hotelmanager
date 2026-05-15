
using System;
using System.Collections.Generic;

namespace EOM.TSHotelManagement.Common
{
    /// <summary>
    /// 统一的日期范围DTO，用于时间范围查询。
    /// 支持单个日期范围（Start/End）或多个日期范围（Ranges）。
    /// </summary>
    public class DateRangeDto
    {
        /// <summary>
        /// 查询开始时间（用于单个日期字段）
        /// </summary>
        public DateTime? Start { get; set; }

        /// <summary>
        /// 查询结束时间（用于单个日期字段）
        /// </summary>
        public DateTime? End { get; set; }

        /// <summary>
        /// 多个日期范围查询（键为实体字段名，值为日期范围）
        /// </summary>
        public Dictionary<string, DateRange> Ranges { get; set; } = new Dictionary<string, DateRange>();
    }

    /// <summary>
    /// 单个日期范围
    /// </summary>
    public class DateRange
    {
        /// <summary>
        /// 查询开始时间
        /// </summary>
        public DateTime? Start { get; set; }

        /// <summary>
        /// 查询结束时间
        /// </summary>
        public DateTime? End { get; set; }
    }
}