namespace King.Framework.Common
{
    using King.Framework.Common.Util;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static class PropertyExtensions
    {
        private static Dictionary<Type, Func<object, object>> ConvertFuns;

        static PropertyExtensions()
        {
            Dictionary<Type, Func<object, object>> dictionary = new Dictionary<Type, Func<object, object>>();
            dictionary.Add(typeof(int), o => ToInt(o, false));
            dictionary.Add(typeof(int?), o => ToInt(o, true));
            dictionary.Add(typeof(DateTime), o => ToDateTime(o, false));
            dictionary.Add(typeof(DateTime?), o => ToDateTime(o, true));
            dictionary.Add(typeof(float), o => ToFloat(o, false));
            dictionary.Add(typeof(float?), o => ToFloat(o, true));
            dictionary.Add(typeof(double), o => ToDouble(o, false));
            dictionary.Add(typeof(double?), o => ToDouble(o, true));
            dictionary.Add(typeof(string), o => TString(o, true));
            dictionary.Add(typeof(decimal), o => ToDecimal(o, false));
            dictionary.Add(typeof(decimal?), o => ToDecimal(o, true));
            dictionary.Add(typeof(long), o => ToLong(o, false));
            dictionary.Add(typeof(long?), o => ToLong(o, true));
            dictionary.Add(typeof(bool), o => ToBool(o, false));
            dictionary.Add(typeof(bool?), o => ToBool(o, true));
            ConvertFuns = dictionary;
        }

        public static object GetPropertyValue(this object targetObj, string propertyName, Type targetType = null)
        {
            object obj2;
            if (targetType == null)
            {
                targetType = targetObj.GetType();
            }
            try
            {
                obj2 = new PropertyAccessor(targetType, propertyName).Get(targetObj);
            }
            catch (Exception)
            {
                throw new Exception(string.Format("{0} 的 {1} 属性不存在!", targetType.Name, propertyName));
            }
            return obj2;
        }

        private static void SetConvertValue(object targetObj, string propertyName, object value, Type targetType, Type memberType)
        {
            if (!ConvertFuns.ContainsKey(memberType))
            {
                throw new SetPropertyValueException(string.Format("当前属性[{0}]的类型[{1}]不支持转换", propertyName, memberType));
            }
            value = ConvertFuns[memberType](value);
            new PropertyAccessor(targetType, propertyName).Set(targetObj, value);
        }

        public static void SetPropertyConvertValue(this object targetObj, string propertyName, object value)
        {
            Type type = targetObj.GetType();
            PropertyInfo property = type.GetProperty(propertyName);
            if (property == null)
            {
                throw new SetPropertyValueException(string.Format("当前属性[{0}]不是类型[{1}]的公共属性或不存在", propertyName, type));
            }
            Type propertyType = property.PropertyType;
            SetConvertValue(targetObj, propertyName, value, type, propertyType);
        }

        public static void SetPropertyValue<T>(this object targetObj, Expression<Func<T>> e, object value)
        {
            MemberExpression body;
            try
            {
                body = (MemberExpression) e.Body;
            }
            catch (Exception)
            {
                throw new SetPropertyValueException(string.Format("表达式不正确,只能是属性表达式", new object[0]));
            }
            Expression expression2 = body.Expression;
            if (expression2.Type != targetObj.GetType())
            {
                throw new SetPropertyValueException(string.Format("表达式不正确,必须是实体本身属性的指向", new object[0]));
            }
            Type memberType = typeof(T);
            string name = body.Member.Name;
            SetConvertValue(targetObj, name, value, expression2.Type, memberType);
        }

        public static void SetPropertyValue(this object targetObj, string propertyName, object value, Type targetType = null)
        {
            if (targetType == null)
            {
                targetType = targetObj.GetType();
            }
            new PropertyAccessor(targetType, propertyName).Set(targetObj, value);
        }

        private static bool? ToBool(object val, bool isNullable = true)
        {
            bool flag;
            if ((val == null) || !bool.TryParse(val.ToString(), out flag))
            {
                if (!isNullable)
                {
                    return false;
                }
                return null;
            }
            return new bool?(flag);
        }

        private static DateTime? ToDateTime(object val, bool isNullable = true)
        {
            DateTime time;
            if ((val == null) || !DateTime.TryParse(val.ToString(), out time))
            {
                if (!isNullable)
                {
                    return new DateTime();
                }
                return null;
            }
            return new DateTime?(time);
        }

        private static decimal? ToDecimal(object val, bool isNullable = true)
        {
            decimal num;
            if ((val == null) || !decimal.TryParse(val.ToString(), out num))
            {
                if (!isNullable)
                {
                    return new decimal?(decimal.Parse("0"));
                }
                return null;
            }
            return new decimal?(num);
        }

        private static double? ToDouble(object val, bool isNullable = true)
        {
            double num;
            if ((val == null) || !double.TryParse(val.ToString(), out num))
            {
                if (!isNullable)
                {
                    return 0.0;
                }
                return null;
            }
            return new double?(num);
        }

        private static float? ToFloat(object val, bool isNullable = true)
        {
            float num;
            if ((val == null) || !float.TryParse(val.ToString(), out num))
            {
                if (!isNullable)
                {
                    return 0f;
                }
                return null;
            }
            return new float?(num);
        }

        private static int? ToInt(object val, bool isNullable = true)
        {
            int result = 0;
            if ((val == null) || !int.TryParse(val.ToString(), out result))
            {
                if (!isNullable)
                {
                    return 0;
                }
                return null;
            }
            return new int?(result);
        }

        private static long? ToLong(object val, bool isNullable = true)
        {
            long num;
            if ((val == null) || !long.TryParse(val.ToString(), out num))
            {
                if (!isNullable)
                {
                    return 0L;
                }
                return null;
            }
            return new long?(num);
        }

        private static string TString(object val, bool isNullable = true)
        {
            string str = Convert.ToString(val);
            if (string.IsNullOrEmpty(str))
            {
                if (!isNullable)
                {
                    return "";
                }
                return null;
            }
            return str;
        }
    }
}
