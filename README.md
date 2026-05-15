<h1 align="center"><img src="https://foruda.gitee.com/avatar/1677165732744604624/7158691_java-and-net_1677165732.png!avatar100" alt="组织logo.png" /></h1>
<h1 align="center">TopskyHotelManagementSystem-WebApi</h1>
<p align="center">
	<a href='https://gitee.com/java-and-net/topsky-hotel-management-system-web-api/stargazers'><img src='https://gitee.com/java-and-net/topsky-hotel-management-system-web-api/badge/star.svg?theme=white' alt='star'></img></a>
        <a href='https://gitee.com/java-and-net/topsky-hotel-management-system-web-api/fork'><img src='https://gitee.com/java-and-net/topsky-hotel-management-system-web-api/badge/fork.svg?theme=white' alt='fork'></img></a>
        <a href='https://img.shields.io/badge/license-MIT-000000.svg'><img src="https://img.shields.io/badge/license-MIT-000000.svg" alt=""></img></a>
        <a href='https://img.shields.io/badge/language-C#-red.svg'><img src="https://img.shields.io/badge/language-CSharp-red.svg" alt=""></img></a>
</p>
<div align="center">
	<p>中文文档 | <a href="./README.en.md">English Document</a></p>
	<p>项目 Wiki：<a href="https://gitee.com/java-and-net/topsky-hotel-management-system-web-api/wikis/pages">https://gitee.com/java-and-net/topsky-hotel-management-system-web-api/wikis/pages</a></p>
</div>

## 项目简介

本项目是基于 **.NET 10** 构建的 **TS酒店管理系统** 后端 Web API 项目。采用 **C#** 语言编写，使用 **SqlSugar** ORM 框架，支持 **MariaDB**, **MySQL**, **PostgreSQL**, **SQL Server** 等多种数据库。

主要用于现有的 C/S 项目酒店管理系统升级 2.0 后实现前后端分离，提供完善的业务接口支持。本项目不包含前端 UI 界面，仅提供 RESTful API 服务。

## 核心功能特性

### 0. 账户安全增强（TOTP 2FA）
- **支持 TOTP 双因子认证**：员工、管理员、客户三类账号均支持开启/关闭 2FA。
- **支持恢复备用码**：当用户丢失验证器时，可使用一次性备用码完成登录。
- **首次启用即返回备用码**：`EnableTwoFactor` 成功后会直接返回首批备用码，避免前端重复重置生成。
- **安全告警**：检测到“备用码登录”后，系统会在登录成功前尝试发送邮件通知用户。
- **接口能力完整**：支持状态查询、绑定信息生成、启用、关闭、备用码重置与剩余数量查询。

### 1. 业务管理模块
- **房间管理**：支持房间状态（空房、已住、维修、脏房、预约）管理，入住、退房、换房，房间配置（类型、价格）。
- **客户管理**：客户档案管理，客户账号注册登录，会员类型管理。
- **预订管理**：客房预订，预订过期自动检测（基于 Quartz），支持邮件通知提醒。
- **消费管理**：商品销售管理（库存），客房消费（水电费、住宿费），结算状态跟踪。
- **员工管理**：员工档案管理，考勤打卡（上下班签到），奖惩记录，履历管理，证件照片管理。
- **资产管理**：酒店固定资产管理（资产编号、价值、来源、经办人）。
- **新闻公告**：酒店内部新闻发布，系统公告管理。

### 2. 系统管理模块 (RBAC)
- **管理员管理**：支持超级管理员，普通管理员，管理员类型。
- **角色与权限**：基于角色的访问控制 (RBAC)，细粒度的权限分配（菜单权限、按钮权限）。
- **菜单管理**：动态菜单配置，支持按角色构建菜单树。
- **数据字典**：支持民族、学历、部门、职位、证件类型等基础数据维护。

### 3. 数据统计与监控
- **管理驾驶舱 (Dashboard)**：提供房间状态统计（空/住/修/脏/预）、营业统计（会员性别比例、消费趋势日月年）、物资库存预警、员工考勤统计（出勤/迟到/旷工）。
- **操作日志**：详细的 HTTP 请求日志（耗时、参数、IP）和业务操作日志记录。
- **监管统计**：支持酒店内部监管数据的录入与查询。

### 4. 基础设施与安全
- **多数据库支持**：通过 SqlSugar 实现无缝切换主流关系型数据库，支持一键初始化建库建表。
- **安全机制**：
    - **JWT 双Token机制**：采用短效 Access Token（15分钟）+ 长效 Refresh Token（7天）的机制，大幅提升安全性。
      - Access Token 用于 API 请求认证。
      - Refresh Token 存储在 HttpOnly Cookie 中，防止 XSS 攻击。
      - 支持自动 Token 轮换和撤销，登出时同时清理两个 Token。
      - 详见 `docs/frontend-jwt-integration-guide.md` 前端对接文档。
    - **CSRF**：防止跨站请求伪造攻击。
    - **Data Protection**：敏感数据（如身份证号、联系方式）使用 ASP.NET Core Data Protection API 加密存储。
    - **请求日志**：全局请求中间件，记录接口调用详情。
- **定时任务**：使用 Quartz .NET 处理预订过期提醒邮件发送。
- **第三方集成**：
    - **邮件服务**：MailKit 集成，支持发送提醒邮件（HTML 模板）。
    - **图床服务**：兰空图床 (Lsky) 集成，用于图片上传托管。

## 技术栈

- **.NET 10**: 基础框架。
- **C# 12**: 开发语言。
- **SqlSugar**: ORM 框架（多租户/多数据库支持，高性能）。
- **Autofac**: IoC 容器（依赖注入）。
- **Quartz .NET**: 定时任务调度（处理过期预订）。
- **MailKit**: 邮件发送库。
- **NSwag**: API 文档生成（Swagger UI）。

## 项目结构

项目采用分层架构设计，结构清晰，便于维护和扩展：

```text
EOM.TSHotelManagement.Web
├─ EOM.TSHotelManagement.API          # API 入口层 (Controllers, Middleware, Config)
├─ EOM.TSHotelManagement.Contract     # 契约层 (DTO, Request/Response Models, Service Interfaces)
├─ EOM.TSHotelManagement.Domain       # 领域层 (Entities, Domain Logic)
├─ EOM.TSHotelManagement.Service      # 服务层 (Business Logic Implementation)
├─ EOM.TSHotelManagement.Data         # 数据层 (DbContext, Repositories, Database Init)
├─ EOM.TSHotelManagement.Infrastructure # 基础设施层 (Config Models, Helpers, JWT Config)
├─ EOM.TSHotelManagement.Common       # 公共层 (Utils, Constants, Enums, Email Templates)
└─ EOM.TSHotelManagement.Migration    # 数据库迁移工具
```

## 数据库支持

本项目已通过 SqlSugar 框架支持多数据库一键建库建表：

| 数据库     | 版本      | 支持建库建表 | 状态 |
| ---------- | --------- | :----------: | :--: |
| MariaDB    | 10.11.10+ |      ✅       |  ✅   |
| PostgreSQL | 13+       |      ✅       |  ✅   |
| MySQL      | 8.0+      |      ✅       |  ✅   |
| SQL Server | 2022+     |      ✅       |  ✅   |
| Oracle     | -         |      ❌       |  ❌   |
| SQLite     | -         |      ❌       |  ❌   |

## 快速开始

### 环境要求
- .NET 10 SDK
- Runtime 10.x
- Visual Studio 2026 (或 VS Code + C# Dev Kit)

### 本地运行

1. **克隆项目**：
   ```bash
   git clone https://gitee.com/java-and-net/topsky-hotel-management-system-web-api.git
   ```

2. **配置数据库**：
   修改 `EOM.TSHotelManagement.API/appsettings.json`：
   ```json
   {
     "DefaultDatabase": "MariaDB", // 可选值: MariaDB, MySql, PgSql, SqlServer
     "ConnectionStrings": {
       "MariaDB": "Server=localhost;Database=tshoteldb;User=root;Password=123456;"
       // ...
     },
     "InitializeDatabase": true // ⚠️ 首次运行请设为 true 以自动创建数据库和表结构
   }
   ```

3. **配置密钥**：
   在 `appsettings.json` 中设置 JWT 相关配置：
   ```json
   {
     "Jwt": {
       "Key": "your-secret-key-must-be-long-enough",
       "ExpiryMinutes": 15,              // Access Token 过期时间（分钟）
       "RefreshTokenExpiryDays": 7       // Refresh Token 过期时间（天）
     }
   }
   ```
   同时配置 `DataProtection` 相关的 Key (用于敏感数据加解密)。

4. **运行项目**：
   使用 Visual Studio 打开 `EOM.TSHotelManagement.Web.sln` 并启动 `EOM.TSHotelManagement.API` 项目。

### Docker 部署

项目提供了 Dockerfile(亦可通过build.ps1文件快速构建镜像，前提需确保本地启用WSL2.0以及Hyper-V和安装Docker Desktop)，支持 Docker 容器化部署。API 默认监听 8080 端口。  
为了避免手写超长 `docker run` 命令，仓库已提供 `docker-compose.yml` + `.env.example`。

```bash
# 1) 准备环境变量文件
cp .env.example .env

# Windows PowerShell 可用:
# Copy-Item .env.example .env

# 2) 按需修改 .env（数据库连接、JWT 密钥、幂等策略、邮箱等）

# 3) 启动
docker compose up -d

# 4) 查看日志
docker compose logs -f tshotel-api

# 5) 停止并移除容器
docker compose down
```

如果你仍想使用 `docker run`，也可以改成 `--env-file` 方式，命令会短很多：

```bash
docker run -d \
  --name tshotel-api \
  --env-file .env \
  -v /app/config:/app/config \
  -v /app/keys:/app/keys \
  -p 63001:8080 \
  yjj6731/tshotel-management-system-api:latest
```

| 参数名称 | 参数说明 | 必填(Y/N) | 默认值 | 可选值 |
|------|------|---------|-----|-----|
|name|容器名称|Y|N/A|N/A|
|DefaultDatabase|默认数据库|Y|N/A|MariaDB/MySql/SqlServer/PgSql|
|ASPNETCORE_ENVIRONMENT|系统环境(决定Dataprotection Key的生成位置以及环境判断)|Y|docker|docker|
|{默认数据库(e.g:MariaDB/MySql/SqlServer/PgSql)}ConnectStr|对应数据库链接字符串|Y|N/A|N/A|
|Jwt__Key|JWT Key|Y|无，必须设置|N/A|
|Jwt__ExpiryMinutes|Access Token有效时间/分钟|Y|15|N/A|
|Jwt__RefreshTokenExpiryDays|Refresh Token有效时间/天|Y|7|N/A|
|Lsky__Enabled|是否启用兰空图床集成|Y|false|true/false|
|Lsky__BaseAddress|兰空图床基础地址|Y|N/A|N/A|
|Lsky__Email|兰空图床账户邮箱|Y|N/A|N/A|
|Lsky__Password|兰空图床账户密码|Y|N/A|N/A|
|Lsky__UploadApi|兰空图床上传图片接口|Y|N/A|N/A|
|Lsky__GetTokenApi|兰空图床获取tokens接口|Y|N/A|N/A|
|Mail__Enabled|是否启用邮件服务|Y|true|true/false|
|Mail__Host|邮箱smtp协议地址|Y|smtp.example.com|N/A|
|Mail__UserName|邮箱smtp协议地址|Y|N/A|N/A|
|Mail__Port|邮箱smtp端口|Y|465|N/A|
|Mail__Password|邮箱密码|Y|N/A|N/A|
|Mail__EnableSsl|是否启用SSL|Y|true|true/false|
|Mail__DisplayName|发送邮件人显示名称|Y|N/A|N/A|
|InitializeDatabase|初始化数据库|N|true|true/false|
|ExpirationSettings__NotifyDaysBefore|提前通知天数|Y|3|a few days|
|ExpirationSettings__CheckIntervalMinutes|通知检查间隔|Y|5|a few minutes|
|AllowedOrigins__0|允许域站点,用于开发环境|Y|http://localhost:8080|http://localhost:8080|
|AllowedOrigins__1|允许域站点,用于生产环境|Y|https://www.yourdomain.com|https://www.yourdomain.com|
|SoftwareVersion|软件版本号,用于标记说明|N|N/A|N/A|
|JobKeys__0|定时任务1|Y|ReservationExpirationCheckJob:0 0 1 * * ?|JobName:CronExpression|
|JobKeys__1|定时任务2|Y|MailServiceCheckJob:0 */5 * * * ?|JobName:CronExpression|
|JobKeys__2|定时任务3|Y|RedisServiceCheckJob:0 */5 * * * ?|JobName:CronExpression|
|Redis__Enabled|是否启用Redis服务|N|false|true/false|
|Redis__ConnectionString|Redis连接字符串|N|N/A|N/A|
|Redis__DefaultDatabase|默认数据库|N|0|0|
|Redis__ConnectTimeoutMs|Redis 建连超时(毫秒)|N|5000|1000~30000|
|Redis__AsyncTimeoutMs|Redis 异步命令超时(毫秒)|N|2000|500~30000|
|Redis__SyncTimeoutMs|Redis 同步命令超时(毫秒)|N|2000|500~30000|
|Redis__KeepAliveSeconds|Redis KeepAlive 间隔(秒)|N|15|5~300|
|Redis__ConnectRetry|Redis 连接重试次数|N|3|1~10|
|Redis__ReconnectRetryBaseDelayMs|Redis 重连指数退避基准延迟(毫秒)|N|3000|500~30000|
|Redis__OperationTimeoutMs|JWT 吊销检查操作超时(毫秒)|N|1200|200~5000|
|Redis__FailureCooldownSeconds|Redis 失败后熔断冷却时间(秒)|N|30|5~300|
|Idempotency__Enabled|是否启用幂等键中间件|N|true|true/false|
|Idempotency__EnforceKey|是否强制写请求必须携带 Idempotency-Key|N|false|true/false|
|Idempotency__MaxKeyLength|Idempotency-Key 最大长度|N|128|>=16 的整数|
|Idempotency__InProgressTtlSeconds|处理中记录 TTL（秒）|N|120|30~600|
|Idempotency__CompletedTtlHours|完成记录 TTL（小时）|N|24|1~168|
|Idempotency__PersistFailureResponse|是否缓存失败响应（非2xx）|N|false|true/false|

> ⚠️ **安全提醒**：生产环境中请勿直接通过 `-e` 明文传入密码类参数，推荐使用 Docker Secrets 或环境变量注入工具（如 HashiCorp Vault）进行保护。

## 开发节奏

![development_pace](https://picrepo.oscode.top/i/2026/02/18/Development_pace.png)

## 鸣谢

感谢以下优秀的开源项目：

1. **Autofac** - An addictive .NET IoC container. ([MIT](https://github.com/autofac/Autofac))
2. **SqlSugar** - 国内最受欢迎ORM框架. ([MIT](https://gitee.com/dotnetchina/SqlSugar))
3. **Mailkit** - A cross-platform .NET library for IMAP, POP3, and SMTP. ([MIT](https://github.com/jstedfast/MailKit))
4. **NSwag** - The OpenAPI/Swagger API toolchain for .NET and TypeScript. ([MIT](https://github.com/NSwag/NSwag))
5. **Quartz .NET** - Open-source job scheduling system for .NET. ([Apache 2.0](https://github.com/quartznet/quartznet))

## 项目作者

- **ck_yeun9** (原版本)
- **shaun** (pro版本)


## 许可证

本项目基于 **MIT** 协议开源。免费开源，但请勿用于商业用途（具体请阅读 LICENSE 文件）。

tshotel.oscode.top/signin
账号密码都是demo

​       [![咖啡与网络/TopskyHotelManagementSystem-WebApi](https://gitee.com/java-and-net/topsky-hotel-management-system-web-api/widgets/widget_card.svg?colors=4183c4,ffffff,ffffff,e3e9ed,666666,9b9b9b)](https://gitee.com/java-and-net/topsky-hotel-manager-system-web-api)
