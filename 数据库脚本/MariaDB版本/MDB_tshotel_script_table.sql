/*
 Navicat Premium Dump SQL

 Source Server         : local-MariaDB
 Source Server Type    : MariaDB
 Source Server Version : 101110 (10.11.10-MariaDB)
 Source Host           : localhost:3306
 Source Schema         : tshoteldb_test

 Target Server Type    : MariaDB
 Target Server Version : 101110 (10.11.10-MariaDB)
 File Encoding         : 65001

 Date: 26/04/2025 17:48:40
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for administrator
-- ----------------------------
DROP TABLE IF EXISTS `administrator`;
CREATE TABLE `administrator`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `admin_number` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '管理员账号 (Administrator Account)',
  `admin_account` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '管理员名称 (Administrator Name)',
  `admin_password` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '管理员密码 (Administrator Password)',
  `admin_type` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '管理员类型 (Administrator Type)',
  `admin_name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '管理员名称 (Administrator Name)',
  `is_admin` int(11) NOT NULL COMMENT '是否为超级管理员 (Is Super Administrator)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`, `admin_number`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '管理员实体类 (Administrator Entity)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for administrator_type
-- ----------------------------
DROP TABLE IF EXISTS `administrator_type`;
CREATE TABLE `administrator_type`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `type_id` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '类型编号 (Type ID)',
  `type_name` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '类型名称 (Type Name)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`, `type_id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '管理员类型 (Administrator Type)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for app_banner
-- ----------------------------
DROP TABLE IF EXISTS `app_banner`;
CREATE TABLE `app_banner`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `banner_content` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '宣传内容（支持富文本） (Promotion Content with Rich Text)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = 'APP横幅配置表 (APP Banner Configuration)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for app_config_base
-- ----------------------------
DROP TABLE IF EXISTS `app_config_base`;
CREATE TABLE `app_config_base`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `url_no` int(11) NOT NULL COMMENT '地址编号 (URL Number)',
  `url_addr` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '地址 (URL Address)',
  PRIMARY KEY (`id`, `url_no`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '系统信息 (System Information)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for app_version
-- ----------------------------
DROP TABLE IF EXISTS `app_version`;
CREATE TABLE `app_version`  (
  `base_versionId` int(11) NOT NULL AUTO_INCREMENT COMMENT '流水号 (Sequence Number)',
  `base_version` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '版本号 (Version Number)',
  PRIMARY KEY (`base_versionId`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '应用版本 (Application Version)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for appointment_notice
-- ----------------------------
DROP TABLE IF EXISTS `appointment_notice`;
CREATE TABLE `appointment_notice`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `notice_no` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '公告编号 (AppointmentNotice Number)',
  `notice_theme` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '公告主题 (AppointmentNotice Theme)',
  `notice_type` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '公告类型 (AppointmentNotice Type)',
  `notice_time` datetime NOT NULL COMMENT '公告时间 (AppointmentNotice Time)',
  `notice_content` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '公告正文 (AppointmentNotice Content)',
  `notice_department` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '发文部门 (Issuing Department)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`, `notice_no`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '任命公告 (Appointment AppointmentNotice)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for asset
-- ----------------------------
DROP TABLE IF EXISTS `asset`;
CREATE TABLE `asset`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `asset_number` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '资产编号',
  `asset_name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '资产名称',
  `asset_value` decimal(18, 2) NOT NULL COMMENT '资产名称',
  `department_code` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '所属部门代码 (Department Code)',
  `acquisition_date` datetime NOT NULL COMMENT '入库时间 (购置日期) (Acquisition Date)',
  `asset_source` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '资产来源 (Asset Source)',
  `acquired_by_employee` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '资产经办人 (员工ID) (Acquired By - Employee ID)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`, `asset_number`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '资产管理' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for card_code
-- ----------------------------
DROP TABLE IF EXISTS `card_code`;
CREATE TABLE `card_code`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `province` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '省份 (Province)',
  `city` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '城市 (City)',
  `district` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '地区 (District)',
  `district_code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '地区识别码 (Area Code)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 4489 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '卡片代码 (Card Codes)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for custo_type
-- ----------------------------
DROP TABLE IF EXISTS `custo_type`;
CREATE TABLE `custo_type`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `custo_type` int(11) NOT NULL COMMENT '客户类型 (Customer Type)',
  `type_name` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '客户类型名称 (Customer Type Name)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`, `custo_type`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '客户类型' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for customer
-- ----------------------------
DROP TABLE IF EXISTS `customer`;
CREATE TABLE `customer`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `custo_no` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '客户编号 (Customer Number)',
  `custo_name` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '客户名称 (Customer Name)',
  `custo_gender` int(11) NOT NULL COMMENT '客户性别 (Customer Gender)',
  `passport_type` int(11) NOT NULL COMMENT '客户性别 (Customer Gender)',
  `custo_tel` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '客户电话 (Customer Phone Number)',
  `custo_birth` date NOT NULL COMMENT '出生日期 (Date of Birth)',
  `passport_id` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '证件号码 (Passport ID)',
  `custo_address` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '居住地址 (Customer Address)',
  `custo_type` int(11) NOT NULL COMMENT '客户类型 (Customer Type)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`, `custo_no`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '客户信息' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for customer_spend
-- ----------------------------
DROP TABLE IF EXISTS `customer_spend`;
CREATE TABLE `customer_spend`  (
  `spend_number` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '消费编号 (Consumption Number)',
  `room_no` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '房间编号 (Room Number)',
  `custo_no` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '客户编号 (Customer Number)',
  `product_number` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '商品编号 (Product Number)',
  `spend_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '商品名称 (Product Name)',
  `apend_quantity` int(11) NOT NULL COMMENT '消费数量 (Consumption Quantity)',
  `spend_price` decimal(18, 4) NOT NULL COMMENT '商品单价 (Product Price)',
  `spend_amount` decimal(18, 4) NOT NULL COMMENT '消费金额 (Consumption Amount)',
  `spend_time` datetime NOT NULL COMMENT '消费时间 (Consumption Time)',
  `settlement_status` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '结算状态 (Settlement Status)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`spend_number`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '消费信息 (Consumption Information)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for department
-- ----------------------------
DROP TABLE IF EXISTS `department`;
CREATE TABLE `department`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `dept_no` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '部门编号 (Department Number)',
  `dept_name` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '部门名称 (Department Name)',
  `dept_desc` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '部门描述 (Department Description)',
  `dept_date` datetime NULL DEFAULT NULL COMMENT '创建时间(部门) (Department Creation Date)',
  `dept_leader` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '部门主管 (Department Leader)',
  `dept_parent` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '上级部门编号 (Parent Department Number)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '部门表 (Department Table)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for employee
-- ----------------------------
DROP TABLE IF EXISTS `employee`;
CREATE TABLE `employee`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `employee_number` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '员工账号/工号 (Employee Account/ID)',
  `employee_name` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '员工姓名 (Employee Name)',
  `employee_date_of_birth` date NOT NULL COMMENT '出生日期 (Date of Birth)',
  `employee_gender` int(11) NOT NULL COMMENT '员工性别 (Employee Gender)',
  `employee_nation` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '民族类型 (Ethnicity)',
  `employee_tel` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '员工电话 (Employee Phone Number)',
  `employee_department` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '所属部门 (Department)',
  `employee_address` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '居住地址 (Residential Address)',
  `employee_postion` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '员工职位 (Employee Position)',
  `card_type` int(11) NOT NULL COMMENT '证件类型 (ID Card Type)',
  `card_number` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '证件号码 (ID Card Number)',
  `employee_password` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '员工密码 (Employee Password)',
  `hire_time` date NOT NULL COMMENT '员工入职时间 (Hire Date)',
  `employee_political` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '员工面貌 (Political Affiliation)',
  `employee_quality` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '教育程度 (Education Level)',
  `enable_mk` int(11) NOT NULL COMMENT '禁用标记',
  `email_address` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '邮箱地址',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`, `employee_number`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '员工信息 (Employee Information)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for employee_check
-- ----------------------------
DROP TABLE IF EXISTS `employee_check`;
CREATE TABLE `employee_check`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `check_number` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '打卡编号',
  `employee_number` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '员工工号 (Employee ID)',
  `check_time` datetime NOT NULL COMMENT '打卡时间 (Check-in/Check-out Time)',
  `check_way` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '打卡方式 (Check-in/Check-out Method)',
  `check_state` int(11) NOT NULL COMMENT '打卡状态 (Check-in/Check-out Status)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`, `check_number`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '员工打卡考勤 (Employee Check-in/Check-out Record)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for employee_history
-- ----------------------------
DROP TABLE IF EXISTS `employee_history`;
CREATE TABLE `employee_history`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `history_number` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '履历编号',
  `employee_number` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '员工工号 (Employee ID)',
  `start_date` date NOT NULL COMMENT '开始时间 (Start Date)',
  `end_date` date NOT NULL COMMENT '结束时间 (End Date)',
  `position` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '职位 (Position)',
  `company` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '公司 (Company)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`, `history_number`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '员工履历 (Employee History)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for employee_pic
-- ----------------------------
DROP TABLE IF EXISTS `employee_pic`;
CREATE TABLE `employee_pic`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `employee_number` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '员工工号 (Employee ID)',
  `pic_url` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '照片路径 (Photo Path)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`, `employee_number`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '员工照片 (Employee Photo)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for energy_management
-- ----------------------------
DROP TABLE IF EXISTS `energy_management`;
CREATE TABLE `energy_management`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `information_number` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '信息编号 (Information ID)',
  `room_no` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '房间编号 (Room Number)',
  `use_date` datetime NOT NULL COMMENT '开始使用时间 (Start Date)',
  `end_date` datetime NOT NULL COMMENT '结束使用时间 (End Date)',
  `water_use` decimal(18, 2) NOT NULL COMMENT '水费 (Water Usage)',
  `power_use` decimal(18, 2) NOT NULL COMMENT '电费 (Power Usage)',
  `recorder` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL DEFAULT 'Administrator' COMMENT '记录员 (Recorder)',
  `custo_no` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '客户编号 (Customer Number)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`, `information_number`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '水电信息' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for menu
-- ----------------------------
DROP TABLE IF EXISTS `menu`;
CREATE TABLE `menu`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `key` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '菜单键 (Menu Key)',
  `title` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '菜单标题 (Menu Title)',
  `path` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '菜单路径 (Menu Path)',
  `parent` int(11) NULL DEFAULT NULL COMMENT '父级ID (Parent ID)',
  `icon` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '图标',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 36 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '菜单表 (Menu Table)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for nation
-- ----------------------------
DROP TABLE IF EXISTS `nation`;
CREATE TABLE `nation`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `nation_no` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '民族编号 (Nation Number)',
  `nation_name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '民族名称 (Nation Name)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`, `nation_no`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '民族信息表 (Nation Information)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for nav_bar
-- ----------------------------
DROP TABLE IF EXISTS `nav_bar`;
CREATE TABLE `nav_bar`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `nav_name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '导航控件名称 (Navigation Bar Name)',
  `nav_or` int(11) NOT NULL DEFAULT 0 COMMENT '导航控件排序 (Navigation Bar Order)',
  `nav_pic` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '导航控件图片路径 (Navigation Bar Image Path)',
  `nav_event` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '导航控件事件标识 (Navigation Bar Event Identifier)',
  `margin_left` int(11) NOT NULL DEFAULT 0 COMMENT '导航控件左边距像素值 (Margin Left in Pixels)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '导航栏配置表 (Navigation Bar Configuration)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for operation_log
-- ----------------------------
DROP TABLE IF EXISTS `operation_log`;
CREATE TABLE `operation_log`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `operation_number` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '日志ID (Log ID)',
  `operation_time` datetime NOT NULL COMMENT '操作时间 (Operation Time)',
  `log_content` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '操作信息 (Log Content)',
  `operation_account` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '操作账号 (Operation Account)',
  `operation_level` int(11) NOT NULL COMMENT '日志等级枚举值 (Log Level Enum)',
  `software_version` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '软件版本号 (Software Version)',
  `login_ip` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '登录IP地址 (Login IP Address)',
  `request_path` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '请求路径 (Request Path)',
  `query_string` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '查询参数 (Query String)',
  `elapsed_time` bigint(20) NOT NULL DEFAULT 0 COMMENT '响应时间（毫秒） (Elapsed Time in ms)',
  `http_method` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'HTTP请求方法 (HTTP Method)',
  `status_code` int(11) NOT NULL DEFAULT 200 COMMENT 'HTTP状态码 (HTTP Status Code)',
  `exception_message` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '异常消息内容 (Exception Message)',
  `exception_stacktrace` varchar(4000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '异常堆栈信息 (Exception Stack Trace)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`, `operation_number`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 46 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '操作日志表 (Operation Log)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for passport_type
-- ----------------------------
DROP TABLE IF EXISTS `passport_type`;
CREATE TABLE `passport_type`  (
  `passport_number` int(11) NOT NULL COMMENT '证件类型',
  `passport_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '证件名称',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`passport_number`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '证件类型' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for position
-- ----------------------------
DROP TABLE IF EXISTS `position`;
CREATE TABLE `position`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `position_no` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '职位编号 (Position Number)',
  `position_name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '职位名称 (Position Name)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`, `position_no`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '职位信息表 (Position Information)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for qualification
-- ----------------------------
DROP TABLE IF EXISTS `qualification`;
CREATE TABLE `qualification`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `education_no` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '学历编号 (Education Number)',
  `education_name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '学历名称 (Education Name)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`, `education_no`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '学历 (Education)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for reser
-- ----------------------------
DROP TABLE IF EXISTS `reser`;
CREATE TABLE `reser`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `reser_number` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '预约唯一编号 (Unique Reservation ID)',
  `custo_name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '客户姓名 (Customer Full Name)',
  `custo_tel` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '客户联系电话 (Contact Phone Number)',
  `reser_way` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '预约来源渠道 (如官网/APP/第三方平台)',
  `reser_room` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '预定房间编号 (关联房间表)',
  `reser_date` datetime NOT NULL COMMENT '入住日期（格式：yyyy-MM-dd） (Check-In Date)',
  `reser_end_date` datetime NOT NULL COMMENT '离店日期（格式：yyyy-MM-dd） (Check-Out Date)',
  `reser_status` int(11) NOT NULL DEFAULT 0 COMMENT '预约状态（0-待确认/1-已确认/2-已取消）',
  `remarks` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '特殊需求备注 (Special Requests)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`, `reser_number`) USING BTREE,
  INDEX `Index_reser_reser_room`(`reser_room` ASC) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '预约信息表 (Reservation Information)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for reward_punishment
-- ----------------------------
DROP TABLE IF EXISTS `reward_punishment`;
CREATE TABLE `reward_punishment`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `reward_punishment_id` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '奖惩编号 (Reward/Punishment ID)',
  `employee_number` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '员工工号 (Employee ID)',
  `reward_punishment_information` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '奖惩信息 (Reward/Punishment Information)',
  `reward_punishment_type` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '奖惩类型 (Reward/Punishment Type)',
  `reward_punishment_operator` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '奖惩操作人 (Reward/Punishment Operator)',
  `reward_punishment_time` datetime NOT NULL COMMENT '奖惩时间 (Reward/Punishment Time)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`, `reward_punishment_id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '员工奖惩 (Employee Rewards/Punishments)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for reward_punishment_type
-- ----------------------------
DROP TABLE IF EXISTS `reward_punishment_type`;
CREATE TABLE `reward_punishment_type`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `reward_punishment_type_number` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '奖惩类型唯一编号 (Unique Reward/Punishment Type ID)',
  `reward_punishment_type_name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '奖惩类型名称（如优秀员工奖/迟到警告）',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`, `reward_punishment_type_number`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '奖惩类型配置表 (Reward/Punishment Type Configuration)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for role
-- ----------------------------
DROP TABLE IF EXISTS `role`;
CREATE TABLE `role`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `role_number` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '角色唯一标识编码 (Unique Role Identifier)',
  `role_name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '角色名称（如管理员/前台接待）',
  `role_description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '角色详细权限描述 (Detailed Permissions Description)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`, `role_number`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '系统角色配置表 (System Role Configuration)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for role_permission
-- ----------------------------
DROP TABLE IF EXISTS `role_permission`;
CREATE TABLE `role_permission`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `role_number` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '关联角色编码 (Linked Role Code)',
  `permission_number` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '关联权限编码 (Linked Permission Code)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`, `role_number`, `permission_number`) USING BTREE,
  INDEX `Index_role_permission_role_number`(`role_number` ASC) USING BTREE,
  INDEX `Index_role_permission_permission_number`(`permission_number` ASC) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '角色权限关联表 (Role-Permission Mapping)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for room
-- ----------------------------
DROP TABLE IF EXISTS `room`;
CREATE TABLE `room`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `room_no` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '房间唯一编号 (Unique Room Number)',
  `room_type` int(11) NOT NULL COMMENT '房间类型ID (关联房间类型表)',
  `custo_no` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '当前入住客户编号 (Linked Customer ID)',
  `check_in_time` datetime NULL DEFAULT NULL COMMENT '最后一次入住时间 (Last Check-In Time)',
  `check_out_time` datetime NULL DEFAULT NULL COMMENT '最后一次退房时间 (Last Check-Out Time)',
  `room_state_id` int(11) NOT NULL COMMENT '房间状态ID (如0-空闲/1-已入住)',
  `room_rent` decimal(10, 2) NOT NULL COMMENT '房间单价（单位：元） (Price per Night in CNY)',
  `room_deposit` decimal(10, 2) NOT NULL DEFAULT 0.00 COMMENT '房间押金（单位：元） (Deposit Amount in CNY)',
  `room_position` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '房间位置描述 (如楼层+门牌号)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`, `room_no`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '酒店房间信息表 (Hotel Room Information)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for room_type
-- ----------------------------
DROP TABLE IF EXISTS `room_type`;
CREATE TABLE `room_type`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `room_type` int(11) NOT NULL COMMENT '房间类型唯一编号 (Unique Room Type ID)',
  `room_name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '房间类型名称 (如标准间/豪华套房)',
  `room_rent` decimal(18, 2) NOT NULL COMMENT '每日租金（单位：元） (Price per Day in CNY)',
  `room_deposit` decimal(18, 2) NOT NULL DEFAULT 0.00 COMMENT '入住押金（单位：元） (Deposit Amount in CNY)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`, `room_type`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '房间类型配置表 (Room Type Configuration)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for sellthing
-- ----------------------------
DROP TABLE IF EXISTS `sellthing`;
CREATE TABLE `sellthing`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `sell_no` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '商品唯一编号 (Unique Product ID)',
  `sell_name` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '商品名称（如『西湖龙井茶叶500g』）',
  `sell_price` decimal(18, 2) NOT NULL COMMENT '商品单价（单位：元）',
  `specification` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '规格描述（如『500g/罐，陶瓷包装』）',
  `stock` decimal(18, 4) NOT NULL DEFAULT 0.0000 COMMENT '当前库存数量（单位：件/个）',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`, `sell_no`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '商品信息表 (Product Information)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for supervision_statistics
-- ----------------------------
DROP TABLE IF EXISTS `supervision_statistics`;
CREATE TABLE `supervision_statistics`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `statistics_number` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '监管记录唯一编号 (Unique Statistics ID)',
  `supervising_department` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '监管部门编码（关联部门表）',
  `supervision_progress` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '监管进度描述（如：已完成/整改中）',
  `supervision_loss` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '监管造成的经济损失',
  `supervision_score` int(11) NOT NULL DEFAULT 100 COMMENT '监管评分（范围：0-100分）',
  `supervision_statistician` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '统计责任人姓名',
  `supervision_advice` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '监管整改建议内容',
  `supervision_time` datetime NOT NULL COMMENT '监管检查时间',
  `rectification_status` int(11) NOT NULL DEFAULT 0 COMMENT '0-未整改/1-已整改',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`, `statistics_number`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '监管统计信息表 (Supervision Statistics)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for user_role
-- ----------------------------
DROP TABLE IF EXISTS `user_role`;
CREATE TABLE `user_role`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `role_number` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '关联角色编码 (Linked Role Code)',
  `user_number` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '关联用户编码 (Linked User Code)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`, `role_number`, `user_number`) USING BTREE,
  INDEX `Index_user_role_role_number`(`role_number` ASC) USING BTREE,
  INDEX `Index_user_role_user_number`(`user_number` ASC) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '用户角色关联表 (User-Role Mapping)' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for vip_rule
-- ----------------------------
DROP TABLE IF EXISTS `vip_rule`;
CREATE TABLE `vip_rule`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '编号 (ID)',
  `rule_id` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '规则业务唯一编号 (如VIPR-2023-001)',
  `rule_name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT '规则名称（如『黄金会员准入规则』）',
  `rule_value` decimal(18, 2) NOT NULL COMMENT '累计消费金额阈值（单位：元）',
  `type_id` int(11) NOT NULL COMMENT '关联会员等级表 (Linked VIP Level)',
  `delete_mk` int(11) NOT NULL DEFAULT 0 COMMENT '删除标识',
  `datains_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料创建人',
  `datains_date` datetime NULL DEFAULT NULL COMMENT '资料创建时间',
  `datachg_usr` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '资料更新人',
  `datachg_date` datetime NULL DEFAULT NULL COMMENT '资料更新时间',
  PRIMARY KEY (`id`, `rule_id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '会员等级规则配置表 (VIP Level Rule Configuration)' ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;
