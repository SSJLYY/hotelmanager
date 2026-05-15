using EOM.TSHotelManagement.Domain;

namespace EOM.TSHotelManagement.Migration
{
    public class EntityBuilder
    {
        public EntityBuilder(string? initialAdminEncryptedPassword = null, string? initialEmployeeEncryptedPassword = null)
        {
            if (string.IsNullOrWhiteSpace(initialAdminEncryptedPassword) || string.IsNullOrWhiteSpace(initialEmployeeEncryptedPassword))
            {
                throw new ArgumentException("Initial encrypted passwords for administrator and employee are required.");
            }

            var admin = entityDatas
                .OfType<Administrator>()
                .FirstOrDefault(a => string.Equals(a.Account, "admin", StringComparison.OrdinalIgnoreCase));

            if (admin != null)
            {
                admin.Password = initialAdminEncryptedPassword;
            }

            var employee = entityDatas
                .OfType<Employee>()
                .FirstOrDefault(a => string.Equals(a.EmployeeId, "WK010", StringComparison.OrdinalIgnoreCase));

            if (employee != null)
            {
                employee.Password = initialEmployeeEncryptedPassword;
            }
        }

        private readonly Type[] entityTypes =
        {
            typeof(Administrator),
            typeof(AdministratorType),
            typeof(AdministratorPhoto),
            typeof(AppointmentNotice),
            typeof(AppointmentNoticeType),
            typeof(Asset),
            typeof(Customer),
            typeof(CustomerAccount),
            typeof(CustoType),
            typeof(CardCode),
            typeof(Department),
            typeof(Employee),
            typeof(EmployeeCheck),
            typeof(EmployeeHistory),
            typeof(EmployeePhoto),
            typeof(EmployeeRewardPunishment),
            typeof(EnergyManagement),
            typeof(Education),
            typeof(Menu),
            typeof(Nation),
            typeof(NavBar),
            typeof(UserFavoriteCollection),
            typeof(OperationLog),
            typeof(Position),
            typeof(PromotionContent),
            typeof(PassportType),
            typeof(Room),
            typeof(RoomType),
            typeof(Reser),
            typeof(RewardPunishmentType),
            typeof(Role),
            typeof(RolePermission),
            typeof(SellThing),
            typeof(Spend),
            typeof(SupervisionStatistics),
            typeof(UserRole),
            typeof(VipLevelRule),
            typeof(RequestLog),
            typeof(News),
            typeof(Permission),
            typeof(TwoFactorAuth),
            typeof(TwoFactorRecoveryCode)
        };

        private readonly List<object> entityDatas = new()
        {
            new AdministratorType
            {
                TypeId = "Admin",
                TypeName = "超级管理员",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Administrator
            {
                Number = "AD-202005060001",
                Account = "admin",
                Password = string.Empty,
                Name = "超级管理员",
                Type = "Admin",
                Address = "广东珠海",
                DateOfBirth = DateOnly.FromDateTime(new DateTime(1990,1,1,0,0,0)),
                EducationLevel = "E-000001",
                EmailAddress = string.Empty,
                Ethnicity = "N-000001",
                Gender = 1,
                IdCardNumber = "666",
                IdCardType = 0,
                PhoneNumber = "666",
                IsSuperAdmin = 1,
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 1
            {
                Key = "home",
                Title = "首页",
                Path = "/",
                Parent = null,
                Icon = "HomeOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 2
            {
                Key = "basic",
                Title = "基础信息管理",
                Path = "/",
                Parent = null,
                Icon = "AppstoreOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 3
            {
                Key = "position",
                Title = "职位管理",
                Path = "/position",
                Parent = 2,
                Icon = "PartitionOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 4
            {
                Key = "nation",
                Title = "民族管理",
                Path = "/nation",
                Parent = 2,
                Icon = "FlagOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 5
            {
                Key = "qualification",
                Title = "学历管理",
                Path = "/qualification",
                Parent = 2,
                Icon = "ReadOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 6
            {
                Key = "department",
                Title = "部门管理",
                Path = "/department",
                Parent = 2,
                Icon = "ApartmentOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 7
            {
                Key = "noticetype",
                Title = "公告类型管理",
                Path = "/noticetype",
                Parent = 2,
                Icon = "ApartmentOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 8
            {
                Key = "passport",
                Title = "证件类型管理",
                Path = "/passport",
                Parent = 2,
                Icon = "IdcardOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 9
            {
                Key = "finance",
                Title = "财务信息管理",
                Path = "/",
                Parent = null,
                Icon = "WalletOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 10
            {
                Key = "internalfinance",
                Title = "内部资产管理",
                Path = "/internalfinance",
                Parent = 9,
                Icon = "DollarOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 11
            {
                Key = "hydroelectricity",
                Title = "水电信息管理",
                Path = "/",
                Parent = null,
                Icon = "FireOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 12
            {
                Key = "hydroelectricinformation",
                Title = "水电信息管理",
                Path = "/hydroelectricinformation",
                Parent = 11,
                Icon = "FireOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 13
            {
                Key = "supervisionmanagement",
                Title = "监管统计管理",
                Path = "/",
                Parent = null,
                Icon = "AuditOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 14
            {
                Key = "supervisioninfo",
                Title = "监管情况",
                Path = "/supervisioninfo",
                Parent = 13,
                Icon = "AuditOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 15
            {
                Key = "roominformation",
                Title = "客房信息管理",
                Path = "/",
                Parent = null,
                Icon = "HolderOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 16
            {
                Key = "resermanagement",
                Title = "预约管理",
                Path = "/resermanagement",
                Parent = 15,
                Icon = "BellOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 17
            {
                Key = "roommap",
                Title = "房态图一览",
                Path = "/roommap",
                Parent = 15,
                Icon = "TableOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 18
            {
                Key = "roommanagement",
                Title = "客房管理",
                Path = "/roommanagement",
                Parent = 15,
                Icon = "HomeOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 19
            {
                Key = "roomconfig",
                Title = "客房配置",
                Path = "/roomconfig",
                Parent = 15,
                Icon = "ToolOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 20
            {
                Key = "customermanagement",
                Title = "酒店客户管理",
                Path = "/",
                Parent = null,
                Icon = "DesktopOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 21
            {
                Key = "viplevel",
                Title = "会员等级规则",
                Path = "/viplevel",
                Parent = 20,
                Icon = "CrownOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 22
            {
                Key = "customer",
                Title = "客户信息管理",
                Path = "/customer",
                Parent = 20,
                Icon = "ContactsOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 23
            {
                Key = "customerspend",
                Title = "客户消费账单",
                Path = "/customerspend",
                Parent = 20,
                Icon = "PayCircleOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 24
            {
                Key = "customertype",
                Title = "客户类型管理",
                Path = "/customertype",
                Parent = 20,
                Icon = "TagOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 25
            {
                Key = "humanresourcemanagement",
                Title = "酒店人事管理",
                Path = "/",
                Parent = null,
                Icon = "TeamOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 26
            {
                Key = "staffmanagement",
                Title = "员工管理",
                Path = "/staffmanagement",
                Parent = 25,
                Icon = "TeamOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 27
            {
                Key = "materialmanagement",
                Title = "酒店物资管理",
                Path = "/",
                Parent = null,
                Icon = "ProjectOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 28
            {
                Key = "goodsmanagement",
                Title = "商品管理",
                Path = "/goodsmanagement",
                Parent = 27,
                Icon = "ShopOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 29
            {
                Key = "operationmanagement",
                Title = "行为操作管理",
                Path = "/",
                Parent = null,
                Icon = "CoffeeOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 30
            {
                Key = "operationlog",
                Title = "操作日志",
                Path = "/operationlog",
                Parent = 29,
                Icon = "SolutionOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 31
            {
                Key = "systemmanagement",
                Title = "系统管理",
                Path = "/",
                Parent = null,
                Icon = "ToolOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 32
            {
                Key = "administratormanagement",
                Title = "管理员管理",
                Path = "/administratormanagement",
                Parent = 31,
                Icon = "KeyOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 33
            {
                Key = "menumanagement",
                Title = "菜单管理",
                Path = "/menumanagement",
                Parent = 31,
                Icon = "MenuOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 34
            {
                Key = "rolemanagement",
                Title = "角色管理",
                Path = "/rolemanagement",
                Parent = 31,
                Icon = "SmileOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 35
            {
                Key = "admintypemanagement",
                Title = "管理员类型管理",
                Path = "/admintypemanagement",
                Parent = 31,
                Icon = "TagOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 36
            {
                Key = "quartzjoblist",
                Title = "Quartz任务列表",
                Path = "/quartzjoblist",
                Parent = 31,
                Icon = "OrderedListOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 37
            {
                Key = "my",
                Title = "我的",
                Path = "/home",
                Parent = 1,
                Icon = "HomeOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 38
            {
                Key = "dashboard",
                Title = "仪表盘",
                Path = "/dashboard",
                Parent = 1,
                Icon = "DashboardOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 39
            {
                Key = "promotioncontent",
                Title = "宣传联动内容",
                Path = "/promotioncontent",
                Parent = 2,
                Icon = "DashboardOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new Menu // 40
            {
                Key = "requestlog",
                Title = "请求日志",
                Path = "/requestlog",
                Parent = 29,
                Icon = "SolutionOutlined",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new NavBar
            {
                NavigationBarName = "客房管理",
                NavigationBarOrder = 1,
                NavigationBarImage = string.Empty,
                NavigationBarEvent = "RoomManager_Event",
                IsDelete = 0,
                MarginLeft = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now
            },
            new NavBar
            {
                NavigationBarName = "客户管理",
                NavigationBarOrder = 2,
                NavigationBarImage = string.Empty,
                NavigationBarEvent = "CustomerManager_Event",
                IsDelete = 0,
                MarginLeft = 120,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now
            },
            new NavBar
            {
                NavigationBarName = "商品消费",
                NavigationBarOrder = 3,
                NavigationBarImage = string.Empty,
                NavigationBarEvent = "SellManager_Event",
                IsDelete = 0,
                MarginLeft = 120,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now
            },
            new Department
            {
              DepartmentNumber = "D-000001",
              DepartmentName = "酒店部",
              DepartmentDescription = null,
              DepartmentCreationDate = DateOnly.FromDateTime(DateTime.Now),
              DepartmentLeader = "WK010",
              IsDelete = 0,
              DataInsUsr = "System",
              DataInsDate = DateTime.Now
            },
            new Position
            {
              PositionNumber = "P-000001",
              PositionName = "初级职员",
              IsDelete = 0,
              DataInsUsr = "System",
              DataInsDate = DateTime.Now
            },
            new Education
            {
              EducationNumber = "E-000001",
              EducationName = "本科",
              IsDelete = 0,
              DataInsUsr = "System",
              DataInsDate = DateTime.Now
            },
            new Nation
            {
              NationNumber = "N-000001",
              NationName = "汉族",
              IsDelete = 0,
              DataInsUsr = "System",
              DataInsDate = DateTime.Now
            },
            new PassportType
            {
              PassportId = 666,
              PassportName = "中国居民身份证",
              IsDelete = 0,
              DataInsUsr = "System",
              DataInsDate = DateTime.Now
            },
            new Employee
            {
              EmployeeId = "WK010",
              Name = "阿杰",
              DateOfBirth = DateOnly.FromDateTime(new DateTime(1999,7,20,0,0,0)),
              Password = string.Empty,
              Department = "D-000001",
              Position = "P-000001",
              EducationLevel = "E-000001",
              Address = "广东珠海",
              Ethnicity = "N-000001",
              PoliticalAffiliation = "TheMasses",
              EmailAddress = "demo@oscode.top",
              Gender = 1,
              HireDate = DateOnly.FromDateTime(new DateTime(2025,05,06,0,0,0)),
              IdCardNumber = "666",
              PhoneNumber = "666",
              IdCardType = 666,
              IsEnable = 1,
              IsInitialize = 1,
              DataInsUsr = "System",
              DataInsDate = DateTime.Now
            },
            new UserFavoriteCollection
            {
                UserNumber = "WK010",
                LoginType = "employee",
                Account = "WK010",
                FavoriteRoutesJson = "[\"/roommap\"]",
                RouteCount = 1,
                UpdatedAt = DateTime.UtcNow,
                TriggeredBy = "seed",
                DataInsUsr = "System",
                DataInsDate = DateTime.Now
            },
            new PromotionContent
            {
                PromotionContentNumber = "PC-000001",
                PromotionContentMessage = "欢迎使用酒店管理系统！",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new PromotionContent
            {
                PromotionContentNumber = "PC-000002",
                PromotionContentMessage = "本酒店即日起与闪修平台联合推出“多修多折”活动，详情请咨询前台！",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new PromotionContent
            {
                PromotionContentNumber = "PC-000003",
                PromotionContentMessage = "本酒店即日起与神之食餐厅联合推出“吃多折多”活动，详情请咨询前台！",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            },
            new PromotionContent
            {
                PromotionContentNumber = "PC-000004",
                PromotionContentMessage = "本酒店即日起与Second网吧联合推出“免费体验酒店式网吧”活动，详情请咨询前台！",
                IsDelete = 0,
                DataInsUsr = "System",
                DataInsDate = DateTime.Now,
            }

            ,
            // ===== Permission seeds synced from controller [RequirePermission] =====

            // Basic (基础信息管理)
            // 部门
            new Permission { PermissionNumber = "department.create", PermissionName = "新增部门", Module = "basic", Description = "基础信息-部门-新增", MenuKey = "department", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "department.delete", PermissionName = "删除部门", Module = "basic", Description = "基础信息-部门-删除", MenuKey = "department", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "department.export", PermissionName = "导出部门", Module = "basic", Description = "基础信息-部门-导出列表", MenuKey = "department", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "department.update", PermissionName = "更新部门", Module = "basic", Description = "基础信息-部门-更新", MenuKey = "department", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "department.view", PermissionName = "查询部门列表", Module = "basic", Description = "基础信息-部门-查询列表", MenuKey = "department", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },

            // 民族
            new Permission { PermissionNumber = "nation.create", PermissionName = "新增民族", Module = "basic", Description = "基础信息-民族-新增", MenuKey = "nation", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "nation.delete", PermissionName = "删除民族", Module = "basic", Description = "基础信息-民族-删除", MenuKey = "nation", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "nation.export", PermissionName = "导出民族", Module = "basic", Description = "基础信息-民族-导出列表", MenuKey = "nation", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "nation.update", PermissionName = "更新民族", Module = "basic", Description = "基础信息-民族-更新", MenuKey = "nation", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "nation.view", PermissionName = "查询民族列表", Module = "basic", Description = "基础信息-民族-查询列表", MenuKey = "nation", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },

            // 证件类型
            new Permission { PermissionNumber = "passport.create", PermissionName = "新增证件类型", Module = "basic", Description = "基础信息-证件类型-新增", MenuKey = "passport", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "passport.delete", PermissionName = "删除证件类型", Module = "basic", Description = "基础信息-证件类型-删除", MenuKey = "passport", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "passport.export", PermissionName = "导出证件类型", Module = "basic", Description = "基础信息-证件类型-导出列表", MenuKey = "passport", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "passport.update", PermissionName = "更新证件类型", Module = "basic", Description = "基础信息-证件类型-更新", MenuKey = "passport", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "passport.view", PermissionName = "查询证件类型列表", Module = "basic", Description = "基础信息-证件类型-查询列表", MenuKey = "passport", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },

            // 职位
            new Permission { PermissionNumber = "position.create", PermissionName = "新增职位", Module = "basic", Description = "基础信息-职位-新增", MenuKey = "position", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "position.delete", PermissionName = "删除职位", Module = "basic", Description = "基础信息-职位-删除", MenuKey = "position", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "position.export", PermissionName = "导出职位", Module = "basic", Description = "基础信息-职位-导出列表", MenuKey = "position", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "position.update", PermissionName = "更新职位", Module = "basic", Description = "基础信息-职位-更新", MenuKey = "position", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "position.view", PermissionName = "查询职位列表", Module = "basic", Description = "基础信息-职位-查询列表", MenuKey = "position", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },

            // 学历
            new Permission { PermissionNumber = "qualification.create", PermissionName = "新增学历", Module = "basic", Description = "基础信息-学历-新增", MenuKey = "qualification", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "qualification.delete", PermissionName = "删除学历", Module = "basic", Description = "基础信息-学历-删除", MenuKey = "qualification", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "qualification.export", PermissionName = "导出学历", Module = "basic", Description = "基础信息-学历-导出列表", MenuKey = "qualification", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "qualification.update", PermissionName = "更新学历", Module = "basic", Description = "基础信息-学历-更新", MenuKey = "qualification", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "qualification.view", PermissionName = "查询学历列表", Module = "basic", Description = "基础信息-学历-查询列表", MenuKey = "qualification", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            
            // 公告类型
            new Permission { PermissionNumber = "noticetype.create", PermissionName = "添加公告类型", Module = "basic", Description = "添加公告类型", MenuKey = "noticetype", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "noticetype.delete", PermissionName = "删除公告类型", Module = "basic", Description = "删除公告类型", MenuKey = "noticetype", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "noticetype.export", PermissionName = "导出公告类型", Module = "basic", Description = "导出公告类型列表", MenuKey = "noticetype", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "noticetype.update", PermissionName = "更新公告类型", Module = "basic", Description = "更新公告类型", MenuKey = "noticetype", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "noticetype.view", PermissionName = "查询所有公告类型", Module = "basic", Description = "查询所有公告类型", MenuKey = "noticetype", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            
            // 宣传联动内容
            new Permission { PermissionNumber = "promotioncontent.apc", PermissionName = "添加宣传联动内容", Module = "basic", Description = "添加宣传联动内容", MenuKey = "promotioncontent", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "promotioncontent.dpc", PermissionName = "删除宣传联动内容", Module = "basic", Description = "删除宣传联动内容", MenuKey = "promotioncontent", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "promotioncontent.export", PermissionName = "导出宣传联动内容", Module = "basic", Description = "导出宣传联动内容列表", MenuKey = "promotioncontent", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "promotioncontent.spca", PermissionName = "查询所有宣传联动内容", Module = "basic", Description = "查询所有宣传联动内容", MenuKey = "promotioncontent", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "promotioncontent.spcs", PermissionName = "查询所有宣传联动内容(跑马灯)", Module = "basic", Description = "查询所有宣传联动内容(跑马灯)", MenuKey = "promotioncontent", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "promotioncontent.upc", PermissionName = "更新宣传联动内容", Module = "basic", Description = "更新宣传联动内容", MenuKey = "promotioncontent", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },

            // Finance (财务信息管理)
            // 资产信息管理
            new Permission { PermissionNumber = "internalfinance.aai", PermissionName = "添加资产信息", Module = "internalfinance", Description = "添加资产信息", MenuKey = "internalfinance", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "internalfinance.dai", PermissionName = "删除资产信息", Module = "internalfinance", Description = "删除资产信息", MenuKey = "internalfinance", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "internalfinance.export", PermissionName = "导出资产信息", Module = "internalfinance", Description = "导出资产信息列表", MenuKey = "internalfinance", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "internalfinance.saia", PermissionName = "查询资产信息", Module = "internalfinance", Description = "查询资产信息", MenuKey = "internalfinance", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "internalfinance.uai", PermissionName = "更新资产信息", Module = "internalfinance", Description = "更新资产信息", MenuKey = "internalfinance", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },

            // Nav Bar (导航栏管理)
            // 导航控件管理
            new Permission { PermissionNumber = "navbar.addnavbar", PermissionName = "添加导航控件", Module = "client", Description = "添加导航控件", MenuKey = "navbar", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "navbar.dn", PermissionName = "删除导航控件", Module = "client", Description = "删除导航控件", MenuKey = "navbar", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "navbar.navbarlist", PermissionName = "导航控件列表", Module = "client", Description = "导航控件列表", MenuKey = "navbar", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "navbar.un", PermissionName = "更新导航控件", Module = "client", Description = "更新导航控件", MenuKey = "navbar", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },


            // Hydroelectricity (水电信息管理)
            // 水电费信息管理
            new Permission { PermissionNumber = "hydroelectricinformation.demi", PermissionName = "删除水电费信息", Module = "hydroelectricity", Description = "删除水电费信息", MenuKey = "hydroelectricinformation", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "hydroelectricinformation.export", PermissionName = "导出水电费信息", Module = "hydroelectricity", Description = "导出水电费信息列表", MenuKey = "hydroelectricinformation", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "hydroelectricinformation.iemi", PermissionName = "添加水电费信息", Module = "hydroelectricity", Description = "添加水电费信息", MenuKey = "hydroelectricinformation", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "hydroelectricinformation.semi", PermissionName = "查询水电费信息", Module = "hydroelectricity", Description = "查询水电费信息", MenuKey = "hydroelectricinformation", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "hydroelectricinformation.uemi", PermissionName = "修改水电费信息", Module = "hydroelectricity", Description = "修改水电费信息", MenuKey = "hydroelectricinformation", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },


            // Supervision (监管统计管理)
            // 监管统计信息管理
            new Permission { PermissionNumber = "supervisioninfo.dss", PermissionName = "删除监管统计信息", Module = "supervision", Description = "删除监管统计信息", MenuKey = "supervisioninfo", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "supervisioninfo.export", PermissionName = "导出监管统计信息", Module = "supervision", Description = "导出监管统计信息列表", MenuKey = "supervisioninfo", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "supervisioninfo.iss", PermissionName = "插入监管统计信息", Module = "supervision", Description = "插入监管统计信息", MenuKey = "supervisioninfo", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "supervisioninfo.sssa", PermissionName = "查询所有监管统计信息", Module = "supervision", Description = "查询所有监管统计信息", MenuKey = "supervisioninfo", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "supervisioninfo.uss", PermissionName = "更新监管统计信息", Module = "supervision", Description = "更新监管统计信息", MenuKey = "supervisioninfo", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },

            
            // Room information (客房信息管理)
            // 房间管理
            new Permission { PermissionNumber = "roommap.view", PermissionName = "房态图-查看", Module = "room", Description = "房态图一览-查看", MenuKey = "roommap", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roommanagement.crbr", PermissionName = "根据预约信息办理入住", Module = "room", Description = "根据预约信息办理入住", MenuKey = "roommanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roommanagement.cr", PermissionName = "退房操作", Module = "room", Description = "退房操作", MenuKey = "roommanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roommanagement.dbrn", PermissionName = "根据房间编号查询截止到今天住了多少天", Module = "room", Description = "根据房间编号查询截止到今天住了多少天", MenuKey = "roommanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roommanagement.deleteroom", PermissionName = "删除房间", Module = "room", Description = "删除房间", MenuKey = "roommanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roommanagement.export", PermissionName = "导出房间信息", Module = "room", Description = "导出房间信息列表", MenuKey = "roommanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roommanagement.insertroom", PermissionName = "添加房间", Module = "room", Description = "添加房间", MenuKey = "roommanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roommanagement.scura", PermissionName = "根据房间状态来查询可使用的房间", Module = "room", Description = "根据房间状态来查询可使用的房间", MenuKey = "roommanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roommanagement.scurabrs", PermissionName = "查询可入住房间数量", Module = "room", Description = "查询可入住房间数量", MenuKey = "roommanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roommanagement.sfrabrs", PermissionName = "查询维修房数量", Module = "room", Description = "查询维修房数量", MenuKey = "roommanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roommanagement.sncrabrs", PermissionName = "查询脏房数量", Module = "room", Description = "查询脏房数量", MenuKey = "roommanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roommanagement.snurabrs", PermissionName = "查询已入住房间数量", Module = "room", Description = "查询已入住房间数量", MenuKey = "roommanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roommanagement.srrabrs", PermissionName = "查询预约房数量", Module = "room", Description = "查询预约房数量", MenuKey = "roommanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roommanagement.sra", PermissionName = "获取所有房间信息", Module = "room", Description = "获取所有房间信息", MenuKey = "roommanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roommanagement.srbrn", PermissionName = "根据房间编号查询房间信息", Module = "room", Description = "根据房间编号查询房间信息", MenuKey = "roommanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roommanagement.srbrp", PermissionName = "根据房间编号查询房间价格", Module = "room", Description = "根据房间编号查询房间价格", MenuKey = "roommanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roommanagement.srbrs", PermissionName = "根据房间状态获取相应状态的房间信息", Module = "room", Description = "根据房间状态获取相应状态的房间信息", MenuKey = "roommanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roommanagement.srbtn", PermissionName = "获取房间分区的信息", Module = "room", Description = "获取房间分区的信息", MenuKey = "roommanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roommanagement.tr", PermissionName = "转房操作", Module = "room", Description = "转房操作", MenuKey = "roommanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roommanagement.updateroom", PermissionName = "更新房间", Module = "room", Description = "更新房间", MenuKey = "roommanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roommanagement.uri", PermissionName = "根据房间编号修改房间信息（入住）", Module = "room", Description = "根据房间编号修改房间信息（入住）", MenuKey = "roommanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roommanagement.uriwr", PermissionName = "根据房间编号修改房间信息（预约）", Module = "room", Description = "根据房间编号修改房间信息（预约）", MenuKey = "roommanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roommanagement.ursbrn", PermissionName = "根据房间编号更改房间状态", Module = "room", Description = "根据房间编号更改房间状态", MenuKey = "roommanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            // 房间配置管理
            new Permission { PermissionNumber = "roomconfig.drt", PermissionName = "删除房间配置", Module = "room", Description = "删除房间配置", MenuKey = "roomconfig", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roomconfig.export", PermissionName = "导出房间配置", Module = "room", Description = "导出房间配置列表", MenuKey = "roomconfig", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roomconfig.irt", PermissionName = "添加房间配置", Module = "room", Description = "添加房间配置", MenuKey = "roomconfig", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roomconfig.srtbrn", PermissionName = "根据房间编号查询房间类型名称", Module = "room", Description = "根据房间编号查询房间类型名称", MenuKey = "roomconfig", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roomconfig.srta", PermissionName = "获取所有房间类型", Module = "room", Description = "获取所有房间类型", MenuKey = "roomconfig", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "roomconfig.urt", PermissionName = "更新房间配置", Module = "room", Description = "更新房间配置", MenuKey = "roomconfig", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            // 预约信息管理
            new Permission { PermissionNumber = "resermanagement.dri", PermissionName = "删除预约信息", Module = "room", Description = "删除预约信息", MenuKey = "resermanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "resermanagement.export", PermissionName = "导出预约信息", Module = "room", Description = "导出预约信息列表", MenuKey = "resermanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "resermanagement.iri", PermissionName = "添加预约信息", Module = "room", Description = "添加预约信息", MenuKey = "resermanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "resermanagement.sra", PermissionName = "获取所有预约信息", Module = "room", Description = "获取所有预约信息", MenuKey = "resermanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "resermanagement.sribrn", PermissionName = "根据房间编号获取预约信息", Module = "room", Description = "根据房间编号获取预约信息", MenuKey = "resermanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "resermanagement.srta", PermissionName = "查询所有预约类型", Module = "room", Description = "查询所有预约类型", MenuKey = "resermanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "resermanagement.uri", PermissionName = "更新预约信息", Module = "room", Description = "更新预约信息", MenuKey = "resermanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },


            // Customer management (客户管理)
            // 会员等级规则管理
            new Permission { PermissionNumber = "viplevel.addviprule", PermissionName = "添加会员等级规则", Module = "customer", Description = "添加会员等级规则", MenuKey = "viplevel", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "viplevel.delviprule", PermissionName = "删除会员等级规则", Module = "customer", Description = "删除会员等级规则", MenuKey = "viplevel", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "viplevel.export", PermissionName = "导出会员等级规则", Module = "customer", Description = "导出会员等级规则列表", MenuKey = "viplevel", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "viplevel.svr", PermissionName = "查询会员等级规则", Module = "customer", Description = "查询会员等级规则", MenuKey = "viplevel", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "viplevel.svrlist", PermissionName = "查询会员等级规则列表", Module = "customer", Description = "查询会员等级规则列表", MenuKey = "viplevel", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "viplevel.updviprule", PermissionName = "更新会员等级规则", Module = "customer", Description = "更新会员等级规则", MenuKey = "viplevel", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            // 客户信息
            new Permission { PermissionNumber = "customer.dci", PermissionName = "删除客户信息", Module = "customer", Description = "删除客户信息", MenuKey = "customer", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "customer.export", PermissionName = "导出客户信息", Module = "customer", Description = "导出客户信息列表", MenuKey = "customer", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "customer.ici", PermissionName = "添加客户信息", Module = "customer", Description = "添加客户信息", MenuKey = "customer", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "customer.scbi", PermissionName = "查询指定客户信息", Module = "customer", Description = "查询指定客户信息", MenuKey = "customer", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "customer.scs", PermissionName = "查询所有客户信息", Module = "customer", Description = "查询所有客户信息", MenuKey = "customer", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "customer.uci", PermissionName = "更新客户信息", Module = "customer", Description = "更新客户信息", MenuKey = "customer", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "customer.uctbcn", PermissionName = "更新会员等级", Module = "customer", Description = "更新会员等级", MenuKey = "customer", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            // 客户消费信息
            new Permission { PermissionNumber = "customerspend.acs", PermissionName = "添加客户消费信息", Module = "customer", Description = "添加客户消费信息", MenuKey = "customerspend", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "customerspend.export", PermissionName = "导出客户消费信息", Module = "customer", Description = "导出客户消费信息列表", MenuKey = "customerspend", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "customerspend.ssbrn", PermissionName = "查询房间消费信息", Module = "customer", Description = "查询房间消费信息", MenuKey = "customerspend", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "customerspend.ssia", PermissionName = "查询所有消费信息", Module = "customer", Description = "查询所有消费信息", MenuKey = "customerspend", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "customerspend.shsia", PermissionName = "查询客户历史消费信息", Module = "customer", Description = "查询客户历史消费信息", MenuKey = "customerspend", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "customerspend.sca", PermissionName = "查询消费总金额", Module = "customer", Description = "查询消费总金额", MenuKey = "customerspend", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "customerspend.ucs", PermissionName = "撤回客户消费信息", Module = "customer", Description = "撤回客户消费信息", MenuKey = "customerspend", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "customerspend.usi", PermissionName = "更新消费信息", Module = "customer", Description = "更新消费信息", MenuKey = "customerspend", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            // 客户类型
            new Permission { PermissionNumber = "customertype.create", PermissionName = "新增客户类型", Module = "customer", Description = "基础信息-客户类型-新增", MenuKey = "customertype", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "customertype.delete", PermissionName = "删除客户类型", Module = "customer", Description = "基础信息-客户类型-删除", MenuKey = "customertype", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "customertype.export", PermissionName = "导出客户类型", Module = "customer", Description = "基础信息-客户类型-导出列表", MenuKey = "customertype", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "customertype.update", PermissionName = "更新客户类型", Module = "customer", Description = "基础信息-客户类型-更新", MenuKey = "customertype", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "customertype.view", PermissionName = "查询客户类型列表", Module = "customer", Description = "基础信息-客户类型-查询列表", MenuKey = "customertype", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },


            // Human resource (酒店人事管理)
            // 员工管理
            new Permission { PermissionNumber = "staffmanagement.ae", PermissionName = "添加员工信息", Module = "humanresource", Description = "添加员工信息", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "staffmanagement.export", PermissionName = "导出员工信息", Module = "humanresource", Description = "导出员工信息列表", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "staffmanagement.mea", PermissionName = "员工账号禁/启用", Module = "humanresource", Description = "员工账号禁/启用", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "staffmanagement.reap", PermissionName = "重置员工账号密码", Module = "humanresource", Description = "重置员工账号密码", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "staffmanagement.sea", PermissionName = "获取所有工作人员信息", Module = "humanresource", Description = "获取所有工作人员信息", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "staffmanagement.seibei", PermissionName = "根据登录名称查询员工信息", Module = "humanresource", Description = "根据登录名称查询员工信息", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "staffmanagement.ue", PermissionName = "修改员工信息", Module = "humanresource", Description = "修改员工信息", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            // 员工履历管理
            new Permission { PermissionNumber = "staffmanagement.shbei", PermissionName = "根据工号查询履历信息", Module = "humanresource", Description = "根据工号查询履历信息", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "staffmanagement.ahbei", PermissionName = "根据工号添加员工履历", Module = "humanresource", Description = "根据工号添加员工履历", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "staffmanagement.update", PermissionName = "根据工号更新员工履历", Module = "humanresource", Description = "根据工号更新员工履历", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            // 员工打卡管理
            new Permission { PermissionNumber = "staffmanagement.stcfobwn", PermissionName = "查询今天员工是否已签到", Module = "humanresource", Description = "查询今天员工是否已签到", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "staffmanagement.swcdsbei", PermissionName = "查询员工签到天数", Module = "humanresource", Description = "查询员工签到天数", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "staffmanagement.acfo", PermissionName = "添加员工打卡数据", Module = "humanresource", Description = "添加员工打卡数据", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "staffmanagement.scfobei", PermissionName = "根据员工编号查询其所有的打卡记录", Module = "humanresource", Description = "根据员工编号查询其所有的打卡记录", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            // 员工照片管理
            new Permission { PermissionNumber = "staffmanagement.ueap", PermissionName = "修改员工账号密码", Module = "humanresource", Description = "修改员工账号密码", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "staffmanagement.uwp", PermissionName = "更新员工照片", Module = "humanresource", Description = "更新员工照片", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "staffmanagement.dwp", PermissionName = "删除员工照片", Module = "humanresource", Description = "删除员工照片", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "staffmanagement.iwp", PermissionName = "添加员工照片", Module = "humanresource", Description = "添加员工照片", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "staffmanagement.ep", PermissionName = "查询员工照片", Module = "humanresource", Description = "查询员工照片", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            // 员工两步验证管理
            new Permission { PermissionNumber = "staffmanagement.dtf", PermissionName = "关闭当前员工账号 2FA", Module = "humanresource", Description = "关闭当前员工账号 2FA", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "staffmanagement.etf", PermissionName = "启用当前员工账号 2FA", Module = "humanresource", Description = "启用当前员工账号 2FA", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "staffmanagement.gtfs", PermissionName = "生成当前员工账号的 2FA 绑定信息", Module = "humanresource", Description = "生成当前员工账号的 2FA 绑定信息", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "staffmanagement.gtfse", PermissionName = "获取当前员工账号的 2FA 状态", Module = "humanresource", Description = "获取当前员工账号的 2FA 状态", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "staffmanagement.rtfrc", PermissionName = "重置当前员工账号恢复备用码", Module = "humanresource", Description = "重置当前员工账号恢复备用码", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            
            
            // Material management (酒店物资管理)
            // 商品管理
            new Permission { PermissionNumber = "goodsmanagement.dst", PermissionName = "删除商品信息", Module = "material", Description = "删除商品信息", MenuKey = "goodsmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "goodsmanagement.export", PermissionName = "导出商品信息", Module = "material", Description = "导出商品信息列表", MenuKey = "goodsmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "goodsmanagement.ist", PermissionName = "添加商品", Module = "material", Description = "添加商品", MenuKey = "goodsmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "goodsmanagement.ssta", PermissionName = "查询所有商品", Module = "material", Description = "查询所有商品", MenuKey = "goodsmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "goodsmanagement.sstbnap", PermissionName = "根据商品名称和价格查询商品编号", Module = "material", Description = "根据商品名称和价格查询商品编号", MenuKey = "goodsmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "goodsmanagement.ust", PermissionName = "修改商品", Module = "material", Description = "修改商品", MenuKey = "goodsmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },


            // Operation management (行为操作管理)
            // 操作日志
            new Permission { PermissionNumber = "operationlog.delete", PermissionName = "删除时间范围的操作日志", Module = "operation", Description = "删除时间范围的操作日志", MenuKey = "operationlog", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "operationlog.export", PermissionName = "导出操作日志", Module = "operation", Description = "导出操作日志列表", MenuKey = "operationlog", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "operationlog.view", PermissionName = "查询所有操作日志", Module = "operation", Description = "查询所有操作日志", MenuKey = "operationlog", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            // 请求日志
            new Permission { PermissionNumber = "requestlog.delete", PermissionName = "删除时间范围的请求日志", Module = "operation", Description = "删除时间范围的请求日志", MenuKey = "requestlog", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "requestlog.export", PermissionName = "导出请求日志", Module = "operation", Description = "导出请求日志列表", MenuKey = "requestlog", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "requestlog.view", PermissionName = "查询所有请求日志", Module = "operation", Description = "查询所有请求日志", MenuKey = "requestlog", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },


            // System management (系统管理)
            // 管理员管理
            new Permission { PermissionNumber = "system:admin:addadmin", PermissionName = "添加管理员", Module = "system", Description = "添加管理员", MenuKey = "administratormanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:admin:deladmin", PermissionName = "删除管理员", Module = "system", Description = "删除管理员", MenuKey = "administratormanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:admin:export", PermissionName = "导出管理员", Module = "system", Description = "导出管理员列表", MenuKey = "administratormanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:admin:gaal", PermissionName = "获取所有管理员列表", Module = "system", Description = "获取所有管理员列表", MenuKey = "administratormanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:admin:updadmin", PermissionName = "更新管理员", Module = "system", Description = "更新管理员", MenuKey = "administratormanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            // 管理员类型管理
            new Permission { PermissionNumber = "system:admintype:aat", PermissionName = "添加管理员类型", Module = "system", Description = "添加管理员类型", MenuKey = "admintypemanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:admintype:dat", PermissionName = "删除管理员类型", Module = "system", Description = "删除管理员类型", MenuKey = "admintypemanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:admintype:export", PermissionName = "导出管理员类型", Module = "system", Description = "导出管理员类型列表", MenuKey = "admintypemanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:admintype:gaat", PermissionName = "获取所有管理员类型", Module = "system", Description = "获取所有管理员类型", MenuKey = "admintypemanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:admintype:uat", PermissionName = "更新管理员类型", Module = "system", Description = "更新管理员类型", MenuKey = "admintypemanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            // 管理员两步验证管理
            new Permission { PermissionNumber = "system:admin:gtfs", PermissionName = "获取当前管理员账号的 2FA 状态", Module = "system", Description = "获取当前管理员账号的 2FA 状态", MenuKey = "administratormanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:admin:dtf", PermissionName = "关闭当前管理员账号 2FA", Module = "system", Description = "关闭当前管理员账号 2FA", MenuKey = "administratormanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:admin:etf", PermissionName = "启用当前管理员账号 2FA", Module = "system", Description = "启用当前管理员账号 2FA", MenuKey = "administratormanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:admin:gtfsu", PermissionName = "生成当前管理员账号的 2FA 绑定信息", Module = "system", Description = "生成当前管理员账号的 2FA 绑定信息", MenuKey = "administratormanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:admin:rtfrc", PermissionName = "重置当前管理员账号恢复备用码", Module = "system", Description = "重置当前管理员账号恢复备用码", MenuKey = "administratormanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            // 角色管理
            new Permission { PermissionNumber = "system:role:aru", PermissionName = "为角色分配管理员（全量覆盖）", Module = "system", Description = "为角色分配管理员（全量覆盖）", MenuKey = "rolemanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:role:deleterole", PermissionName = "删除角色", Module = "system", Description = "删除角色", MenuKey = "rolemanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:role:export", PermissionName = "导出角色", Module = "system", Description = "导出角色列表", MenuKey = "rolemanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:role:grp", PermissionName = "为角色授予权限（全量覆盖）", Module = "system", Description = "为角色授予权限（全量覆盖）", MenuKey = "rolemanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:role:insertrole", PermissionName = "添加角色", Module = "system", Description = "添加角色", MenuKey = "rolemanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:role:rrp", PermissionName = "读取指定角色已授予的权限编码集合", Module = "system", Description = "读取指定角色已授予的权限编码集合", MenuKey = "rolemanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:role:rrg", PermissionName = "读取指定角色菜单和权限授权（菜单与权限独立）", Module = "system", Description = "读取指定角色菜单和权限授权（菜单与权限独立）", MenuKey = "rolemanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:role:rru", PermissionName = "读取隶属于指定角色的管理员用户编码集合", Module = "system", Description = "读取隶属于指定角色的管理员用户编码集合", MenuKey = "rolemanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:role:srl", PermissionName = "查询角色列表", Module = "system", Description = "查询角色列表", MenuKey = "rolemanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:role:updaterole", PermissionName = "更新角色", Module = "system", Description = "更新角色", MenuKey = "rolemanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            // 菜单管理
            new Permission { PermissionNumber = "menumanagement.bma", PermissionName = "构建菜单树", Module = "system", Description = "构建菜单树", MenuKey = "menumanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "menumanagement.deletemenu", PermissionName = "删除菜单", Module = "system", Description = "删除菜单", MenuKey = "menumanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "menumanagement.export", PermissionName = "导出菜单", Module = "system", Description = "导出菜单列表", MenuKey = "menumanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "menumanagement.insertmenu", PermissionName = "插入菜单", Module = "system", Description = "插入菜单", MenuKey = "menumanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "menumanagement.sma", PermissionName = "查询所有菜单信息", Module = "system", Description = "查询所有菜单信息", MenuKey = "menumanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "menumanagement.updatemenu", PermissionName = "更新菜单", Module = "system", Description = "更新菜单", MenuKey = "menumanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:quartzjob:export", PermissionName = "导出Quartz任务", Module = "system", Description = "导出Quartz任务列表", MenuKey = "quartzjoblist", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },


            // 主页
            // 仪表盘
            new Permission { PermissionNumber = "dashboard.view", PermissionName = "仪表盘-查看", Module = "home", Description = "仪表盘-查看", MenuKey = "dashboard", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "home.view", PermissionName = "首页-查看", Module = "home", Description = "首页-查看", MenuKey = "home", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "dashboard.bs", PermissionName = "获取业务统计信息", Module = "dashboard", Description = "获取业务统计信息", MenuKey = "dashboard", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "dashboard.hrs", PermissionName = "获取人事统计信息", Module = "dashboard", Description = "获取人事统计信息", MenuKey = "dashboard", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "dashboard.ls", PermissionName = "获取后勤统计信息", Module = "dashboard", Description = "获取后勤统计信息", MenuKey = "dashboard", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "dashboard.rs", PermissionName = "获取房间统计信息", Module = "dashboard", Description = "获取房间统计信息", MenuKey = "dashboard", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },

            
            // 权限分配
            // 管理员-角色权限管理（网页端）
            new Permission { PermissionNumber = "system:user:admin.rudp", PermissionName = "读取指定用户的“直接权限”", Module = "system", Description = "读取指定用户的“直接权限”", MenuKey = "administratormanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:user:admin.rurp", PermissionName = "读取指定用户的“角色-权限”明细", Module = "system", Description = "读取指定用户的“角色-权限”明细", MenuKey = "administratormanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:user:admin.rur", PermissionName = "读取指定用户已分配的角色编码集合", Module = "system", Description = "读取指定用户已分配的角色编码集合", MenuKey = "administratormanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:user:admin:aup", PermissionName = "为指定用户分配“直接权限”", Module = "system", Description = "为指定用户分配“直接权限”", MenuKey = "administratormanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:user:admin:aur", PermissionName = "为用户分配角色（全量覆盖）", Module = "system", Description = "为用户分配角色（全量覆盖）", MenuKey = "administratormanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:admin:assign.spl", PermissionName = "查询权限列表（支持条件过滤与分页/忽略分页）", Module = "system", Description = "查询权限列表（支持条件过滤与分页/忽略分页）", MenuKey = "administratormanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            // 客户-角色权限管理（网页端）
            new Permission { PermissionNumber = "system:user:customer.rudp", PermissionName = "读取客户“直接权限”权限编码集合", Module = "system", Description = "读取客户“直接权限”权限编码集合", MenuKey = "customer", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:user:customer.rurp", PermissionName = "读取客户“角色-权限”明细", Module = "system", Description = "读取客户“角色-权限”明细", MenuKey = "customer", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:user:customer.rur", PermissionName = "读取客户已分配的角色编码集合", Module = "system", Description = "读取客户已分配的角色编码集合", MenuKey = "customer", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:user:customer:aup", PermissionName = "为客户分配“直接权限”", Module = "system", Description = "为客户分配“直接权限”", MenuKey = "customer", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:user:customer:aur", PermissionName = "为客户分配角色（全量覆盖）", Module = "system", Description = "为客户分配角色（全量覆盖）", MenuKey = "customer", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:customer:assign.spl", PermissionName = "查询权限列表（支持条件过滤与分页/忽略分页）", Module = "system", Description = "查询权限列表（支持条件过滤与分页/忽略分页）", MenuKey = "customer", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            // 员工-角色权限管理（网页端）
            new Permission { PermissionNumber = "system:user:employee.rudp", PermissionName = "读取员工“直接权限”权限编码集合", Module = "system", Description = "读取员工“直接权限”权限编码集合", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:user:employee.rurp", PermissionName = "读取员工“角色-权限”明细", Module = "system", Description = "读取员工“角色-权限”明细", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:user:employee.rur", PermissionName = "读取员工已分配的角色编码集合", Module = "system", Description = "读取员工已分配的角色编码集合", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:user:employee:aup", PermissionName = "为员工分配“直接权限”", Module = "system", Description = "为员工分配“直接权限”", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:user:employee:aur", PermissionName = "为员工分配角色（全量覆盖）", Module = "system", Description = "为员工分配角色（全量覆盖）", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },
            new Permission { PermissionNumber = "system:employee:assign.spl", PermissionName = "查询权限列表（支持条件过滤与分页/忽略分页）", Module = "system", Description = "查询权限列表（支持条件过滤与分页/忽略分页）", MenuKey = "staffmanagement", ParentNumber = null, DataInsUsr = "System", DataInsDate = DateTime.Now },

        };

        public Type[] EntityTypes => entityTypes;

        public List<object> GetEntityDatas() => entityDatas;
    }
}
