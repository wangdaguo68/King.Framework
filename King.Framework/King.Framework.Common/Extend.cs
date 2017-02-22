namespace King.Framework.Common
{
    using King.Framework.EntityLibrary;
    using King.Framework.LiteQueryDef.Internal;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static class Extend
    {
        private static Dictionary<Type, Func<string, object>> convertDict = new Dictionary<Type, Func<string, object>>();

        static Extend()
        {
            convertDict[typeof(int)] = p => p.ToInt();
            convertDict[typeof(int?)] = p => p.ToIntNullable();
            convertDict[typeof(long)] = p => p.ToLong();
            convertDict[typeof(long?)] = p => p.ToLongNullable();
            convertDict[typeof(bool)] = p => p.ToBool();
            convertDict[typeof(bool?)] = p => p.ToBoolNullable();
        }

        public static decimal Average(this DataTable value, string FieldName)
        {
            if ((value == null) || (value.Rows.Count == 0))
            {
                return 0M;
            }
            decimal num = 0M;
            for (int i = 0; i < value.Rows.Count; i++)
            {
                DataRow row = value.Rows[i];
                decimal result = 0M;
                decimal.TryParse(Convert.ToString(row[FieldName]), out result);
                num += result;
            }
            return (num / value.Rows.Count);
        }

        public static T ChangeType<T>(this object o)
        {
            if (o == null)
            {
                return default(T);
            }
            string arg = o.ToString();
            if (convertDict.ContainsKey(typeof(T)))
            {
                return (T) convertDict[typeof(T)](arg);
            }
            return (T) Convert.ChangeType(arg, typeof(T));
        }

        public static int Count(this DataTable value)
        {
            if (value == null)
            {
                return 0;
            }
            return value.Rows.Count;
        }

        public static T Find<T>(this IList<T> source, long? id) where T: ICachedEntity
        {
            if (source.Count != 0)
            {
                T local = source[0];
                CacheContext metaCache = local.MetaCache;
                if (metaCache != null)
                {
                    return metaCache.FindById<T>(id);
                }
            }
            return default(T);
        }

        public static LiteFilter GetFilter(this ViewQueryEx ve)
        {
            switch (ve.CompareType)
            {
                case CompareTypeEnum.LessThan:
                    return FilterField.New(ve.TableAlias, ve.FieldName).LessThan(ve.ConditionValue);

                case CompareTypeEnum.LessThanOrEqual:
                    return FilterField.New(ve.TableAlias, ve.FieldName).LessThanOrEqual(ve.ConditionValue);

                case CompareTypeEnum.Equal:
                    return FilterField.New(ve.TableAlias, ve.FieldName).Equal(ve.ConditionValue);

                case CompareTypeEnum.GreaterThanOrEqual:
                    return FilterField.New(ve.TableAlias, ve.FieldName).GreaterThanOrEqual(ve.ConditionValue);

                case CompareTypeEnum.GreaterThan:
                    return FilterField.New(ve.TableAlias, ve.FieldName).GreaterThan(ve.ConditionValue);

                case CompareTypeEnum.EndWith:
                    return FilterField.New(ve.TableAlias, ve.FieldName).EndWith(ve.ConditionValue);

                case CompareTypeEnum.StartWith:
                    return FilterField.New(ve.TableAlias, ve.FieldName).StartWith(ve.ConditionValue);

                case CompareTypeEnum.Contains:
                    return FilterField.New(ve.TableAlias, ve.FieldName).Contains(ve.ConditionValue);

                case CompareTypeEnum.NotEqual:
                    return FilterField.New(ve.TableAlias, ve.FieldName).NotEqual(ve.ConditionValue);

                case CompareTypeEnum.IN:
                    return FilterField.New(ve.TableAlias, ve.FieldName).IN(ve.SubQuery);

                case CompareTypeEnum.IsNull:
                    return FilterField.New(ve.TableAlias, ve.FieldName).IsNull();

                case CompareTypeEnum.IsNotNull:
                    return FilterField.New(ve.TableAlias, ve.FieldName).IsNotNull();
            }
            throw new Exception(string.Format("暂不支持动态添加{0}类型查询条件！", CompareTypeEnum.StartWith.ToString()));
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static decimal Max(this DataTable value, string FieldName)
        {
            if ((value == null) || (value.Rows.Count == 0))
            {
                return 0M;
            }
            decimal num = -79228162514264337593543950335M;
            for (int i = 0; i < value.Rows.Count; i++)
            {
                DataRow row = value.Rows[i];
                decimal result = 0M;
                decimal.TryParse(Convert.ToString(row[FieldName]), out result);
                if (result > num)
                {
                    num = result;
                }
            }
            return num;
        }

        public static decimal Min(this DataTable value, string FieldName)
        {
            if ((value == null) || (value.Rows.Count == 0))
            {
                return 0M;
            }
            decimal num = 79228162514264337593543950335M;
            for (int i = 0; i < value.Rows.Count; i++)
            {
                DataRow row = value.Rows[i];
                decimal result = 0M;
                decimal.TryParse(Convert.ToString(row[FieldName]), out result);
                if (result < num)
                {
                    num = result;
                }
            }
            return num;
        }

        public static int RangedTo(this int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }
            if (value > max)
            {
                return max;
            }
            return value;
        }

        public static decimal Sum(this DataTable value, string FieldName)
        {
            if ((value == null) || (value.Rows.Count == 0))
            {
                return 0M;
            }
            decimal num = 0M;
            for (int i = 0; i < value.Rows.Count; i++)
            {
                DataRow row = value.Rows[i];
                decimal result = 0M;
                decimal.TryParse(Convert.ToString(row[FieldName]), out result);
                num += result;
            }
            return num;
        }

        public static bool ToBool(this int? value)
        {
            if (value != 1)
            {
                return false;
            }
            return true;
        }

        public static bool ToBool(this object o)
        {
            return Convert.ToString(o).ToBool();
        }

        public static bool ToBool(this string value)
        {
            bool? nullable2 = value.ToBoolNullable();
            return (nullable2.HasValue ? nullable2.GetValueOrDefault() : false);
        }

        public static bool? ToBoolNullable(this string value)
        {
            if (!value.IsNullOrEmpty())
            {
                if ((value == "1") || (value.ToLower() == "true"))
                {
                    return true;
                }
                if ((value == "0") || (value.ToLower() == "false"))
                {
                    return false;
                }
            }
            return null;
        }

        public static decimal ToDecimal(this string value)
        {
            decimal result = 0M;
            decimal.TryParse(value, out result);
            return result;
        }

        public static decimal? ToDecimalNullable(this string value)
        {
            decimal num;
            if (!value.IsNullOrEmpty() && decimal.TryParse(value, out num))
            {
                return new decimal?(num);
            }
            return null;
        }

        public static int ToInt(this bool value)
        {
            if (value)
            {
                return 1;
            }
            return 0;
        }

        public static int ToInt(this object o)
        {
            return Convert.ToString(o).ToInt();
        }

        public static int ToInt(this string value)
        {
            int num;
            if (int.TryParse(value, out num))
            {
                return num;
            }
            return 0;
        }

        public static int ToInt(this string value, int defaultValue)
        {
            int num;
            if (!string.IsNullOrWhiteSpace(value) && int.TryParse(value, out num))
            {
                return num;
            }
            return defaultValue;
        }

        public static int? ToIntNullable(this string value)
        {
            int num;
            if (!value.IsNullOrEmpty() && int.TryParse(value, out num))
            {
                return new int?(num);
            }
            return null;
        }

        public static long ToLong(this object o)
        {
            return Convert.ToString(o).ToLong();
        }

        public static long ToLong(this string value)
        {
            long result = 0L;
            long.TryParse(value, out result);
            return result;
        }

        public static long ToLong(this string value, long defaultValue = -1L)
        {
            long num;
            if (!string.IsNullOrWhiteSpace(value) && long.TryParse(value, out num))
            {
                return num;
            }
            return defaultValue;
        }

        public static long? ToLongNullable(this string value)
        {
            long num;
            if (!value.IsNullOrEmpty() && long.TryParse(value, out num))
            {
                return new long?(num);
            }
            return null;
        }

        public static string ToPingYin(this string str)
        {
            ConvertPingYin yin = new ConvertPingYin();
            return yin.Convert(str, false);
        }

        public static string ToPingYinFirst(this string str)
        {
            ConvertPingYin yin = new ConvertPingYin();
            return yin.Convert(str, true);
        }

        public static string ToStringNullable(this object o)
        {
            if (o == null)
            {
                return null;
            }
            return o.ToString();
        }
    }
}
