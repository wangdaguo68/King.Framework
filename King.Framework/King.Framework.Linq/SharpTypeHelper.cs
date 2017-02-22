namespace King.Framework.Linq
{
    using King.Framework.Linq.CSharpTypes;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal static class SharpTypeHelper
    {
        private static readonly ISharpType enumHelper = new EnumType();
        private static readonly ISharpType nullableEnumHelper = new EnumNullableType();
        private static readonly Dictionary<Type, ISharpType> oracleTypehelpers;
        private static readonly Dictionary<Type, ISharpType> sqlTypehelpers = new Dictionary<Type, ISharpType>(50);

        static SharpTypeHelper()
        {
            sqlTypehelpers.Add(typeof(string), new StringType());
            sqlTypehelpers.Add(typeof(int), new IntType());
            sqlTypehelpers.Add(typeof(int?), new IntNullableType());
            sqlTypehelpers.Add(typeof(long), new LongType());
            sqlTypehelpers.Add(typeof(long?), new LongNullableType());
            sqlTypehelpers.Add(typeof(decimal), new DecimalType());
            sqlTypehelpers.Add(typeof(decimal?), new DecimalNullableType());
            sqlTypehelpers.Add(typeof(DateTime), new DateTimeType());
            sqlTypehelpers.Add(typeof(DateTime?), new DateTimeNullableType());
            sqlTypehelpers.Add(typeof(bool), new BoolType());
            sqlTypehelpers.Add(typeof(bool?), new BoolNullableType());
            sqlTypehelpers.Add(typeof(Guid), new GuidType());
            sqlTypehelpers.Add(typeof(Guid?), new GuidNullableType());
            sqlTypehelpers.Add(typeof(byte[]), new ByteArrayType());
            sqlTypehelpers.Add(typeof(byte), new ByteType());
            sqlTypehelpers.Add(typeof(byte?), new ByteNullableType());
            sqlTypehelpers.Add(typeof(short), new ShortType());
            sqlTypehelpers.Add(typeof(short?), new ShortNullableType());
            sqlTypehelpers.Add(typeof(double), new DoubleType());
            sqlTypehelpers.Add(typeof(double?), new DoubleNullableType());
            oracleTypehelpers = new Dictionary<Type, ISharpType>(50);
            oracleTypehelpers.Add(typeof(string), new StringType_Oracle() as ISharpType);
            oracleTypehelpers.Add(typeof(int), new IntType_Oracle() as ISharpType);
            oracleTypehelpers.Add(typeof(int?), new IntNullableType_Oracle() as ISharpType);
            oracleTypehelpers.Add(typeof(long), new LongType_Oracle() as ISharpType);
            oracleTypehelpers.Add(typeof(long?), new LongNullableType_Oracle() as ISharpType);
            oracleTypehelpers.Add(typeof(decimal), new DecimalType_Oracle() as ISharpType);
            oracleTypehelpers.Add(typeof(decimal?), new DecimalNullableType_Oracle() as ISharpType);
            oracleTypehelpers.Add(typeof(DateTime), new DateTimeType_Oracle() as ISharpType);
            oracleTypehelpers.Add(typeof(DateTime?), new DateTimeNullableType_Oracle() as ISharpType);
            oracleTypehelpers.Add(typeof(bool), new BoolType_Oracle() as ISharpType);
            oracleTypehelpers.Add(typeof(bool?), new BoolNullableType_Oracle() as ISharpType);
            oracleTypehelpers.Add(typeof(Guid), new GuidType_Oracle() as ISharpType);
            oracleTypehelpers.Add(typeof(Guid?), new GuidNullableType_Oracle() as ISharpType);
            oracleTypehelpers.Add(typeof(byte[]), new ByteArrayType_Oracle() as ISharpType);
            oracleTypehelpers.Add(typeof(byte), new ByteType_Oracle() as ISharpType);
            oracleTypehelpers.Add(typeof(byte?), new ByteNullableType_Oracle() as ISharpType);
            oracleTypehelpers.Add(typeof(short), new ShortType_Oracle() as ISharpType);
            oracleTypehelpers.Add(typeof(short?), new ShortNullableType_Oracle() as ISharpType);
            oracleTypehelpers.Add(typeof(double), new DoubleType_Oracle() as ISharpType);
            oracleTypehelpers.Add(typeof(double?), new DoubleNullableType_Oracle() as ISharpType);
        }

        public static string Format(object value)
        {
            return GetShartTypeHelper(value.GetType()).FormatAsSql(value);
        }

        public static string FormatOracle(object value)
        {
            return GetShartTypeHelper_Oracle(value.GetType()).FormatAsSql(value);
        }

        internal static object GetDefaultValue(Type parameter)
        {
            return typeof(DefaultGenerator<>).MakeGenericType(new Type[] { parameter }).InvokeMember("GetDefault", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new object[0]);
        }

        internal static ISharpType GetShartTypeHelper(Type type)
        {
            ISharpType type2;
            if (sqlTypehelpers.TryGetValue(type, out type2))
            {
                return type2;
            }
            if (type.IsEnum)
            {
                return enumHelper;
            }
            if (!IsNullableEnum(type))
            {
                throw new ApplicationException("不支持的类型:" + type.ToString());
            }
            return nullableEnumHelper;
        }

        internal static ISharpType GetShartTypeHelper_Oracle(Type type)
        {
            ISharpType type2;
            if (oracleTypehelpers.TryGetValue(type, out type2))
            {
                return type2;
            }
            if (type.IsEnum)
            {
                return enumHelper;
            }
            if (!IsNullableEnum(type))
            {
                throw new ApplicationException("不支持的类型:" + type.ToString());
            }
            return nullableEnumHelper;
        }

        public static bool IsEnumOrNullableEnum(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return (type.IsEnum || IsNullableEnum(type));
        }

        public static bool IsGenericNullableType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        public static bool IsNullableEnum(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                Type type2 = type.GetGenericArguments()[0];
                if (type2.IsEnum)
                {
                    return true;
                }
            }
            return false;
        }

        private class DefaultGenerator<T>
        {
            public static T GetDefault()
            {
                return default(T);
            }
        }
    }
}
