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
 *模块说明：操作日志类
 */
using SqlSugar;
using System;

namespace EOM.TSHotelManagement.Domain
{
    /// <summary>
    /// 操作日志表 (Operation Log)
    /// </summary>
    [SugarTable("operation_log", "操作日志表 (Operation Log)")]
    public class OperationLog : AuditEntity
    {
        /// <summary>
        /// 编号 (ID)
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号 (ID)")]
        public int Id { get; set; }

        /// <summary>
        /// 日志ID (Log ID)
        /// </summary>
        [SugarColumn(
            ColumnName = "operation_number",
            IsPrimaryKey = true,
            ColumnDescription = "日志ID (Log ID)",
            IsNullable = false,
            Length = 128
        )]
        public string OperationId { get; set; }

        /// <summary>
        /// 操作时间 (Operation Time)
        /// </summary>
        [SugarColumn(
            ColumnName = "operation_time",
            ColumnDescription = "操作时间 (Operation Time)",
            IsNullable = false
        )]
        public DateTime OperationTime { get; set; }

        /// <summary>
        /// 操作信息 (Log Content)
        /// </summary>
        [SugarColumn(
            ColumnName = "log_content",
            ColumnDescription = "操作信息 (Log Content)",
            IsNullable = false,
            Length = 2000
        )]
        public string LogContent { get; set; }

        /// <summary>
        /// 操作账号 (Operation Account)
        /// </summary>
        [SugarColumn(
            ColumnName = "operation_account",
            ColumnDescription = "操作账号 (Operation Account)",
            IsNullable = false,
            Length = 50
        )]
        public string OperationAccount { get; set; }

        /// <summary>
        /// 日志等级 (Log Level)
        /// </summary>
        [SugarColumn(
            ColumnName = "operation_level",
            ColumnDescription = "日志等级枚举值 (Log Level Enum)",
            IsNullable = false
        )]
        public int LogLevel { get; set; }

        /// <summary>
        /// 软件版本 (Software Version)
        /// </summary>
        [SugarColumn(
            ColumnName = "software_version",
            ColumnDescription = "软件版本号 (Software Version)",
            IsNullable = true,
            Length = 50
        )]
        public string SoftwareVersion { get; set; }

        /// <summary>
        /// 登录IP (Login IP Address)
        /// </summary>
        [SugarColumn(
            ColumnName = "login_ip",
            ColumnDescription = "登录IP地址 (Login IP Address)",
            IsNullable = false,
            Length = 45
        )]
        public string LoginIpAddress { get; set; }

        /// <summary>
        /// 日志等级名称 (Log Level Name)（不存储到数据库）
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string LogLevelName { get; set; }
    }
}
