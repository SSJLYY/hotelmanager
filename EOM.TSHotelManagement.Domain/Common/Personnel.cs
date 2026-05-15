using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace EOM.TSHotelManagement.Domain;

public class Personnel : SoftDeleteEntity
{
    [SugarColumn(ColumnName = "name", IsNullable = false, ColumnDescription = "姓名", Length = 250)]
    public string Name { get; set; } = string.Empty;
    [SugarColumn(ColumnName = "phone_number", IsNullable = false, ColumnDescription = "电话号码", Length = 256)]
    public string PhoneNumber { get; set; } = string.Empty;
    [SugarColumn(ColumnName = "id_number", IsNullable = false, ColumnDescription = "证件号码", Length = 256)]
    public string IdCardNumber { get; set; } = string.Empty;
    [SugarColumn(ColumnName = "address", IsNullable = false, ColumnDescription = "联系地址", Length = 500)]
    public string Address { get; set; } = string.Empty;
    [SugarColumn(ColumnName = "date_of_birth", IsNullable = false, ColumnDescription = "出生日期")]
    public DateOnly DateOfBirth { get; set; } = DateOnly.MinValue;
    [SugarColumn(ColumnName = "gender", IsNullable = false, ColumnDescription = "性别(0/女，1/男)")]
    public int Gender { get; set; } = 0;
    [SugarColumn(ColumnName = "id_type", IsNullable = false, ColumnDescription = "证件类型")]
    public int IdCardType { get; set; } = 0;
    [SugarColumn(ColumnName = "ethnicity", IsNullable = false, ColumnDescription = "民族", Length = 128)]
    public string Ethnicity { get; set; } = string.Empty;
    [SugarColumn(ColumnName = "education_level", IsNullable = false, ColumnDescription = "教育程度", Length = 128)]
    public string EducationLevel { get; set; } = string.Empty;
    [SugarColumn(ColumnName = "email_address", IsNullable = false, Length = 256, ColumnDescription = "邮箱地址")]
    public string EmailAddress { get; set; } = string.Empty;
}
