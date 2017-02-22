namespace King.Framework.Common
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;

    public static class EnumHelper
    {
        private static Dictionary<string, List<EnumItem>> dic = new Dictionary<string, List<EnumItem>>();

        private static bool CompareItem(FieldInfo fi, int flag, int value)
        {
            int num = Convert.ToInt32(fi.GetRawConstantValue());
            bool flag2 = true;
            if (flag == 0)
            {
                return (num < value);
            }
            if (flag == 1)
            {
                return (num == value);
            }
            if (flag == 2)
            {
                flag2 = num > value;
            }
            return flag2;
        }

        public static string GetDescription(object enumValue)
        {
            if (enumValue == null)
            {
                throw new ArgumentNullException("enumValue");
            }
            if (!enumValue.GetType().IsEnum)
            {
                throw new ArgumentException("enumValue不是枚举值");
            }
            return GetDescription(enumValue.GetType(), Convert.ToInt32(enumValue));
        }

        public static string GetDescription(Type enumType, int enumValue)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException("enumType");
            }
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("enumType不是枚举类型");
            }
            EnumItem item = GetEnumItems(enumType).Find(p => p.Value == enumValue);
            if (item == null)
            {
                return "未知";
            }
            return ((item == null) ? enumValue.ToString() : item.Description);
        }

        public static List<EnumItem> GetEnumItems(Type type)
        {
            if (!dic.ContainsKey(type.Name))
            {
                FieldInfo[] fields = type.GetFields();
                List<EnumItem> list = new List<EnumItem>(fields.Length);
                foreach (FieldInfo info in fields)
                {
                    if (info.FieldType == type)
                    {
                        EnumItem item = new EnumItem {
                            Name = info.Name,
                            Value = Convert.ToInt32(info.GetRawConstantValue()),
                            Description = GetFieldDesc(info)
                        };
                        if (string.IsNullOrEmpty(item.Description))
                        {
                            item.Description = item.Name;
                        }
                        list.Add(item);
                    }
                }
                dic[type.Name] = list;
            }
            return new List<EnumItem>(dic[type.Name]);
        }

        public static List<EnumItem> GetEnumItems(Type type, int flag, int value)
        {
            FieldInfo[] fields = type.GetFields();
            List<EnumItem> list = new List<EnumItem>(fields.Length);
            foreach (FieldInfo info in fields)
            {
                if ((info.FieldType == type) && CompareItem(info, flag, value))
                {
                    EnumItem item = new EnumItem {
                        Name = info.Name,
                        Value = Convert.ToInt32(info.GetRawConstantValue()),
                        Description = GetFieldDesc(info)
                    };
                    list.Add(item);
                }
            }
            return list;
        }

        private static string GetFieldDesc(FieldInfo field_info)
        {
            object[] customAttributes = field_info.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if ((customAttributes != null) && (customAttributes.Length > 0))
            {
                return (customAttributes[0] as DescriptionAttribute).Description;
            }
            return field_info.Name;
        }
    }
}
