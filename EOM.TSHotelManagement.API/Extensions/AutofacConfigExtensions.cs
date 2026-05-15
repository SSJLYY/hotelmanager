using Autofac;
using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Data;
using SqlSugar;
using System;
using System.IO;
using System.Reflection;

namespace EOM.TSHotelManagement.WebApi
{
    public static class AutofacConfigExtensions
    {
        public static void ConfigureAutofacContainer(this ContainerBuilder builder)
        {
            #region AutoFac IOC容器,实现批量依赖注入的容器
            try
            {
                builder.Register(c =>
                {
                    var factory = c.Resolve<ISqlSugarClientConnector>();
                    return factory.CreateClient();
                }).As<ISqlSugarClient>().InstancePerLifetimeScope();

                builder.RegisterType<DatabaseInitializer>().As<IDatabaseInitializer>().InstancePerLifetimeScope();

                builder.RegisterType<SqlSugarClientConnector>().As<ISqlSugarClientConnector>().SingleInstance();

                builder.RegisterType<RequestLoggingMiddleware>()
                       .InstancePerDependency();
                builder.RegisterType<IdempotencyKeyMiddleware>()
                       .InstancePerDependency();

                builder.RegisterType<RedisHelper>().AsSelf().SingleInstance();
                builder.RegisterType<JwtTokenRevocationService>().AsSelf().SingleInstance();
                builder.RegisterType<JWTHelper>().AsSelf().InstancePerLifetimeScope();
                builder.RegisterType<MailHelper>().AsSelf().InstancePerLifetimeScope();
                builder.RegisterType<LskyHelper>().AsSelf().InstancePerLifetimeScope();
                builder.RegisterType<DataProtectionHelper>().AsSelf().InstancePerLifetimeScope();
                builder.RegisterType<TwoFactorHelper>().AsSelf().InstancePerLifetimeScope();

                builder.RegisterGeneric(typeof(GenericRepository<>)).AsSelf().InstancePerLifetimeScope();

                // 程序集批量反射注入
                var assemblyService = Assembly.LoadFrom(Path.Combine(AppContext.BaseDirectory, "EOM.TSHotelManagement.Service.dll"));
                builder.RegisterAssemblyTypes(assemblyService)
                    .AsImplementedInterfaces()
                    .InstancePerDependency()
                    .PropertiesAutowired();

                // 注入加解密组件
                var encryptionService = Assembly.LoadFrom(Path.Combine(AppContext.BaseDirectory, "jvncorelib.dll"));
                builder.RegisterAssemblyTypes(encryptionService)
                    .AsImplementedInterfaces()
                    .PropertiesAutowired();
            }
            catch (Exception ex)
            {
                throw new Exception($"Autofac configuration failed: {ex.Message}", ex);
            }
            #endregion
        }
    }
}
