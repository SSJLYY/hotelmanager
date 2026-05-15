using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EOM.TSHotelManagement.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // 获取配置目录（支持 Docker 挂载）
            var configDir = Environment.GetEnvironmentVariable("ASPNETCORE_CONFIG_DIRECTORY")
                ?? "/app/config";
            
            // 加载配置：环境变量优先级最高，最后添加的 provider 优先级最高
            builder.Configuration
                // 先加载嵌入的配置文件
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Application.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Database.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Services.json", optional: true, reloadOnChange: true)
                // 再加载外部挂载的配置文件（覆盖嵌入的配置）
                .AddJsonFile($"{configDir}/appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{configDir}/appsettings.Application.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{configDir}/appsettings.Database.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{configDir}/appsettings.Services.json", optional: true, reloadOnChange: true)
                // 最后加载环境变量（优先级最高）
                .AddEnvironmentVariables(prefix: "Jwt__")
                .AddEnvironmentVariables(prefix: "Mail__")
                .AddEnvironmentVariables(prefix: "Redis__")
                .AddEnvironmentVariables(prefix: "Lsky__")
                .AddEnvironmentVariables(prefix: "TwoFactor__")
                .AddEnvironmentVariables(prefix: "AllowedOrigins__")
                .AddEnvironmentVariables(prefix: "JobKeys__")
                .AddEnvironmentVariables(prefix: "ExpirationSettings__")
                .AddEnvironmentVariables(prefix: "Idempotency__");
            var configuration = builder.Configuration;

            // Autofac 容器配置
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(AutofacConfigExtensions.ConfigureAutofacContainer);

            // 服务配置
            builder.Services.ConfigureDataProtection(configuration);
            builder.Services.ConfigureQuartz(configuration);
            builder.Services.ConfigureAuthentication(configuration);
            builder.Services.RegisterSingletonServices(configuration);
            builder.Services.ConfigureControllers();
            builder.Services.ConfigureSwagger();
            builder.Services.ConfigureCors(configuration);
            builder.Services.AddHttpContextAccessor();
            builder.Services.ConfigureXForward();

            // 构建应用
            var app = builder.Build();

            // 应用配置
            app.ConfigureEnvironment();
            app.ConfigureMiddlewares();

            app.InitializeDatabase();

            app.SyncPermissionsFromAttributes();
            app.ConfigureEndpoints();
            app.ConfigureSwaggerUI();

            app.Run();
        }
    }
}