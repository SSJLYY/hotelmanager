using EOM.TSHotelManagement.Infrastructure;

namespace EOM.TSHotelManagement.Common
{
    public class SystemConstant : CodeConstantBase<SystemConstant>
    {
        public static readonly SystemConstant MariaDB = new SystemConstant("MariaDB", "Maria DB");
        public static readonly SystemConstant PgSql = new SystemConstant("PgSql", "Postgres SQL");
        public static readonly SystemConstant MySql = new SystemConstant("MySql", "My SQL");
        public static readonly SystemConstant SqlServer = new SystemConstant("SqlServer", "SQL Server");
        public static readonly SystemConstant Oracle = new SystemConstant("Oracle", "Oracle");
        public static readonly SystemConstant Sqlite = new SystemConstant("Sqlite", "SQLite");
        public static readonly SystemConstant DefaultDatabase = new SystemConstant("DefaultDatabase", "Default Database");
        public static readonly SystemConstant InitializeDatabase = new SystemConstant("InitializeDatabase", "Initialize Database");
        public static readonly SystemConstant Env = new SystemConstant("ASPNETCORE_ENVIRONMENT", "Asp.NET Core Environment");
        public static readonly SystemConstant Docker = new SystemConstant("docker", "Docker");


        public static readonly SystemConstant JobKeys = new SystemConstant("JobKeys", "Job Keys");

        public static readonly SystemConstant BranchName = new SystemConstant("TS酒店", "TS酒店");
        public static readonly SystemConstant BranchLogo = new SystemConstant("https://picrepo.oscode.top/i/2024/11/09/672f437f2e191.ico", "https://picrepo.oscode.top/i/2024/11/09/672f437f2e191.ico");

        protected SystemConstant(string code, string description) : base(code, description) { }
    }
}
