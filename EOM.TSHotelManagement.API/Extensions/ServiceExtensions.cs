using EOM.TSHotelManagement.API.Filters;
using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Common.QuartzWorkspace.BusinessJob;
using EOM.TSHotelManagement.Infrastructure;
using EOM.TSHotelManagement.Service;
using EOM.TSHotelManagement.WebApi.Authorization;
using EOM.TSHotelManagement.WebApi.Filters;
using jvncorelib.CodeLib;
using jvncorelib.EncryptorLib;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using Quartz;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace EOM.TSHotelManagement.WebApi
{
    public static class ServiceExtensions
    {
        private const string AuthFailureReasonItemKey = JwtAuthConstants.AuthFailureReasonItemKey;
        private const string AuthFailureReasonTokenRevoked = JwtAuthConstants.AuthFailureReasonTokenRevoked;
        private const string AuthFailureReasonTokenExpired = JwtAuthConstants.AuthFailureReasonTokenExpired;
        private const string AuthFailureReasonTokenInvalid = JwtAuthConstants.AuthFailureReasonTokenInvalid;
        private const string JwtTokenUserIdItemKey = JwtAuthConstants.JwtTokenUserIdItemKey;
        private const string JwtTokenJtiItemKey = JwtAuthConstants.JwtTokenJtiItemKey;

        public static void ConfigureDataProtection(this IServiceCollection services, IConfiguration configuration)
        {
            if (Environment.GetEnvironmentVariable(SystemConstant.Env.Code) == SystemConstant.Docker.Code)
            {
                services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo("/app/keys"))
                    .SetApplicationName("TSHotelManagementSystem");
            }
            else
            {
                services.AddDataProtection().SetApplicationName("TSHotelManagementSystem");
            }
        }

        public static void ConfigureQuartz(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddQuartz(q =>
            {
                var jobs = configuration.GetSection(SystemConstant.JobKeys.Code).Get<string[]>() ?? Array.Empty<string>();
                var jobRegistrations = new Dictionary<string, Action<IServiceCollectionQuartzConfigurator, string, string>>(StringComparer.OrdinalIgnoreCase)
                {
                    [nameof(ReservationExpirationCheckJob)] = (configurator, jobName, cronExpression) =>
                        RegisterJobAndTrigger<ReservationExpirationCheckJob>(configurator, jobName, cronExpression),
                    [nameof(MailServiceCheckJob)] = (configurator, jobName, cronExpression) =>
                        RegisterJobAndTrigger<MailServiceCheckJob>(configurator, jobName, cronExpression),
                    [nameof(ImageHostingServiceCheckJob)] = (configurator, jobName, cronExpression) =>
                        RegisterJobAndTrigger<ImageHostingServiceCheckJob>(configurator, jobName, cronExpression),
                    [nameof(RedisServiceCheckJob)] = (configurator, jobName, cronExpression) =>
                        RegisterJobAndTrigger<RedisServiceCheckJob>(configurator, jobName, cronExpression),
                    [nameof(AutomaticallyUpgradeMembershipLevelJob)] = (configurator, jobName, cronExpression) =>
                        RegisterJobAndTrigger<AutomaticallyUpgradeMembershipLevelJob>(configurator, jobName, cronExpression)
                };
                var registeredJobs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var job in jobs.Where(a => !string.IsNullOrWhiteSpace(a)).Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    var (jobName, cronExpression) = ParseJobRegistration(job);
                    if (!registeredJobs.Add(jobName))
                    {
                        throw new InvalidOperationException($"Duplicate quartz job configuration found for '{jobName}'.");
                    }

                    if (!jobRegistrations.TryGetValue(jobName, out var register))
                    {
                        throw new InvalidOperationException($"Unsupported quartz job '{jobName}' in '{SystemConstant.JobKeys.Code}'.");
                    }

                    register(q, jobName, cronExpression);
                }
            });

            services.AddQuartzHostedService(q =>
            {
                q.WaitForJobsToComplete = true;
                q.AwaitApplicationStarted = true;
                q.StartDelay = TimeSpan.FromSeconds(5);
            });
        }

        private static void RegisterJobAndTrigger<TJob>(IServiceCollectionQuartzConfigurator quartz, string jobName, string cronExpression)
            where TJob : IJob
        {
            quartz.AddJob<TJob>(opts => opts
                .WithIdentity(jobName)
                .StoreDurably()
                .WithDescription($"{jobName} 定时作业"));

            quartz.AddTrigger(opts => opts
                .ForJob(jobName)
                .WithIdentity($"{jobName}-Trigger")
                .WithCronSchedule(cronExpression));
        }

        private static (string JobName, string CronExpression) ParseJobRegistration(
            string jobConfiguration)
        {
            var entry = jobConfiguration?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(entry))
            {
                throw new InvalidOperationException($"Invalid quartz job configuration value in '{SystemConstant.JobKeys.Code}'.");
            }

            var separatorIndex = entry.IndexOf(':');
            if (separatorIndex > 0 && separatorIndex < entry.Length - 1)
            {
                var jobName = entry[..separatorIndex].Trim();
                var cronExpression = entry[(separatorIndex + 1)..].Trim();
                return (jobName, cronExpression);
            }

            throw new InvalidOperationException(
                $"Quartz job '{entry}' is missing cron expression. Use 'JobName:CronExpression' format.");
        }

        public static void ConfigureXForward(this IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                           ForwardedHeaders.XForwardedProto;

                options.KnownIPNetworks.Clear();
                options.KnownProxies.Clear();
            });
        }

        public static void RegisterSingletonServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ValidationFilter>();
            services.AddHttpClient("HeartBeatCheckClient", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            services.AddSingleton<RedisConfigFactory>();
            services.AddSingleton<JwtConfigFactory>();
            services.AddSingleton<MailConfigFactory>();
            services.AddSingleton<LskyConfigFactory>();
            services.AddSingleton<TwoFactorConfigFactory>();
            services.AddSingleton<DataProtectionHelper>();
            services.Configure<CsrfTokenConfig>(configuration.GetSection("CsrfToken"));
            services.AddSingleton<EncryptLib>();
            services.AddSingleton<UniqueCode>();
            services.AddSingleton<DeleteConcurrencyHelper>();
            services.AddHostedService<DeleteConcurrencyHelperWarmupService>();

            // RBAC: 注册基于权限码的动态策略提供者与处理器
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAuthorizationMiddlewareResultHandler>();
        }

        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured")))
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var authorizationHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
                        if (!JwtTokenRevocationService.TryGetBearerToken(authorizationHeader, out var token))
                        {
                            context.Fail("Missing token.");
                            return;
                        }

                        var userId = context.Principal?.FindFirst(ClaimTypes.SerialNumber)?.Value
                                     ?? context.Principal?.FindFirst("serialnumber")?.Value
                                     ?? context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                     ?? context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                        if (!string.IsNullOrWhiteSpace(userId))
                        {
                            context.HttpContext.Items[JwtTokenUserIdItemKey] = userId;
                        }

                        var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                        if (!string.IsNullOrWhiteSpace(jti))
                        {
                            context.HttpContext.Items[JwtTokenJtiItemKey] = jti;
                        }

                        var tokenRevocationService = context.HttpContext.RequestServices
                            .GetRequiredService<JwtTokenRevocationService>();

                        if (await tokenRevocationService.IsTokenRevokedAsync(token))
                        {
                            context.HttpContext.Items[AuthFailureReasonItemKey] = AuthFailureReasonTokenRevoked;
                            context.Fail("Token has been revoked.");
                        }
                    },
                    OnAuthenticationFailed = context =>
                    {
                        context.HttpContext.Items[AuthFailureReasonItemKey] = ResolveAuthFailureReason(context.Exception);
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiAccess", policy =>
                {
                    policy.AuthenticationSchemes.Add("Bearer");
                    policy.RequireAuthenticatedUser();
                });
                options.DefaultPolicy = options.GetPolicy("ApiAccess")!;
            });

            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = "XSRF-TOKEN";
                options.HeaderName = "X-CSRF-TOKEN-HEADER";
                options.Cookie.HttpOnly = false;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.SuppressXFrameOptionsHeader = false;
            });
        }

        public static void ConfigureControllers(this IServiceCollection services)
        {
            services.AddScoped<BusinessOperationAuditFilter>();

            services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
                options.Filters.Add<BusinessOperationAuditFilter>();
                options.Conventions.Add(new AuthorizeAllControllersConvention());
                options.Conventions.Add(new ClientApiGroupConvention());
                options.RespectBrowserAcceptHeader = true;
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.DictionaryKeyPolicy = null;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = false;
                options.InvalidModelStateResponseFactory = context =>
                {
                    var result = new BadRequestObjectResult(new
                    {
                        Message = "验证失败",
                        Errors = context.ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });
                    return result;
                };
            });

            // 全局路由配置
            services.AddMvc(opt =>
                opt.UseCentralRoutePrefix(new RouteAttribute("api/[controller]/[action]")));

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddOpenApiDocument(config =>
            {
                config.Title = "TS酒店管理系统API说明文档";
                config.Version = "v1";
                config.DocumentName = "v1";
                config.ApiGroupNames = ClientApiGroups.All;

                config.OperationProcessors.Add(new CSRFTokenOperationProcessor());

                config.AddSecurity("JWT", new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.Http,
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Name = "Authorization",
                    Description = "Type into the textbox: your JWT token",
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            });

            services.AddOpenApiDocument(config =>
            {
                config.Title = "TS Hotel Management System API - Web";
                config.Version = ClientApiGroups.Web;
                config.DocumentName = ClientApiGroups.Web;
                config.ApiGroupNames = new[] { ClientApiGroups.Web };

                config.OperationProcessors.Add(new CSRFTokenOperationProcessor());

                config.AddSecurity("JWT", new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.Http,
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Name = "Authorization",
                    Description = "Type into the textbox: your JWT token",
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            });

            services.AddOpenApiDocument(config =>
            {
                config.Title = "TS Hotel Management System API - Desktop";
                config.Version = ClientApiGroups.Desktop;
                config.DocumentName = ClientApiGroups.Desktop;
                config.ApiGroupNames = new[] { ClientApiGroups.Desktop };

                config.OperationProcessors.Add(new CSRFTokenOperationProcessor());

                config.AddSecurity("JWT", new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.Http,
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Name = "Authorization",
                    Description = "Type into the textbox: your JWT token",
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            });

            services.AddOpenApiDocument(config =>
            {
                config.Title = "TS Hotel Management System API - Mobile";
                config.Version = ClientApiGroups.Mobile;
                config.DocumentName = ClientApiGroups.Mobile;
                config.ApiGroupNames = new[] { ClientApiGroups.Mobile };

                config.OperationProcessors.Add(new CSRFTokenOperationProcessor());

                config.AddSecurity("JWT", new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.Http,
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Name = "Authorization",
                    Description = "Type into the textbox: your JWT token",
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            });
        }

        public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
            services.AddCors(options =>
            {
                options.AddPolicy("MyCorsPolicy", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials()
                          .WithExposedHeaders("X-CSRF-TOKEN-HEADER")
#if DEBUG
                          .SetIsOriginAllowed(_ => true) // 开发环境下允许所有来源
#endif
                          .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
                });
            });
        }

        private static string ResolveAuthFailureReason(Exception exception)
        {
            return exception switch
            {
                SecurityTokenExpiredException => AuthFailureReasonTokenExpired,
                SecurityTokenInvalidSignatureException => AuthFailureReasonTokenInvalid,
                SecurityTokenInvalidAudienceException => AuthFailureReasonTokenInvalid,
                SecurityTokenInvalidIssuerException => AuthFailureReasonTokenInvalid,
                SecurityTokenNoExpirationException => AuthFailureReasonTokenInvalid,
                _ => AuthFailureReasonTokenInvalid
            };
        }
    }
    internal sealed class DeleteConcurrencyHelperWarmupService : IHostedService
    {
        public DeleteConcurrencyHelperWarmupService(DeleteConcurrencyHelper helper)
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
