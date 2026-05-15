using EOM.TSHotelManagement.Common;
using Microsoft.Extensions.Configuration;
using SqlSugar;

namespace EOM.TSHotelManagement.Data
{
    public class SqlSugarClientConnector : ISqlSugarClientConnector
    {
        private readonly IConfiguration _configuration;

        public SqlSugarClientConnector(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ISqlSugarClient CreateClient(string dbName = null)
        {
            Console.WriteLine("=== SqlSugarClientConnector.CreateClient 开始 ===");

            // 1. 获取数据库名称
            dbName = GetDatabaseName(dbName);
            Console.WriteLine($"使用的数据库名称: {dbName}");

            // 2. 获取连接字符串
            string connectionString = GetConnectionString(dbName);
            Console.WriteLine($"获取的连接字符串: {connectionString ?? "null"}");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("连接字符串不能为空");
            }

            // 3. 获取数据库类型
            var dbType = GetDbType(dbName);
            Console.WriteLine($"数据库类型: {dbType}");

            // 4. 创建配置
            ConnectionConfig config = new()
            {
                ConnectionString = connectionString,
                DbType = dbType,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
                LanguageType = LanguageType.Chinese
            };

            // 5. 数据库特定配置
            ConfigureDatabaseSpecificSettings(dbType, config);

            Console.WriteLine("=== SqlSugarClientConnector.CreateClient 完成 ===");
            return new SqlSugarClient(config);
        }

        private string GetDatabaseName(string dbName)
        {
            if (!string.IsNullOrEmpty(dbName))
            {
                return dbName;
            }

            // 优先从环境变量获取
            var envValue = Environment.GetEnvironmentVariable(SystemConstant.Env.Code);
            
            if (!string.IsNullOrEmpty(envValue) && envValue.ToLower() == SystemConstant.Docker.Code)
            {
                var envDbName = Environment.GetEnvironmentVariable(SystemConstant.DefaultDatabase.Code);

                if (!string.IsNullOrEmpty(envDbName))
                {
                    return envDbName;
                }
            }

            // 从配置文件获取
            var configDbName = _configuration[SystemConstant.DefaultDatabase.Code];

            return configDbName ?? SystemConstant.MariaDB.Code;
        }

        private string GetConnectionString(string dbName)
        {
            var envValue = Environment.GetEnvironmentVariable(SystemConstant.Env.Code);

            if (!string.IsNullOrEmpty(envValue) && envValue.ToLower() == "docker")
            {
                var envVarName = $"{dbName}ConnectStr";
                var envConnectionString = Environment.GetEnvironmentVariable(envVarName);

                if (!string.IsNullOrEmpty(envConnectionString))
                {
                    return envConnectionString.Trim('"');
                }

                throw new ArgumentException($"Docker环境下未找到环境变量: {envVarName}");
            }

            var configConnectionString = _configuration.GetConnectionString($"{dbName}ConnectStr");

            return configConnectionString;
        }

        private void ConfigureDatabaseSpecificSettings(DbType dbType, ConnectionConfig config)
        {
            switch (dbType)
            {
                case DbType.PostgreSQL:
                    config.MoreSettings = new ConnMoreSettings
                    {
                        PgSqlIsAutoToLower = true,
                        PgSqlIsAutoToLowerCodeFirst = true,
                        DisableMillisecond = true,
                    };
                    break;
                case DbType.SqlServer:
                    config.MoreSettings = new ConnMoreSettings()
                    {
                        SqlServerCodeFirstNvarchar = true,
                        IsWithNoLockQuery = true
                    };
                    break;
            }
        }

        public static DbType GetDbType(string dbName)
        {
            return dbName switch
            {
                var name when name == SystemConstant.PgSql.Code => DbType.PostgreSQL,
                var name when name == SystemConstant.MySql.Code => DbType.MySql,
                var name when name == SystemConstant.Oracle.Code => DbType.Oracle,
                var name when name == SystemConstant.SqlServer.Code => DbType.SqlServer,
                var name when name == SystemConstant.MariaDB.Code => DbType.MySqlConnector,
                _ => throw new ArgumentException("Unsupported database", nameof(dbName))
            };
        }
    }
}
