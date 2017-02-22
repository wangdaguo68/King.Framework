namespace King.Framework.EntityLibrary
{
    using System;
    using System.Runtime.CompilerServices;

    internal static class Extend
    {
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
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
            int result = 0;
            int.TryParse(value, out result);
            return result;
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

        public static long? ToLongNullable(this string value)
        {
            long num;
            if (!value.IsNullOrEmpty() && long.TryParse(value, out num))
            {
                return new long?(num);
            }
            return null;
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

