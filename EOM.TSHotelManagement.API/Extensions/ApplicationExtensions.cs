using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace EOM.TSHotelManagement.WebApi
{
    public static class ApplicationExtensions
    {
        public static void ConfigureEnvironment(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
        }

        public static void ConfigureMiddlewares(this WebApplication app)
        {
            app.UseForwardedHeaders();
            app.UseRouting();
            app.UseCors("MyCorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAntiforgery();
            app.UseRequestLogging();
            app.UseIdempotencyKey();
        }

        /// <summary>
        /// 初始化数据库
        /// </summary>
        /// <param name="app"></param>
        /// <exception cref="Exception"></exception>
        public static void InitializeDatabase(this WebApplication app)
        {
            try
            {
                var enableInitializeDatabase = false;
                if (Environment.GetEnvironmentVariable(SystemConstant.Env.Code) == SystemConstant.Docker.Code)
                {
                    enableInitializeDatabase = Environment.GetEnvironmentVariable(SystemConstant.InitializeDatabase.Code).ToLower() == "true";
                }
                else
                {
                    enableInitializeDatabase = app.Configuration.GetSection(SystemConstant.InitializeDatabase.Code).Get<bool>();
                }

                if (!enableInitializeDatabase)
                {
                    Console.WriteLine("已跳过初始化数据库");
                    return;
                }

                using var scope = app.Services.CreateScope();
                var initializer = scope.ServiceProvider.GetService<IDatabaseInitializer>();
                initializer?.InitializeDatabase();
            }
            catch (Exception ex)
            {
                var message = LocalizationHelper.GetLocalizedString(
                    $"Database initialization failed: {ex.Message}. Please manually initialize or fix the error.",
                    $"数据库初始化失败：{ex.Message}，请手动初始化或修复错误。");
                Console.WriteLine(ex.Message);
                throw new Exception(message, ex);
            }
            finally
            {
                Console.WriteLine(LocalizationHelper.GetLocalizedString(
                    "Database initialization completed.",
                    "数据库初始化完成。"));
            }
        }

        public static void ConfigureEndpoints(this WebApplication app)
        {
            app.MapControllers();
            app.MapGet("api/version", () =>
                $"Software Version: {SoftwareVersionHelper.GetSoftwareVersion(app.Configuration["SoftwareVersion"], "Local Mode")}");
        }

        public static void ConfigureSwaggerUI(this WebApplication app)
        {
            app.UseOpenApi();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerUi(config =>
                {
                    config.Path = "/swagger";
                });
            }
            app.UseReDoc(config =>
            {
                config.Path = "/redoc";
                config.DocumentPath = "/swagger/v1/swagger.json";
            });
        }
    }
}
