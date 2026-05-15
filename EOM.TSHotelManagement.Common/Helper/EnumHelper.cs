using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace EOM.TSHotelManagement.Common.Helper
{
    public static class EnumHelper
    {
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = field?
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .FirstOrDefault() as DescriptionAttribute;

            return attribute?.Description ?? value.ToString();
        }

        public static int GetEnumValue(Enum value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return Convert.ToInt32(value);
        }

        public static string GetDescriptionByName<TEnum>(string enumName) where TEnum : Enum
        {
            Type enumType = typeof(TEnum);

            if (!Enum.IsDefined(enumType, enumName))
            {
                return null;
            }

            FieldInfo field = enumType.GetField(enumName);
            DescriptionAttribute attribute = field?
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .SingleOrDefault() as DescriptionAttribute;

            string description = attribute?.Description ?? enumName;

            return description;
        }
    }
}
