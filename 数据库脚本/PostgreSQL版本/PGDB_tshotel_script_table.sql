/*
 Navicat Premium Dump SQL

 Source Server         : PG 13.0
 Source Server Type    : PostgreSQL
 Source Server Version : 130020 (130020)
 Source Host           : localhost:5432
 Source Catalog        : tshoteldb
 Source Schema         : public

 Target Server Type    : PostgreSQL
 Target Server Version : 130020 (130020)
 File Encoding         : 65001

 Date: 26/04/2025 17:52:48
*/


-- ----------------------------
-- Table structure for administrator
-- ----------------------------
DROP TABLE IF EXISTS "public"."administrator";
CREATE TABLE "public"."administrator" (
  "id" int4 NOT NULL DEFAULT nextval('administrator_id_seq'::regclass),
  "admin_number" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "admin_account" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "admin_password" varchar(256) COLLATE "pg_catalog"."default" NOT NULL,
  "admin_type" varchar(150) COLLATE "pg_catalog"."default" NOT NULL,
  "admin_name" varchar(200) COLLATE "pg_catalog"."default" NOT NULL,
  "is_admin" int4 NOT NULL,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."administrator"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."administrator"."admin_number" IS '管理员账号 (Administrator Account)';
COMMENT ON COLUMN "public"."administrator"."admin_account" IS '管理员名称 (Administrator Name)';
COMMENT ON COLUMN "public"."administrator"."admin_password" IS '管理员密码 (Administrator Password)';
COMMENT ON COLUMN "public"."administrator"."admin_type" IS '管理员类型 (Administrator Type)';
COMMENT ON COLUMN "public"."administrator"."admin_name" IS '管理员名称 (Administrator Name)';
COMMENT ON COLUMN "public"."administrator"."is_admin" IS '是否为超级管理员 (Is Super Administrator)';
COMMENT ON COLUMN "public"."administrator"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."administrator"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."administrator"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."administrator"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."administrator"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."administrator" IS '管理员实体类 (Administrator Entity)';

-- ----------------------------
-- Table structure for administrator_type
-- ----------------------------
DROP TABLE IF EXISTS "public"."administrator_type";
CREATE TABLE "public"."administrator_type" (
  "id" int4 NOT NULL DEFAULT nextval('administrator_type_id_seq'::regclass),
  "type_id" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "type_name" varchar(256) COLLATE "pg_catalog"."default" NOT NULL,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."administrator_type"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."administrator_type"."type_id" IS '类型编号 (Type ID)';
COMMENT ON COLUMN "public"."administrator_type"."type_name" IS '类型名称 (Type Name)';
COMMENT ON COLUMN "public"."administrator_type"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."administrator_type"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."administrator_type"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."administrator_type"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."administrator_type"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."administrator_type" IS '管理员类型 (Administrator Type)';

-- ----------------------------
-- Table structure for app_banner
-- ----------------------------
DROP TABLE IF EXISTS "public"."app_banner";
CREATE TABLE "public"."app_banner" (
  "id" int4 NOT NULL DEFAULT nextval('app_banner_id_seq'::regclass),
  "banner_content" varchar(2000) COLLATE "pg_catalog"."default" NOT NULL,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."app_banner"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."app_banner"."banner_content" IS '宣传内容（支持富文本） (Promotion Content with Rich Text)';
COMMENT ON COLUMN "public"."app_banner"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."app_banner"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."app_banner"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."app_banner"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."app_banner"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."app_banner" IS 'APP横幅配置表 (APP Banner Configuration)';

-- ----------------------------
-- Table structure for app_config_base
-- ----------------------------
DROP TABLE IF EXISTS "public"."app_config_base";
CREATE TABLE "public"."app_config_base" (
  "id" int4 NOT NULL DEFAULT nextval('app_config_base_id_seq'::regclass),
  "url_no" int4 NOT NULL,
  "url_addr" varchar(256) COLLATE "pg_catalog"."default" NOT NULL
)
;
COMMENT ON COLUMN "public"."app_config_base"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."app_config_base"."url_no" IS '地址编号 (URL Number)';
COMMENT ON COLUMN "public"."app_config_base"."url_addr" IS '地址 (URL Address)';
COMMENT ON TABLE "public"."app_config_base" IS '系统信息 (System Information)';

-- ----------------------------
-- Table structure for app_version
-- ----------------------------
DROP TABLE IF EXISTS "public"."app_version";
CREATE TABLE "public"."app_version" (
  "base_versionid" int4 NOT NULL DEFAULT nextval('app_version_base_versionid_seq'::regclass),
  "base_version" varchar(100) COLLATE "pg_catalog"."default" NOT NULL
)
;
COMMENT ON COLUMN "public"."app_version"."base_versionid" IS '流水号 (Sequence Number)';
COMMENT ON COLUMN "public"."app_version"."base_version" IS '版本号 (Version Number)';
COMMENT ON TABLE "public"."app_version" IS '应用版本 (Application Version)';

-- ----------------------------
-- Table structure for appointment_notice
-- ----------------------------
DROP TABLE IF EXISTS "public"."appointment_notice";
CREATE TABLE "public"."appointment_notice" (
  "id" int4 NOT NULL DEFAULT nextval('appointment_notice_id_seq'::regclass),
  "notice_no" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "notice_theme" varchar(256) COLLATE "pg_catalog"."default" NOT NULL,
  "notice_type" varchar(150) COLLATE "pg_catalog"."default" NOT NULL,
  "notice_time" timestamp(6) NOT NULL,
  "notice_content" text COLLATE "pg_catalog"."default",
  "notice_department" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."appointment_notice"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."appointment_notice"."notice_no" IS '公告编号 (AppointmentNotice Number)';
COMMENT ON COLUMN "public"."appointment_notice"."notice_theme" IS '公告主题 (AppointmentNotice Theme)';
COMMENT ON COLUMN "public"."appointment_notice"."notice_type" IS '公告类型 (AppointmentNotice Type)';
COMMENT ON COLUMN "public"."appointment_notice"."notice_time" IS '公告时间 (AppointmentNotice Time)';
COMMENT ON COLUMN "public"."appointment_notice"."notice_content" IS '公告正文 (AppointmentNotice Content)';
COMMENT ON COLUMN "public"."appointment_notice"."notice_department" IS '发文部门 (Issuing Department)';
COMMENT ON COLUMN "public"."appointment_notice"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."appointment_notice"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."appointment_notice"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."appointment_notice"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."appointment_notice"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."appointment_notice" IS '任命公告 (Appointment AppointmentNotice)';

-- ----------------------------
-- Table structure for asset
-- ----------------------------
DROP TABLE IF EXISTS "public"."asset";
CREATE TABLE "public"."asset" (
  "id" int4 NOT NULL DEFAULT nextval('asset_id_seq'::regclass),
  "asset_number" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "asset_name" varchar(200) COLLATE "pg_catalog"."default" NOT NULL,
  "asset_value" numeric(18,2) NOT NULL,
  "department_code" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "acquisition_date" timestamp(6) NOT NULL,
  "asset_source" varchar(500) COLLATE "pg_catalog"."default" NOT NULL,
  "acquired_by_employee" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."asset"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."asset"."asset_number" IS '资产编号';
COMMENT ON COLUMN "public"."asset"."asset_name" IS '资产名称';
COMMENT ON COLUMN "public"."asset"."asset_value" IS '资产名称';
COMMENT ON COLUMN "public"."asset"."department_code" IS '所属部门代码 (Department Code)';
COMMENT ON COLUMN "public"."asset"."acquisition_date" IS '入库时间 (购置日期) (Acquisition Date)';
COMMENT ON COLUMN "public"."asset"."asset_source" IS '资产来源 (Asset Source)';
COMMENT ON COLUMN "public"."asset"."acquired_by_employee" IS '资产经办人 (员工ID) (Acquired By - Employee ID)';
COMMENT ON COLUMN "public"."asset"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."asset"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."asset"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."asset"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."asset"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."asset" IS '资产管理';

-- ----------------------------
-- Table structure for card_code
-- ----------------------------
DROP TABLE IF EXISTS "public"."card_code";
CREATE TABLE "public"."card_code" (
  "id" int4 NOT NULL,
  "province" varchar(100) COLLATE "pg_catalog"."default" NOT NULL,
  "city" varchar(100) COLLATE "pg_catalog"."default",
  "district" varchar(100) COLLATE "pg_catalog"."default",
  "district_code" varchar(50) COLLATE "pg_catalog"."default" NOT NULL,
  "delete_mk" int4 NOT NULL,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default",
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default",
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."card_code"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."card_code"."province" IS '省份 (Province)';
COMMENT ON COLUMN "public"."card_code"."city" IS '城市 (City)';
COMMENT ON COLUMN "public"."card_code"."district" IS '地区 (District)';
COMMENT ON COLUMN "public"."card_code"."district_code" IS '地区识别码 (Area Code)';
COMMENT ON COLUMN "public"."card_code"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."card_code"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."card_code"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."card_code"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."card_code"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."card_code" IS '卡片代码 (Card Codes)';

-- ----------------------------
-- Table structure for custo_type
-- ----------------------------
DROP TABLE IF EXISTS "public"."custo_type";
CREATE TABLE "public"."custo_type" (
  "id" int4 NOT NULL DEFAULT nextval('custo_type_id_seq'::regclass),
  "custo_type" int4 NOT NULL,
  "type_name" varchar(256) COLLATE "pg_catalog"."default" NOT NULL,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."custo_type"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."custo_type"."custo_type" IS '客户类型 (Customer Type)';
COMMENT ON COLUMN "public"."custo_type"."type_name" IS '客户类型名称 (Customer Type Name)';
COMMENT ON COLUMN "public"."custo_type"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."custo_type"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."custo_type"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."custo_type"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."custo_type"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."custo_type" IS '客户类型';

-- ----------------------------
-- Table structure for customer
-- ----------------------------
DROP TABLE IF EXISTS "public"."customer";
CREATE TABLE "public"."customer" (
  "id" int4 NOT NULL DEFAULT nextval('customer_id_seq'::regclass),
  "custo_no" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "custo_name" varchar(250) COLLATE "pg_catalog"."default" NOT NULL,
  "custo_gender" int4 NOT NULL,
  "passport_type" int4 NOT NULL,
  "custo_tel" varchar(256) COLLATE "pg_catalog"."default" NOT NULL,
  "custo_birth" date NOT NULL,
  "passport_id" varchar(256) COLLATE "pg_catalog"."default" NOT NULL,
  "custo_address" varchar(256) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "custo_type" int4 NOT NULL,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."customer"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."customer"."custo_no" IS '客户编号 (Customer Number)';
COMMENT ON COLUMN "public"."customer"."custo_name" IS '客户名称 (Customer Name)';
COMMENT ON COLUMN "public"."customer"."custo_gender" IS '客户性别 (Customer Gender)';
COMMENT ON COLUMN "public"."customer"."passport_type" IS '客户性别 (Customer Gender)';
COMMENT ON COLUMN "public"."customer"."custo_tel" IS '客户电话 (Customer Phone Number)';
COMMENT ON COLUMN "public"."customer"."custo_birth" IS '出生日期 (Date of Birth)';
COMMENT ON COLUMN "public"."customer"."passport_id" IS '证件号码 (Passport ID)';
COMMENT ON COLUMN "public"."customer"."custo_address" IS '居住地址 (Customer Address)';
COMMENT ON COLUMN "public"."customer"."custo_type" IS '客户类型 (Customer Type)';
COMMENT ON COLUMN "public"."customer"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."customer"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."customer"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."customer"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."customer"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."customer" IS '客户信息';

-- ----------------------------
-- Table structure for customer_spend
-- ----------------------------
DROP TABLE IF EXISTS "public"."customer_spend";
CREATE TABLE "public"."customer_spend" (
  "spend_number" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "room_no" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "custo_no" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "product_number" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "spend_name" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "apend_quantity" int4 NOT NULL,
  "spend_price" numeric NOT NULL,
  "spend_amount" numeric NOT NULL,
  "spend_time" timestamp(6) NOT NULL,
  "settlement_status" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."customer_spend"."spend_number" IS '消费编号 (Consumption Number)';
COMMENT ON COLUMN "public"."customer_spend"."room_no" IS '房间编号 (Room Number)';
COMMENT ON COLUMN "public"."customer_spend"."custo_no" IS '客户编号 (Customer Number)';
COMMENT ON COLUMN "public"."customer_spend"."product_number" IS '商品编号 (Product Number)';
COMMENT ON COLUMN "public"."customer_spend"."spend_name" IS '商品名称 (Product Name)';
COMMENT ON COLUMN "public"."customer_spend"."apend_quantity" IS '消费数量 (Consumption Quantity)';
COMMENT ON COLUMN "public"."customer_spend"."spend_price" IS '商品单价 (Product Price)';
COMMENT ON COLUMN "public"."customer_spend"."spend_amount" IS '消费金额 (Consumption Amount)';
COMMENT ON COLUMN "public"."customer_spend"."spend_time" IS '消费时间 (Consumption Time)';
COMMENT ON COLUMN "public"."customer_spend"."settlement_status" IS '结算状态 (Settlement Status)';
COMMENT ON COLUMN "public"."customer_spend"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."customer_spend"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."customer_spend"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."customer_spend"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."customer_spend"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."customer_spend" IS '消费信息 (Consumption Information)';

-- ----------------------------
-- Table structure for department
-- ----------------------------
DROP TABLE IF EXISTS "public"."department";
CREATE TABLE "public"."department" (
  "id" int4 NOT NULL DEFAULT nextval('department_id_seq'::regclass),
  "dept_no" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "dept_name" varchar(256) COLLATE "pg_catalog"."default" NOT NULL,
  "dept_desc" varchar(500) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "dept_date" timestamp(6),
  "dept_leader" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "dept_parent" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."department"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."department"."dept_no" IS '部门编号 (Department Number)';
COMMENT ON COLUMN "public"."department"."dept_name" IS '部门名称 (Department Name)';
COMMENT ON COLUMN "public"."department"."dept_desc" IS '部门描述 (Department Description)';
COMMENT ON COLUMN "public"."department"."dept_date" IS '创建时间(部门) (Department Creation Date)';
COMMENT ON COLUMN "public"."department"."dept_leader" IS '部门主管 (Department Leader)';
COMMENT ON COLUMN "public"."department"."dept_parent" IS '上级部门编号 (Parent Department Number)';
COMMENT ON COLUMN "public"."department"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."department"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."department"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."department"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."department"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."department" IS '部门表 (Department Table)';

-- ----------------------------
-- Table structure for employee
-- ----------------------------
DROP TABLE IF EXISTS "public"."employee";
CREATE TABLE "public"."employee" (
  "id" int4 NOT NULL DEFAULT nextval('employee_id_seq'::regclass),
  "employee_number" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "employee_name" varchar(250) COLLATE "pg_catalog"."default" NOT NULL,
  "employee_date_of_birth" date NOT NULL,
  "employee_gender" int4 NOT NULL,
  "employee_nation" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "employee_tel" varchar(256) COLLATE "pg_catalog"."default" NOT NULL,
  "employee_department" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "employee_address" varchar(500) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "employee_postion" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "card_type" int4 NOT NULL,
  "card_number" varchar(256) COLLATE "pg_catalog"."default" NOT NULL,
  "employee_password" varchar(256) COLLATE "pg_catalog"."default" NOT NULL,
  "hire_time" date NOT NULL,
  "employee_political" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "employee_quality" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "enable_mk" int4 NOT NULL,
  "email_address" varchar(256) COLLATE "pg_catalog"."default" NOT NULL,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."employee"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."employee"."employee_number" IS '员工账号/工号 (Employee Account/ID)';
COMMENT ON COLUMN "public"."employee"."employee_name" IS '员工姓名 (Employee Name)';
COMMENT ON COLUMN "public"."employee"."employee_date_of_birth" IS '出生日期 (Date of Birth)';
COMMENT ON COLUMN "public"."employee"."employee_gender" IS '员工性别 (Employee Gender)';
COMMENT ON COLUMN "public"."employee"."employee_nation" IS '民族类型 (Ethnicity)';
COMMENT ON COLUMN "public"."employee"."employee_tel" IS '员工电话 (Employee Phone Number)';
COMMENT ON COLUMN "public"."employee"."employee_department" IS '所属部门 (Department)';
COMMENT ON COLUMN "public"."employee"."employee_address" IS '居住地址 (Residential Address)';
COMMENT ON COLUMN "public"."employee"."employee_postion" IS '员工职位 (Employee Position)';
COMMENT ON COLUMN "public"."employee"."card_type" IS '证件类型 (ID Card Type)';
COMMENT ON COLUMN "public"."employee"."card_number" IS '证件号码 (ID Card Number)';
COMMENT ON COLUMN "public"."employee"."employee_password" IS '员工密码 (Employee Password)';
COMMENT ON COLUMN "public"."employee"."hire_time" IS '员工入职时间 (Hire Date)';
COMMENT ON COLUMN "public"."employee"."employee_political" IS '员工面貌 (Political Affiliation)';
COMMENT ON COLUMN "public"."employee"."employee_quality" IS '教育程度 (Education Level)';
COMMENT ON COLUMN "public"."employee"."enable_mk" IS '禁用标记';
COMMENT ON COLUMN "public"."employee"."email_address" IS '邮箱地址';
COMMENT ON COLUMN "public"."employee"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."employee"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."employee"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."employee"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."employee"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."employee" IS '员工信息 (Employee Information)';

-- ----------------------------
-- Table structure for employee_check
-- ----------------------------
DROP TABLE IF EXISTS "public"."employee_check";
CREATE TABLE "public"."employee_check" (
  "id" int4 NOT NULL DEFAULT nextval('employee_check_id_seq'::regclass),
  "check_number" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "employee_number" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "check_time" timestamp(6) NOT NULL,
  "check_way" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "check_state" int4 NOT NULL,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."employee_check"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."employee_check"."check_number" IS '打卡编号';
COMMENT ON COLUMN "public"."employee_check"."employee_number" IS '员工工号 (Employee ID)';
COMMENT ON COLUMN "public"."employee_check"."check_time" IS '打卡时间 (Check-in/Check-out Time)';
COMMENT ON COLUMN "public"."employee_check"."check_way" IS '打卡方式 (Check-in/Check-out Method)';
COMMENT ON COLUMN "public"."employee_check"."check_state" IS '打卡状态 (Check-in/Check-out Status)';
COMMENT ON COLUMN "public"."employee_check"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."employee_check"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."employee_check"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."employee_check"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."employee_check"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."employee_check" IS '员工打卡考勤 (Employee Check-in/Check-out Record)';

-- ----------------------------
-- Table structure for employee_history
-- ----------------------------
DROP TABLE IF EXISTS "public"."employee_history";
CREATE TABLE "public"."employee_history" (
  "id" int4 NOT NULL DEFAULT nextval('employee_history_id_seq'::regclass),
  "history_number" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "employee_number" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "start_date" date NOT NULL,
  "end_date" date NOT NULL,
  "position" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "company" varchar(256) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."employee_history"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."employee_history"."history_number" IS '履历编号';
COMMENT ON COLUMN "public"."employee_history"."employee_number" IS '员工工号 (Employee ID)';
COMMENT ON COLUMN "public"."employee_history"."start_date" IS '开始时间 (Start Date)';
COMMENT ON COLUMN "public"."employee_history"."end_date" IS '结束时间 (End Date)';
COMMENT ON COLUMN "public"."employee_history"."position" IS '职位 (Position)';
COMMENT ON COLUMN "public"."employee_history"."company" IS '公司 (Company)';
COMMENT ON COLUMN "public"."employee_history"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."employee_history"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."employee_history"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."employee_history"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."employee_history"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."employee_history" IS '员工履历 (Employee History)';

-- ----------------------------
-- Table structure for employee_pic
-- ----------------------------
DROP TABLE IF EXISTS "public"."employee_pic";
CREATE TABLE "public"."employee_pic" (
  "id" int4 NOT NULL DEFAULT nextval('employee_pic_id_seq'::regclass),
  "employee_number" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "pic_url" varchar(256) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."employee_pic"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."employee_pic"."employee_number" IS '员工工号 (Employee ID)';
COMMENT ON COLUMN "public"."employee_pic"."pic_url" IS '照片路径 (Photo Path)';
COMMENT ON COLUMN "public"."employee_pic"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."employee_pic"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."employee_pic"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."employee_pic"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."employee_pic"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."employee_pic" IS '员工照片 (Employee Photo)';

-- ----------------------------
-- Table structure for energy_management
-- ----------------------------
DROP TABLE IF EXISTS "public"."energy_management";
CREATE TABLE "public"."energy_management" (
  "id" int4 NOT NULL DEFAULT nextval('energy_management_id_seq'::regclass),
  "information_number" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "room_no" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "use_date" timestamp(6) NOT NULL,
  "end_date" timestamp(6) NOT NULL,
  "water_use" numeric(18,2) NOT NULL,
  "power_use" numeric(18,2) NOT NULL,
  "recorder" varchar(150) COLLATE "pg_catalog"."default" NOT NULL DEFAULT 'Administrator'::character varying,
  "custo_no" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."energy_management"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."energy_management"."information_number" IS '信息编号 (Information ID)';
COMMENT ON COLUMN "public"."energy_management"."room_no" IS '房间编号 (Room Number)';
COMMENT ON COLUMN "public"."energy_management"."use_date" IS '开始使用时间 (Start Date)';
COMMENT ON COLUMN "public"."energy_management"."end_date" IS '结束使用时间 (End Date)';
COMMENT ON COLUMN "public"."energy_management"."water_use" IS '水费 (Water Usage)';
COMMENT ON COLUMN "public"."energy_management"."power_use" IS '电费 (Power Usage)';
COMMENT ON COLUMN "public"."energy_management"."recorder" IS '记录员 (Recorder)';
COMMENT ON COLUMN "public"."energy_management"."custo_no" IS '客户编号 (Customer Number)';
COMMENT ON COLUMN "public"."energy_management"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."energy_management"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."energy_management"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."energy_management"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."energy_management"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."energy_management" IS '水电信息';

-- ----------------------------
-- Table structure for menu
-- ----------------------------
DROP TABLE IF EXISTS "public"."menu";
CREATE TABLE "public"."menu" (
  "id" int4 NOT NULL DEFAULT nextval('menu_id_seq'::regclass),
  "key" varchar(256) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "title" varchar(256) COLLATE "pg_catalog"."default" NOT NULL,
  "path" text COLLATE "pg_catalog"."default" NOT NULL,
  "parent" int4,
  "icon" varchar(256) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."menu"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."menu"."key" IS '菜单键 (Menu Key)';
COMMENT ON COLUMN "public"."menu"."title" IS '菜单标题 (Menu Title)';
COMMENT ON COLUMN "public"."menu"."path" IS '菜单路径 (Menu Path)';
COMMENT ON COLUMN "public"."menu"."parent" IS '父级ID (Parent ID)';
COMMENT ON COLUMN "public"."menu"."icon" IS '图标';
COMMENT ON COLUMN "public"."menu"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."menu"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."menu"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."menu"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."menu"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."menu" IS '菜单表 (Menu Table)';

-- ----------------------------
-- Table structure for nation
-- ----------------------------
DROP TABLE IF EXISTS "public"."nation";
CREATE TABLE "public"."nation" (
  "id" int4 NOT NULL DEFAULT nextval('nation_id_seq'::regclass),
  "nation_no" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "nation_name" varchar(50) COLLATE "pg_catalog"."default" NOT NULL,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."nation"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."nation"."nation_no" IS '民族编号 (Nation Number)';
COMMENT ON COLUMN "public"."nation"."nation_name" IS '民族名称 (Nation Name)';
COMMENT ON COLUMN "public"."nation"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."nation"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."nation"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."nation"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."nation"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."nation" IS '民族信息表 (Nation Information)';

-- ----------------------------
-- Table structure for nav_bar
-- ----------------------------
DROP TABLE IF EXISTS "public"."nav_bar";
CREATE TABLE "public"."nav_bar" (
  "id" int4 NOT NULL DEFAULT nextval('nav_bar_id_seq'::regclass),
  "nav_name" varchar(50) COLLATE "pg_catalog"."default" NOT NULL,
  "nav_or" int4 NOT NULL DEFAULT 0,
  "nav_pic" varchar(255) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "nav_event" varchar(200) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "margin_left" int4 NOT NULL DEFAULT 0,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."nav_bar"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."nav_bar"."nav_name" IS '导航控件名称 (Navigation Bar Name)';
COMMENT ON COLUMN "public"."nav_bar"."nav_or" IS '导航控件排序 (Navigation Bar Order)';
COMMENT ON COLUMN "public"."nav_bar"."nav_pic" IS '导航控件图片路径 (Navigation Bar Image Path)';
COMMENT ON COLUMN "public"."nav_bar"."nav_event" IS '导航控件事件标识 (Navigation Bar Event Identifier)';
COMMENT ON COLUMN "public"."nav_bar"."margin_left" IS '导航控件左边距像素值 (Margin Left in Pixels)';
COMMENT ON COLUMN "public"."nav_bar"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."nav_bar"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."nav_bar"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."nav_bar"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."nav_bar"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."nav_bar" IS '导航栏配置表 (Navigation Bar Configuration)';

-- ----------------------------
-- Table structure for operation_log
-- ----------------------------
DROP TABLE IF EXISTS "public"."operation_log";
CREATE TABLE "public"."operation_log" (
  "id" int4 NOT NULL DEFAULT nextval('operation_log_id_seq'::regclass),
  "operation_number" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "operation_time" timestamp(6) NOT NULL,
  "log_content" varchar(2000) COLLATE "pg_catalog"."default" NOT NULL,
  "operation_account" varchar(50) COLLATE "pg_catalog"."default" NOT NULL,
  "operation_level" int4 NOT NULL,
  "software_version" varchar(50) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "login_ip" varchar(45) COLLATE "pg_catalog"."default" NOT NULL,
  "request_path" varchar(500) COLLATE "pg_catalog"."default" NOT NULL,
  "query_string" varchar(2000) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "elapsed_time" int8 NOT NULL DEFAULT '0'::bigint,
  "http_method" varchar(10) COLLATE "pg_catalog"."default" NOT NULL,
  "status_code" int4 NOT NULL DEFAULT 200,
  "exception_message" varchar(2000) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "exception_stacktrace" varchar(4000) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."operation_log"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."operation_log"."operation_number" IS '日志ID (Log ID)';
COMMENT ON COLUMN "public"."operation_log"."operation_time" IS '操作时间 (Operation Time)';
COMMENT ON COLUMN "public"."operation_log"."log_content" IS '操作信息 (Log Content)';
COMMENT ON COLUMN "public"."operation_log"."operation_account" IS '操作账号 (Operation Account)';
COMMENT ON COLUMN "public"."operation_log"."operation_level" IS '日志等级枚举值 (Log Level Enum)';
COMMENT ON COLUMN "public"."operation_log"."software_version" IS '软件版本号 (Software Version)';
COMMENT ON COLUMN "public"."operation_log"."login_ip" IS '登录IP地址 (Login IP Address)';
COMMENT ON COLUMN "public"."operation_log"."request_path" IS '请求路径 (Request Path)';
COMMENT ON COLUMN "public"."operation_log"."query_string" IS '查询参数 (Query String)';
COMMENT ON COLUMN "public"."operation_log"."elapsed_time" IS '响应时间（毫秒） (Elapsed Time in ms)';
COMMENT ON COLUMN "public"."operation_log"."http_method" IS 'HTTP请求方法 (HTTP Method)';
COMMENT ON COLUMN "public"."operation_log"."status_code" IS 'HTTP状态码 (HTTP Status Code)';
COMMENT ON COLUMN "public"."operation_log"."exception_message" IS '异常消息内容 (Exception Message)';
COMMENT ON COLUMN "public"."operation_log"."exception_stacktrace" IS '异常堆栈信息 (Exception Stack Trace)';
COMMENT ON COLUMN "public"."operation_log"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."operation_log"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."operation_log"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."operation_log"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."operation_log"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."operation_log" IS '操作日志表 (Operation Log)';

-- ----------------------------
-- Table structure for passport_type
-- ----------------------------
DROP TABLE IF EXISTS "public"."passport_type";
CREATE TABLE "public"."passport_type" (
  "passport_number" int4 NOT NULL,
  "passport_name" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."passport_type"."passport_number" IS '证件类型';
COMMENT ON COLUMN "public"."passport_type"."passport_name" IS '证件名称';
COMMENT ON COLUMN "public"."passport_type"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."passport_type"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."passport_type"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."passport_type"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."passport_type"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."passport_type" IS '证件类型';

-- ----------------------------
-- Table structure for position
-- ----------------------------
DROP TABLE IF EXISTS "public"."position";
CREATE TABLE "public"."position" (
  "id" int4 NOT NULL DEFAULT nextval('position_id_seq'::regclass),
  "position_no" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "position_name" varchar(200) COLLATE "pg_catalog"."default" NOT NULL,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."position"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."position"."position_no" IS '职位编号 (Position Number)';
COMMENT ON COLUMN "public"."position"."position_name" IS '职位名称 (Position Name)';
COMMENT ON COLUMN "public"."position"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."position"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."position"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."position"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."position"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."position" IS '职位信息表 (Position Information)';

-- ----------------------------
-- Table structure for qualification
-- ----------------------------
DROP TABLE IF EXISTS "public"."qualification";
CREATE TABLE "public"."qualification" (
  "id" int4 NOT NULL DEFAULT nextval('qualification_id_seq'::regclass),
  "education_no" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "education_name" varchar(200) COLLATE "pg_catalog"."default" NOT NULL,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."qualification"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."qualification"."education_no" IS '学历编号 (Education Number)';
COMMENT ON COLUMN "public"."qualification"."education_name" IS '学历名称 (Education Name)';
COMMENT ON COLUMN "public"."qualification"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."qualification"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."qualification"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."qualification"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."qualification"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."qualification" IS '学历 (Education)';

-- ----------------------------
-- Table structure for reser
-- ----------------------------
DROP TABLE IF EXISTS "public"."reser";
CREATE TABLE "public"."reser" (
  "id" int4 NOT NULL DEFAULT nextval('reser_id_seq'::regclass),
  "reser_number" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "custo_name" varchar(200) COLLATE "pg_catalog"."default" NOT NULL,
  "custo_tel" varchar(20) COLLATE "pg_catalog"."default" NOT NULL,
  "reser_way" varchar(50) COLLATE "pg_catalog"."default" NOT NULL,
  "reser_room" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "reser_date" timestamp(6) NOT NULL,
  "reser_end_date" timestamp(6) NOT NULL,
  "reser_status" int4 NOT NULL DEFAULT 0,
  "remarks" varchar(500) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."reser"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."reser"."reser_number" IS '预约唯一编号 (Unique Reservation ID)';
COMMENT ON COLUMN "public"."reser"."custo_name" IS '客户姓名 (Customer Full Name)';
COMMENT ON COLUMN "public"."reser"."custo_tel" IS '客户联系电话 (Contact Phone Number)';
COMMENT ON COLUMN "public"."reser"."reser_way" IS '预约来源渠道 (如官网/APP/第三方平台)';
COMMENT ON COLUMN "public"."reser"."reser_room" IS '预定房间编号 (关联房间表)';
COMMENT ON COLUMN "public"."reser"."reser_date" IS '入住日期（格式：yyyy-MM-dd） (Check-In Date)';
COMMENT ON COLUMN "public"."reser"."reser_end_date" IS '离店日期（格式：yyyy-MM-dd） (Check-Out Date)';
COMMENT ON COLUMN "public"."reser"."reser_status" IS '预约状态（0-待确认/1-已确认/2-已取消）';
COMMENT ON COLUMN "public"."reser"."remarks" IS '特殊需求备注 (Special Requests)';
COMMENT ON COLUMN "public"."reser"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."reser"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."reser"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."reser"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."reser"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."reser" IS '预约信息表 (Reservation Information)';

-- ----------------------------
-- Table structure for reward_punishment
-- ----------------------------
DROP TABLE IF EXISTS "public"."reward_punishment";
CREATE TABLE "public"."reward_punishment" (
  "id" int4 NOT NULL DEFAULT nextval('reward_punishment_id_seq'::regclass),
  "reward_punishment_id" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "employee_number" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "reward_punishment_information" varchar(256) COLLATE "pg_catalog"."default" NOT NULL,
  "reward_punishment_type" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "reward_punishment_operator" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "reward_punishment_time" timestamp(6) NOT NULL,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."reward_punishment"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."reward_punishment"."reward_punishment_id" IS '奖惩编号 (Reward/Punishment ID)';
COMMENT ON COLUMN "public"."reward_punishment"."employee_number" IS '员工工号 (Employee ID)';
COMMENT ON COLUMN "public"."reward_punishment"."reward_punishment_information" IS '奖惩信息 (Reward/Punishment Information)';
COMMENT ON COLUMN "public"."reward_punishment"."reward_punishment_type" IS '奖惩类型 (Reward/Punishment Type)';
COMMENT ON COLUMN "public"."reward_punishment"."reward_punishment_operator" IS '奖惩操作人 (Reward/Punishment Operator)';
COMMENT ON COLUMN "public"."reward_punishment"."reward_punishment_time" IS '奖惩时间 (Reward/Punishment Time)';
COMMENT ON COLUMN "public"."reward_punishment"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."reward_punishment"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."reward_punishment"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."reward_punishment"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."reward_punishment"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."reward_punishment" IS '员工奖惩 (Employee Rewards/Punishments)';

-- ----------------------------
-- Table structure for reward_punishment_type
-- ----------------------------
DROP TABLE IF EXISTS "public"."reward_punishment_type";
CREATE TABLE "public"."reward_punishment_type" (
  "id" int4 NOT NULL DEFAULT nextval('reward_punishment_type_id_seq'::regclass),
  "reward_punishment_type_number" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "reward_punishment_type_name" varchar(200) COLLATE "pg_catalog"."default" NOT NULL,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."reward_punishment_type"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."reward_punishment_type"."reward_punishment_type_number" IS '奖惩类型唯一编号 (Unique Reward/Punishment Type ID)';
COMMENT ON COLUMN "public"."reward_punishment_type"."reward_punishment_type_name" IS '奖惩类型名称（如优秀员工奖/迟到警告）';
COMMENT ON COLUMN "public"."reward_punishment_type"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."reward_punishment_type"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."reward_punishment_type"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."reward_punishment_type"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."reward_punishment_type"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."reward_punishment_type" IS '奖惩类型配置表 (Reward/Punishment Type Configuration)';

-- ----------------------------
-- Table structure for role
-- ----------------------------
DROP TABLE IF EXISTS "public"."role";
CREATE TABLE "public"."role" (
  "id" int4 NOT NULL DEFAULT nextval('role_id_seq'::regclass),
  "role_number" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "role_name" varchar(200) COLLATE "pg_catalog"."default" NOT NULL,
  "role_description" varchar(500) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."role"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."role"."role_number" IS '角色唯一标识编码 (Unique Role Identifier)';
COMMENT ON COLUMN "public"."role"."role_name" IS '角色名称（如管理员/前台接待）';
COMMENT ON COLUMN "public"."role"."role_description" IS '角色详细权限描述 (Detailed Permissions Description)';
COMMENT ON COLUMN "public"."role"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."role"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."role"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."role"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."role"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."role" IS '系统角色配置表 (System Role Configuration)';

-- ----------------------------
-- Table structure for role_permission
-- ----------------------------
DROP TABLE IF EXISTS "public"."role_permission";
CREATE TABLE "public"."role_permission" (
  "id" int4 NOT NULL DEFAULT nextval('role_permission_id_seq'::regclass),
  "role_number" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "permission_number" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."role_permission"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."role_permission"."role_number" IS '关联角色编码 (Linked Role Code)';
COMMENT ON COLUMN "public"."role_permission"."permission_number" IS '关联权限编码 (Linked Permission Code)';
COMMENT ON COLUMN "public"."role_permission"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."role_permission"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."role_permission"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."role_permission"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."role_permission"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."role_permission" IS '角色权限关联表 (Role-Permission Mapping)';

-- ----------------------------
-- Table structure for room
-- ----------------------------
DROP TABLE IF EXISTS "public"."room";
CREATE TABLE "public"."room" (
  "id" int4 NOT NULL DEFAULT nextval('room_id_seq'::regclass),
  "room_no" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "room_type" int4 NOT NULL,
  "custo_no" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "check_in_time" timestamp(6),
  "check_out_time" timestamp(6),
  "room_state_id" int4 NOT NULL,
  "room_rent" numeric NOT NULL,
  "room_deposit" numeric NOT NULL DEFAULT 0.00,
  "room_position" varchar(200) COLLATE "pg_catalog"."default" NOT NULL,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."room"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."room"."room_no" IS '房间唯一编号 (Unique Room Number)';
COMMENT ON COLUMN "public"."room"."room_type" IS '房间类型ID (关联房间类型表)';
COMMENT ON COLUMN "public"."room"."custo_no" IS '当前入住客户编号 (Linked Customer ID)';
COMMENT ON COLUMN "public"."room"."check_in_time" IS '最后一次入住时间 (Last Check-In Time)';
COMMENT ON COLUMN "public"."room"."check_out_time" IS '最后一次退房时间 (Last Check-Out Time)';
COMMENT ON COLUMN "public"."room"."room_state_id" IS '房间状态ID (如0-空闲/1-已入住)';
COMMENT ON COLUMN "public"."room"."room_rent" IS '房间单价（单位：元） (Price per Night in CNY)';
COMMENT ON COLUMN "public"."room"."room_deposit" IS '房间押金（单位：元） (Deposit Amount in CNY)';
COMMENT ON COLUMN "public"."room"."room_position" IS '房间位置描述 (如楼层+门牌号)';
COMMENT ON COLUMN "public"."room"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."room"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."room"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."room"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."room"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."room" IS '酒店房间信息表 (Hotel Room Information)';

-- ----------------------------
-- Table structure for room_type
-- ----------------------------
DROP TABLE IF EXISTS "public"."room_type";
CREATE TABLE "public"."room_type" (
  "id" int4 NOT NULL DEFAULT nextval('room_type_id_seq'::regclass),
  "room_type" int4 NOT NULL,
  "room_name" varchar(200) COLLATE "pg_catalog"."default" NOT NULL,
  "room_rent" numeric(18,2) NOT NULL,
  "room_deposit" numeric(18,2) NOT NULL DEFAULT 0.00,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."room_type"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."room_type"."room_type" IS '房间类型唯一编号 (Unique Room Type ID)';
COMMENT ON COLUMN "public"."room_type"."room_name" IS '房间类型名称 (如标准间/豪华套房)';
COMMENT ON COLUMN "public"."room_type"."room_rent" IS '每日租金（单位：元） (Price per Day in CNY)';
COMMENT ON COLUMN "public"."room_type"."room_deposit" IS '入住押金（单位：元） (Deposit Amount in CNY)';
COMMENT ON COLUMN "public"."room_type"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."room_type"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."room_type"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."room_type"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."room_type"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."room_type" IS '房间类型配置表 (Room Type Configuration)';

-- ----------------------------
-- Table structure for sellthing
-- ----------------------------
DROP TABLE IF EXISTS "public"."sellthing";
CREATE TABLE "public"."sellthing" (
  "id" int4 NOT NULL DEFAULT nextval('sellthing_id_seq'::regclass),
  "sell_no" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "sell_name" varchar(500) COLLATE "pg_catalog"."default" NOT NULL,
  "sell_price" numeric(18,2) NOT NULL,
  "specification" varchar(1000) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "stock" numeric NOT NULL DEFAULT '0'::numeric,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."sellthing"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."sellthing"."sell_no" IS '商品唯一编号 (Unique Product ID)';
COMMENT ON COLUMN "public"."sellthing"."sell_name" IS '商品名称（如『西湖龙井茶叶500g』）';
COMMENT ON COLUMN "public"."sellthing"."sell_price" IS '商品单价（单位：元）';
COMMENT ON COLUMN "public"."sellthing"."specification" IS '规格描述（如『500g/罐，陶瓷包装』）';
COMMENT ON COLUMN "public"."sellthing"."stock" IS '当前库存数量（单位：件/个）';
COMMENT ON COLUMN "public"."sellthing"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."sellthing"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."sellthing"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."sellthing"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."sellthing"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."sellthing" IS '商品信息表 (Product Information)';

-- ----------------------------
-- Table structure for supervision_statistics
-- ----------------------------
DROP TABLE IF EXISTS "public"."supervision_statistics";
CREATE TABLE "public"."supervision_statistics" (
  "id" int4 NOT NULL DEFAULT nextval('supervision_statistics_id_seq'::regclass),
  "statistics_number" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "supervising_department" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "supervision_progress" varchar(200) COLLATE "pg_catalog"."default" NOT NULL,
  "supervision_loss" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "supervision_score" int4 NOT NULL DEFAULT 100,
  "supervision_statistician" varchar(50) COLLATE "pg_catalog"."default" NOT NULL,
  "supervision_advice" varchar(1000) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "supervision_time" timestamp(6) NOT NULL,
  "rectification_status" int4 NOT NULL DEFAULT 0,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."supervision_statistics"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."supervision_statistics"."statistics_number" IS '监管记录唯一编号 (Unique Statistics ID)';
COMMENT ON COLUMN "public"."supervision_statistics"."supervising_department" IS '监管部门编码（关联部门表）';
COMMENT ON COLUMN "public"."supervision_statistics"."supervision_progress" IS '监管进度描述（如：已完成/整改中）';
COMMENT ON COLUMN "public"."supervision_statistics"."supervision_loss" IS '监管造成的经济损失';
COMMENT ON COLUMN "public"."supervision_statistics"."supervision_score" IS '监管评分（范围：0-100分）';
COMMENT ON COLUMN "public"."supervision_statistics"."supervision_statistician" IS '统计责任人姓名';
COMMENT ON COLUMN "public"."supervision_statistics"."supervision_advice" IS '监管整改建议内容';
COMMENT ON COLUMN "public"."supervision_statistics"."supervision_time" IS '监管检查时间';
COMMENT ON COLUMN "public"."supervision_statistics"."rectification_status" IS '0-未整改/1-已整改';
COMMENT ON COLUMN "public"."supervision_statistics"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."supervision_statistics"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."supervision_statistics"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."supervision_statistics"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."supervision_statistics"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."supervision_statistics" IS '监管统计信息表 (Supervision Statistics)';

-- ----------------------------
-- Table structure for user_role
-- ----------------------------
DROP TABLE IF EXISTS "public"."user_role";
CREATE TABLE "public"."user_role" (
  "id" int4 NOT NULL DEFAULT nextval('user_role_id_seq'::regclass),
  "role_number" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "user_number" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."user_role"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."user_role"."role_number" IS '关联角色编码 (Linked Role Code)';
COMMENT ON COLUMN "public"."user_role"."user_number" IS '关联用户编码 (Linked User Code)';
COMMENT ON COLUMN "public"."user_role"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."user_role"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."user_role"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."user_role"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."user_role"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."user_role" IS '用户角色关联表 (User-Role Mapping)';

-- ----------------------------
-- Table structure for vip_rule
-- ----------------------------
DROP TABLE IF EXISTS "public"."vip_rule";
CREATE TABLE "public"."vip_rule" (
  "id" int4 NOT NULL DEFAULT nextval('vip_rule_id_seq'::regclass),
  "rule_id" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "rule_name" varchar(200) COLLATE "pg_catalog"."default" NOT NULL,
  "rule_value" numeric(18,2) NOT NULL,
  "type_id" int4 NOT NULL,
  "delete_mk" int4 NOT NULL DEFAULT 0,
  "datains_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datains_date" timestamp(6),
  "datachg_usr" varchar(128) COLLATE "pg_catalog"."default" DEFAULT NULL::character varying,
  "datachg_date" timestamp(6)
)
;
COMMENT ON COLUMN "public"."vip_rule"."id" IS '编号 (ID)';
COMMENT ON COLUMN "public"."vip_rule"."rule_id" IS '规则业务唯一编号 (如VIPR-2023-001)';
COMMENT ON COLUMN "public"."vip_rule"."rule_name" IS '规则名称（如『黄金会员准入规则』）';
COMMENT ON COLUMN "public"."vip_rule"."rule_value" IS '累计消费金额阈值（单位：元）';
COMMENT ON COLUMN "public"."vip_rule"."type_id" IS '关联会员等级表 (Linked VIP Level)';
COMMENT ON COLUMN "public"."vip_rule"."delete_mk" IS '删除标识';
COMMENT ON COLUMN "public"."vip_rule"."datains_usr" IS '资料创建人';
COMMENT ON COLUMN "public"."vip_rule"."datains_date" IS '资料创建时间';
COMMENT ON COLUMN "public"."vip_rule"."datachg_usr" IS '资料更新人';
COMMENT ON COLUMN "public"."vip_rule"."datachg_date" IS '资料更新时间';
COMMENT ON TABLE "public"."vip_rule" IS '会员等级规则配置表 (VIP Level Rule Configuration)';

-- ----------------------------
-- Primary Key structure for table administrator
-- ----------------------------
ALTER TABLE "public"."administrator" ADD CONSTRAINT "administrator_pkey" PRIMARY KEY ("id", "admin_number");

-- ----------------------------
-- Primary Key structure for table administrator_type
-- ----------------------------
ALTER TABLE "public"."administrator_type" ADD CONSTRAINT "administrator_type_pkey" PRIMARY KEY ("id", "type_id");

-- ----------------------------
-- Primary Key structure for table app_banner
-- ----------------------------
ALTER TABLE "public"."app_banner" ADD CONSTRAINT "app_banner_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table app_config_base
-- ----------------------------
ALTER TABLE "public"."app_config_base" ADD CONSTRAINT "app_config_base_pkey" PRIMARY KEY ("id", "url_no");

-- ----------------------------
-- Primary Key structure for table app_version
-- ----------------------------
ALTER TABLE "public"."app_version" ADD CONSTRAINT "app_version_pkey" PRIMARY KEY ("base_versionid");

-- ----------------------------
-- Primary Key structure for table appointment_notice
-- ----------------------------
ALTER TABLE "public"."appointment_notice" ADD CONSTRAINT "appointment_notice_pkey" PRIMARY KEY ("id", "notice_no");

-- ----------------------------
-- Primary Key structure for table asset
-- ----------------------------
ALTER TABLE "public"."asset" ADD CONSTRAINT "asset_pkey" PRIMARY KEY ("id", "asset_number");

-- ----------------------------
-- Primary Key structure for table card_code
-- ----------------------------
ALTER TABLE "public"."card_code" ADD CONSTRAINT "card_code_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table custo_type
-- ----------------------------
ALTER TABLE "public"."custo_type" ADD CONSTRAINT "custo_type_pkey" PRIMARY KEY ("id", "custo_type");

-- ----------------------------
-- Primary Key structure for table customer
-- ----------------------------
ALTER TABLE "public"."customer" ADD CONSTRAINT "customer_pkey" PRIMARY KEY ("id", "custo_no");

-- ----------------------------
-- Primary Key structure for table customer_spend
-- ----------------------------
ALTER TABLE "public"."customer_spend" ADD CONSTRAINT "customer_spend_pkey" PRIMARY KEY ("spend_number");

-- ----------------------------
-- Primary Key structure for table department
-- ----------------------------
ALTER TABLE "public"."department" ADD CONSTRAINT "department_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table employee
-- ----------------------------
ALTER TABLE "public"."employee" ADD CONSTRAINT "employee_pkey" PRIMARY KEY ("id", "employee_number");

-- ----------------------------
-- Primary Key structure for table employee_check
-- ----------------------------
ALTER TABLE "public"."employee_check" ADD CONSTRAINT "employee_check_pkey" PRIMARY KEY ("id", "check_number");

-- ----------------------------
-- Primary Key structure for table employee_history
-- ----------------------------
ALTER TABLE "public"."employee_history" ADD CONSTRAINT "employee_history_pkey" PRIMARY KEY ("id", "history_number");

-- ----------------------------
-- Primary Key structure for table employee_pic
-- ----------------------------
ALTER TABLE "public"."employee_pic" ADD CONSTRAINT "employee_pic_pkey" PRIMARY KEY ("id", "employee_number");

-- ----------------------------
-- Primary Key structure for table energy_management
-- ----------------------------
ALTER TABLE "public"."energy_management" ADD CONSTRAINT "energy_management_pkey" PRIMARY KEY ("id", "information_number");

-- ----------------------------
-- Primary Key structure for table menu
-- ----------------------------
ALTER TABLE "public"."menu" ADD CONSTRAINT "menu_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table nation
-- ----------------------------
ALTER TABLE "public"."nation" ADD CONSTRAINT "nation_pkey" PRIMARY KEY ("id", "nation_no");

-- ----------------------------
-- Primary Key structure for table nav_bar
-- ----------------------------
ALTER TABLE "public"."nav_bar" ADD CONSTRAINT "nav_bar_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table operation_log
-- ----------------------------
ALTER TABLE "public"."operation_log" ADD CONSTRAINT "operation_log_pkey" PRIMARY KEY ("id", "operation_number");

-- ----------------------------
-- Primary Key structure for table passport_type
-- ----------------------------
ALTER TABLE "public"."passport_type" ADD CONSTRAINT "passport_type_pkey" PRIMARY KEY ("passport_number");

-- ----------------------------
-- Primary Key structure for table position
-- ----------------------------
ALTER TABLE "public"."position" ADD CONSTRAINT "position_pkey" PRIMARY KEY ("id", "position_no");

-- ----------------------------
-- Primary Key structure for table qualification
-- ----------------------------
ALTER TABLE "public"."qualification" ADD CONSTRAINT "qualification_pkey" PRIMARY KEY ("id", "education_no");

-- ----------------------------
-- Indexes structure for table reser
-- ----------------------------
CREATE INDEX "index_reser_reser_room" ON "public"."reser" USING btree (
  "reser_room" COLLATE "pg_catalog"."default" "pg_catalog"."text_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table reser
-- ----------------------------
ALTER TABLE "public"."reser" ADD CONSTRAINT "reser_pkey" PRIMARY KEY ("id", "reser_number");

-- ----------------------------
-- Primary Key structure for table reward_punishment
-- ----------------------------
ALTER TABLE "public"."reward_punishment" ADD CONSTRAINT "reward_punishment_pkey" PRIMARY KEY ("id", "reward_punishment_id");

-- ----------------------------
-- Primary Key structure for table reward_punishment_type
-- ----------------------------
ALTER TABLE "public"."reward_punishment_type" ADD CONSTRAINT "reward_punishment_type_pkey" PRIMARY KEY ("id", "reward_punishment_type_number");

-- ----------------------------
-- Primary Key structure for table role
-- ----------------------------
ALTER TABLE "public"."role" ADD CONSTRAINT "role_pkey" PRIMARY KEY ("id", "role_number");

-- ----------------------------
-- Indexes structure for table role_permission
-- ----------------------------
CREATE INDEX "index_role_permission_permission_number" ON "public"."role_permission" USING btree (
  "permission_number" COLLATE "pg_catalog"."default" "pg_catalog"."text_ops" ASC NULLS LAST
);
CREATE INDEX "index_role_permission_role_number" ON "public"."role_permission" USING btree (
  "role_number" COLLATE "pg_catalog"."default" "pg_catalog"."text_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table role_permission
-- ----------------------------
ALTER TABLE "public"."role_permission" ADD CONSTRAINT "role_permission_pkey" PRIMARY KEY ("id", "role_number", "permission_number");

-- ----------------------------
-- Primary Key structure for table room
-- ----------------------------
ALTER TABLE "public"."room" ADD CONSTRAINT "room_pkey" PRIMARY KEY ("id", "room_no");

-- ----------------------------
-- Primary Key structure for table room_type
-- ----------------------------
ALTER TABLE "public"."room_type" ADD CONSTRAINT "room_type_pkey" PRIMARY KEY ("id", "room_type");

-- ----------------------------
-- Primary Key structure for table sellthing
-- ----------------------------
ALTER TABLE "public"."sellthing" ADD CONSTRAINT "sellthing_pkey" PRIMARY KEY ("id", "sell_no");

-- ----------------------------
-- Primary Key structure for table supervision_statistics
-- ----------------------------
ALTER TABLE "public"."supervision_statistics" ADD CONSTRAINT "supervision_statistics_pkey" PRIMARY KEY ("id", "statistics_number");

-- ----------------------------
-- Indexes structure for table user_role
-- ----------------------------
CREATE INDEX "index_user_role_role_number" ON "public"."user_role" USING btree (
  "role_number" COLLATE "pg_catalog"."default" "pg_catalog"."text_ops" ASC NULLS LAST
);
CREATE INDEX "index_user_role_user_number" ON "public"."user_role" USING btree (
  "user_number" COLLATE "pg_catalog"."default" "pg_catalog"."text_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table user_role
-- ----------------------------
ALTER TABLE "public"."user_role" ADD CONSTRAINT "user_role_pkey" PRIMARY KEY ("id", "role_number", "user_number");

-- ----------------------------
-- Primary Key structure for table vip_rule
-- ----------------------------
ALTER TABLE "public"."vip_rule" ADD CONSTRAINT "vip_rule_pkey" PRIMARY KEY ("id", "rule_id");
