using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Domain;
using EOM.TSHotelManagement.Migration;
using jvncorelib.EntityLib;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using SqlSugar;

namespace EOM.TSHotelManagement.Data
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly ISqlSugarClient _client;
        private readonly ISqlSugarClientConnector _connector;
        private readonly IConfiguration _configuration;
        private readonly string _initialAdminEncryptedPassword;
        private readonly string _initialEmployeeEncryptedPassword;
        private readonly IDataProtector _adminPasswordProtector;
        private readonly IDataProtector _employeePasswordProtector;
        private const string AdminProtectorPurpose = "AdminInfoProtector";
        private const string EmployeeProtectorPurpose = "EmployeeInfoProtector";
        private const string DataProtectionPayloadPrefix = "CfDJ8";
        private const string DefaultAdminAccount = "admin";
        private const string DefaultEmployeeId = "WK010";
        private const string DefaultAdminPassword = "admin";
        private const string DefaultEmployeePassword = "WK010";

        public DatabaseInitializer(
            ISqlSugarClient client,
            ISqlSugarClientConnector connector,
            IConfiguration configuration,
            IDataProtectionProvider dataProtectionProvider)
        {
            _client = client;
            _connector = connector;
            _configuration = configuration;
            _adminPasswordProtector = dataProtectionProvider.CreateProtector(AdminProtectorPurpose);
            _employeePasswordProtector = dataProtectionProvider.CreateProtector(EmployeeProtectorPurpose);
            _initialAdminEncryptedPassword = _adminPasswordProtector.Protect(DefaultAdminPassword);
            _initialEmployeeEncryptedPassword = _employeePasswordProtector.Protect(DefaultEmployeePassword);
        }

        #region initlize database
        public void InitializeDatabase()
        {
            var config = _configuration;
            var factory = _connector;

            try
            {
                var dbName = GetDatabaseName(config);

                Console.WriteLine($"Database Name: {dbName}");

                var dbSettings = GetDatabaseSettings(config, dbName);

                using (var masterDb = CreateMasterConnection(config, dbName, dbSettings.DbType))
                {
                    if (dbSettings.DbType == DbType.PostgreSQL)
                    {
                        var dbExists = masterDb.Ado.GetInt($"SELECT 1 FROM pg_database WHERE datname = '{dbSettings.Database}'") > 0;
                        if (!dbExists)
                        {
                            Console.WriteLine($"Creating database {dbSettings.Database}...");
                            masterDb.Ado.ExecuteCommand($"CREATE DATABASE \"{dbSettings.Database}\"");
                            Console.WriteLine("Database created successfully");
                        }
                    }
                    else
                    {
                        if (!masterDb.DbMaintenance.GetDataBaseList().Contains(dbSettings.Database))
                        {
                            Console.WriteLine($"Creating database {dbSettings.Database}...");
                            masterDb.DbMaintenance.CreateDatabase(dbSettings.Database, null);
                            Console.WriteLine("Database created successfully");
                        }
                    }
                }

                using (var db = factory.CreateClient(dbName))
                {
                    Console.WriteLine("Initializing database schema...");

                    var entityBuilder = new EntityBuilder(_initialAdminEncryptedPassword, _initialEmployeeEncryptedPassword);

                    var dbTables = db.DbMaintenance.GetTableInfoList()
                                    .Select(a => a.Name.Trim().ToLower())
                                    .ToList();

                    var initializeTables = entityBuilder.EntityTypes
                                    .Select(a => a.Name.Trim().ToLower())
                                    .ToList();

                    var needCreateTable = initializeTables
                                    .Except(dbTables, StringComparer.OrdinalIgnoreCase)
                                    .ToList();

                    var needCreateTableTypes = needCreateTable
                       .Select(tableName => entityBuilder.EntityTypes.SingleOrDefault(type => type.Name.Trim().Equals(tableName, StringComparison.OrdinalIgnoreCase)))
                       .Where(type => type != null)
                       .ToArray();

                    db.CodeFirst.InitTables(needCreateTableTypes);
                    
                    Console.WriteLine("Database schema initialized");

                    SeedInitialData(db);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization failed:{ex.Message}");
                throw;
            }
        }

        private (string Database, DbType DbType) GetDatabaseSettings(IConfiguration config, string dbName)
        {
            var dbType = SqlSugarClientConnector.GetDbType(dbName);
            var connectionString = GetConnectionString(config, dbName);

            switch (dbType)
            {
                case DbType.MySql:
                case DbType.MySqlConnector:
                    var builder = new MySqlConnectionStringBuilder(connectionString);
                    return (builder.Database, dbType);
                //case DbType.SqlServer: //This project not include reference Package.Please manual install Microsoft.EntityFrameworkCore.SqlServer
                //    var sqlBuilder = new SqlConnectionStringBuilder(connectionString);
                //    return (sqlBuilder.InitialCatalog, dbType);
                //case DbType.Oracle: //This project not include reference Package.Please manual install Oracle.ManagedDataAccess.Core
                //    var oracleBuilder = new OracleConnectionStringBuilder(connectionString);
                //    return (oracleBuilder.DataSource, dbType);
                //case DbType.PostgreSQL: //This project not include reference Package.Please manual install Npgsql.EntityFrameworkCore.PostgreSQL
                //    var pgsqlBuilder = new NpgsqlConnectionStringBuilder(connectionString);
                //    return (pgsqlBuilder.Database, dbType);
                default:
                    throw new NotSupportedException($"Unsupported DbType: {dbType}");
            }
        }
        
        public string GetDatabaseName(IConfiguration config)
        {
            var envCode = Environment.GetEnvironmentVariable(SystemConstant.Env.Code);
            var isDocker = !string.IsNullOrEmpty(envCode) && envCode.ToLower() == SystemConstant.Docker.Code;
            string dbName;

            if (isDocker)
            {
                dbName = Environment.GetEnvironmentVariable(SystemConstant.DefaultDatabase.Code);
                if (string.IsNullOrEmpty(dbName))
                {
                    dbName = config[SystemConstant.DefaultDatabase.Code];
                    if (string.IsNullOrEmpty(dbName))
                    {
                        dbName = SystemConstant.MariaDB.Code;
                    }
                }
            }
            else
            {
                dbName = config[SystemConstant.DefaultDatabase.Code] ?? SystemConstant.MariaDB.Code;
            }

            var supportedDbs = new[] { SystemConstant.MariaDB.Code, SystemConstant.MySql.Code, SystemConstant.PgSql.Code, SystemConstant.SqlServer.Code, SystemConstant.Oracle.Code, SystemConstant.Sqlite.Code };
            if (!supportedDbs.Contains(dbName))
            {
                dbName = SystemConstant.MariaDB.Code;
            }

            return dbName;
        }

        private SqlSugarClient CreateMasterConnection(IConfiguration config, string dbName, DbType dbType)
        {
            var connectionString = GetConnectionString(config, dbName);

            dynamic builder = null;

            switch (dbType)
            {
                case DbType.MySql:
                case DbType.MySqlConnector:
                    builder = new MySqlConnectionStringBuilder(connectionString)
                    {
                        Database = null,
                    };
                    break;
                    //case DbType.SqlServer: //This project not include reference Package.Please manual install Microsoft.EntityFrameworkCore.SqlServer
                    //    builder = new SqlConnectionStringBuilder(connectionString)
                    //    {
                    //        InitialCatalog = "master",
                    //        ConnectTimeout = 30
                    //    };
                    //    break;
                    //case DbType.Oracle: //This project not include reference Package.Please manual install Oracle.ManagedDataAccess.Core
                    //    builder = new OracleConnectionStringBuilder(connectionString)
                    //    {
                    //        UserID = "sys as sysdba",
                    //    };
                    //    break;
                    //case DbType.PostgreSQL: //This project not include reference Package.Please manual install Npgsql.EntityFrameworkCore.PostgreSQL
                    //    builder = new NpgsqlConnectionStringBuilder(connectionString)
                    //    {
                    //        Database = "postgres",
                    //        Timeout = 30,
                    //        Pooling = false
                    //    };
                    //    break;
            }

            return new SqlSugarClient(new ConnectionConfig
            {
                ConnectionString = builder?.ConnectionString ?? string.Empty,
                DbType = dbType,
                IsAutoCloseConnection = true,
                ConfigureExternalServices = new ConfigureExternalServices
                {
                    EntityService = (c, p) =>
                    {
                        if (dbType == DbType.PostgreSQL && p.IsPrimarykey && p.IsIdentity)
                            p.IsIdentity = true;
                        p.OracleSequenceName = $"seq_{c.Name.ToLower()}";
                        if (dbType == DbType.Oracle && p.IsPrimarykey && p.IsIdentity)
                            p.OracleSequenceName = "SEQ_" + c.Name;
                    }
                }
            });
        }

        private string GetConnectionString(IConfiguration config, string dbName)
        {
            var env = Environment.GetEnvironmentVariable(SystemConstant.Env.Code);
            Console.WriteLine($"ASPNETCORE_ENVIRONMENT: {env}");

            if (!string.IsNullOrEmpty(env) && env.ToLower() == "docker")
            {
                var envVarName = $"{dbName}ConnectStr";
                var connectionString = Environment.GetEnvironmentVariable(envVarName);

                if (!string.IsNullOrEmpty(connectionString))
                {
                    var cleanString = connectionString.Trim('"');
                    return cleanString;
                }

                throw new ArgumentException($"Docker 环境变量 {envVarName} 未找到或为空");
            }

            var configString = config.GetConnectionString($"{dbName}ConnectStr");

            if (string.IsNullOrEmpty(configString))
            {
                throw new ArgumentException($"配置文件中未找到连接字符串：{dbName}ConnectStr");
            }

            return configString;
        }

        private void SeedInitialData(ISqlSugarClient db)
        {
            Console.WriteLine("Initializing database data...");

            try
            {
                EnsureDefaultAccountPasswordsEncrypted(db);

                var entityBuilder = new EntityBuilder(_initialAdminEncryptedPassword, _initialEmployeeEncryptedPassword);
                var entitiesToAdd = new List<object>();

                var sortedEntities = entityBuilder.GetEntityDatas()
                    .OrderBy(entity => entity switch
                    {
                        AdministratorType _ => 1,
                        Department _ => 2,
                        Position _ => 3,
                        Nation _ => 4,
                        Education _ => 5,
                        PassportType _ => 6,
                        Menu _ => 7,
                        NavBar _ => 8,
                        PromotionContent _ => 9,
                        Administrator _ => 10,
                        Employee _ => 11,
                        UserFavoriteCollection _ => 12,
                        _ => 99
                    })
                    .ToList();

                Console.WriteLine($"Total entities to process: {sortedEntities.Count}");

                foreach (var entityData in sortedEntities)
                {
                    Console.WriteLine($"Processing entity: {entityData.GetType().Name}");

                    try
                    {
                        bool alreadyExists = false;

                        switch (entityData)
                        {
                            case Menu menu:
                                alreadyExists = db.Queryable<Menu>().Any(a => a.Key == menu.Key);
                                break;

                            case Administrator admin:
                                alreadyExists = db.Queryable<Administrator>().Any(a => a.Account == admin.Account);
                                break;

                            case AdministratorType adminType:
                                alreadyExists = db.Queryable<AdministratorType>().Any(a => a.TypeName == adminType.TypeName);
                                break;

                            case NavBar navBar:
                                alreadyExists = db.Queryable<NavBar>().Any(a => a.NavigationBarName == navBar.NavigationBarName);
                                break;

                            case PromotionContent promoContent:
                                alreadyExists = db.Queryable<PromotionContent>().Any(a => a.PromotionContentMessage == promoContent.PromotionContentMessage);
                                break;

                            case Nation nation:
                                alreadyExists = db.Queryable<Nation>().Any(a => a.NationName == nation.NationName);
                                break;

                            case Department department:
                                alreadyExists = db.Queryable<Department>().Any(a => a.DepartmentNumber == department.DepartmentNumber);
                                break;

                            case Position position:
                                alreadyExists = db.Queryable<Position>().Any(a => a.PositionName == position.PositionName);
                                break;

                            case Education education:
                                alreadyExists = db.Queryable<Education>().Any(a => a.EducationName == education.EducationName);
                                break;

                            case PassportType passportType:
                                alreadyExists = db.Queryable<PassportType>().Any(a => a.PassportName == passportType.PassportName);
                                break;

                            case Employee employee:
                                alreadyExists = db.Queryable<Employee>().Any(a => a.EmployeeId == employee.EmployeeId);
                                break;

                            case UserFavoriteCollection favoriteCollection:
                                alreadyExists = db.Queryable<UserFavoriteCollection>().Any(a => a.UserNumber == favoriteCollection.UserNumber);
                                break;

                            case Permission permission:
                                alreadyExists = db.Queryable<Permission>().Any(a => a.PermissionNumber == permission.PermissionNumber);
                                break;
                        }

                        if (alreadyExists)
                        {
                            Console.WriteLine($"{entityData.GetType().Name} already exists, skipping.");
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error checking existence for {entityData.GetType().Name}: {ex.Message}");
                        throw;
                    }

                    entitiesToAdd.Add(entityData);
                }

                if (entitiesToAdd.Count > 0)
                {
                    Console.WriteLine($"Adding {entitiesToAdd.Count} initial data records");

                    var groupedEntities = entitiesToAdd
                        .GroupBy(e => e.GetType())
                        .ToList();

                    foreach (var group in groupedEntities)
                    {
                        Console.WriteLine($"Inserting {group.Count()} records of type {group.Key.Name}");

                        var list = group.ToList();

                        db.InsertableByObject(list).ExecuteCommand();
                    }

                    Console.WriteLine("Data inserted successfully");

                    // ===== Ensure admin owns ALL permissions via built-in role seeding =====
                    try
                    {
                        const string adminAccount = "admin";
                        const string adminRoleNumber = "R-ADMIN";
                        const string adminRoleName = "超级管理员";

                        // 1) Ensure built-in admin role exists
                        var adminRoleExists = db.Queryable<Role>()
                            .Any(r => r.RoleNumber == adminRoleNumber && r.IsDelete != 1);

                        if (!adminRoleExists)
                        {
                            db.Insertable(new Role
                            {
                                RoleNumber = adminRoleNumber,
                                RoleName = adminRoleName,
                                RoleDescription = "初始化内置角色：拥有系统全部权限",
                                IsDelete = 0,
                                DataInsUsr = "System",
                                DataInsDate = DateTime.Now
                            }).ExecuteCommand();
                            Console.WriteLine($"Seeded role: {adminRoleNumber} - {adminRoleName}");
                        }

                        // 1.1) Ensure default employee and customer groups exist
                        const string employeeRoleNumber = "R-EMPLOYEE";
                        const string employeeRoleName = "员工组";
                        const string customerRoleNumber = "R-CUSTOMER";
                        const string customerRoleName = "客户组";

                        var employeeRoleExists = db.Queryable<Role>()
                            .Any(r => r.RoleNumber == employeeRoleNumber && r.IsDelete != 1);
                        if (!employeeRoleExists)
                        {
                            db.Insertable(new Role
                            {
                                RoleNumber = employeeRoleNumber,
                                RoleName = employeeRoleName,
                                RoleDescription = "默认员工组：用于统一分配员工权限",
                                IsDelete = 0,
                                DataInsUsr = "System",
                                DataInsDate = DateTime.Now
                            }).ExecuteCommand();
                            Console.WriteLine($"Seeded role: {employeeRoleNumber} - {employeeRoleName}");
                        }

                        var customerRoleExists = db.Queryable<Role>()
                            .Any(r => r.RoleNumber == customerRoleNumber && r.IsDelete != 1);
                        if (!customerRoleExists)
                        {
                            db.Insertable(new Role
                            {
                                RoleNumber = customerRoleNumber,
                                RoleName = customerRoleName,
                                RoleDescription = "默认客户组：用于统一分配客户权限",
                                IsDelete = 0,
                                DataInsUsr = "System",
                                DataInsDate = DateTime.Now
                            }).ExecuteCommand();
                            Console.WriteLine($"Seeded role: {customerRoleNumber} - {customerRoleName}");
                        }

                        // 2) Bind admin user to admin role
                        var adminUser = db.Queryable<Administrator>()
                            .Where(a => a.Account == adminAccount && a.IsDelete != 1)
                            .First();

                        if (adminUser != null)
                        {
                            var hasBind = db.Queryable<UserRole>()
                                .Any(ur => ur.UserNumber == adminUser.Number && ur.RoleNumber == adminRoleNumber);

                            if (!hasBind)
                            {
                                db.Insertable(new UserRole
                                {
                                    UserNumber = adminUser.Number,
                                    RoleNumber = adminRoleNumber,
                                    DataInsUsr = "System",
                                    DataInsDate = DateTime.Now
                                }).ExecuteCommand();
                                Console.WriteLine($"Bound admin user({adminUser.Number}) to role {adminRoleNumber}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Admin account not found while binding to role, skip binding.");
                        }

                        // 3) Grant ALL permissions to admin role
                        var allPermNumbers = db.Queryable<Permission>()
                            .Select(p => p.PermissionNumber)
                            .ToList();

                        var existingRolePerms = db.Queryable<RolePermission>()
                            .Where(rp => rp.RoleNumber == adminRoleNumber
                                      && rp.PermissionNumber != null
                                      && rp.PermissionNumber != "")
                            .Select(rp => rp.PermissionNumber)
                            .ToList();

                        var toGrant = allPermNumbers
                            .Where(p => !string.IsNullOrEmpty(p))
                            .Except(existingRolePerms, StringComparer.OrdinalIgnoreCase)
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .ToList();

                        if (toGrant.Count > 0)
                        {
                            var inserts = toGrant.Select(p => new RolePermission
                            {
                                RoleNumber = adminRoleNumber,
                                PermissionNumber = p!,
                                DataInsUsr = "System",
                                DataInsDate = DateTime.Now
                            }).ToList();

                            db.Insertable(inserts).ExecuteCommand();
                            Console.WriteLine($"Granted {toGrant.Count} permissions to role {adminRoleNumber}");
                        }
                        else
                        {
                            Console.WriteLine("Admin role already has all permissions.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Seeding admin ALL-permissions mapping failed: {ex.Message}");
                    }
                    // ===== END admin ALL permissions seeding =====
                }
                else
                {
                    Console.WriteLine("No initial data to add");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database data initialization failed: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner StackTrace: {ex.InnerException.StackTrace}");
                }

                throw;
            }
            finally
            {
                Console.WriteLine("Database data initialization completed");
                Console.WriteLine($"administrator account：admin");
                Console.WriteLine($"administrator password：admin");
            }
        }

        private void EnsureDefaultAccountPasswordsEncrypted(ISqlSugarClient db)
        {
            try
            {
                var admin = db.Queryable<Administrator>()
                    .First(a => a.Account == DefaultAdminAccount && a.IsDelete != 1);
                if (admin != null && !IsProtectedValue(admin.Password, _adminPasswordProtector))
                {
                    var source = string.IsNullOrWhiteSpace(admin.Password) ? DefaultAdminPassword : admin.Password;
                    admin.Password = _adminPasswordProtector.Protect(source);
                    admin.DataChgUsr = "System";
                    admin.DataChgDate = DateTime.Now;
                    db.Updateable(admin)
                        .UpdateColumns(a => new { a.Password, a.DataChgUsr, a.DataChgDate })
                        .ExecuteCommand();
                    Console.WriteLine("Auto-fixed admin password encryption during initialization.");
                }

                var employee = db.Queryable<Employee>()
                    .First(a => a.EmployeeId == DefaultEmployeeId && a.IsDelete != 1);
                if (employee != null && !IsProtectedValue(employee.Password, _employeePasswordProtector))
                {
                    var source = string.IsNullOrWhiteSpace(employee.Password) ? DefaultEmployeePassword : employee.Password;
                    employee.Password = _employeePasswordProtector.Protect(source);
                    employee.DataChgUsr = "System";
                    employee.DataChgDate = DateTime.Now;
                    db.Updateable(employee)
                        .UpdateColumns(a => new { a.Password, a.DataChgUsr, a.DataChgDate })
                        .ExecuteCommand();
                    Console.WriteLine("Auto-fixed employee password encryption during initialization.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ensure default account password encryption skipped: {ex.Message}");
            }
        }

        private static bool IsProtectedValue(string? value, IDataProtector protector)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            _ = protector;
            return value.StartsWith(DataProtectionPayloadPrefix, StringComparison.Ordinal);
        }
        #endregion
    }
}
