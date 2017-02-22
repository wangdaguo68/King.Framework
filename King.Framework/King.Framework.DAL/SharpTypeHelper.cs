

namespace King.Framework.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using King.Framework.DAL.CSharpTypes;
    using King.Framework.DAL.DataAccessLib;
    public static class SharpTypeHelper
    {
        private static readonly ISharpType enumHelper = new EnumType();
        private static readonly Dictionary<Type, ISharpType> helpers = new Dictionary<Type, ISharpType>(50);
        private static readonly ISharpType nullableEnumHelper = new EnumNullableType();

        static SharpTypeHelper()
        {
            helpers.Add(typeof(string), new StringType());
            helpers.Add(typeof(int), new IntType());
            helpers.Add(typeof(int?), new IntNullableType());
            helpers.Add(typeof(long), new LongType());
            helpers.Add(typeof(long?), new LongNullableType());
            helpers.Add(typeof(decimal), new DecimalType());
            helpers.Add(typeof(decimal?), new DecimalNullableType());
            helpers.Add(typeof(DateTime), new DateTimeType());
            helpers.Add(typeof(DateTime?), new DateTimeNullableType());
            helpers.Add(typeof(bool), new BoolType());
            helpers.Add(typeof(bool?), new BoolNullableType());
            helpers.Add(typeof(Guid), new GuidType());
            helpers.Add(typeof(Guid?), new GuidNullableType());
            helpers.Add(typeof(byte[]), new ByteArrayType());
            helpers.Add(typeof(byte), new ByteType());
            helpers.Add(typeof(byte?), new ByteNullableType());
            helpers.Add(typeof(short), new ShortType());
            helpers.Add(typeof(short?), new ShortNullableType());
            helpers.Add(typeof(double), new DoubleType());
            helpers.Add(typeof(double?), new DoubleNullableType());
        }

        public static bool AllowToBeKey(Column col)
        {
            return GetShartTypeHelper(col.DataType).AllowToBeKey;
        }

        public static object ConvertFrom(Type type, object value)
        {
            return GetShartTypeHelper(type).ConvertFrom(value, type);
        }

        public static Func<object, object> CreateGetFunction(Type targetType, string propertyName)
        {
            ParameterExpression expression;
            PropertyInfo property = targetType.GetProperty(propertyName);
            MethodInfo getMethod = property.GetGetMethod();
            UnaryExpression expression2 = getMethod.IsStatic ? null : Expression.Convert(expression = Expression.Parameter(typeof(object), "target"), targetType);
            return Expression.Lambda<Func<object, object>>(Expression.Convert(Expression.Property(expression2, property), typeof(object)), new ParameterExpression[] { Expression.Parameter(typeof(object), "target") }).Compile();
        }

        public static Action<object, object> CreateSetAction(Type targetType, string propertyName)
        {
            ParameterExpression expression;
            ParameterExpression expression2;
            PropertyInfo property = targetType.GetProperty(propertyName);
            MethodInfo setMethod = property.GetSetMethod();
            UnaryExpression instance = setMethod.IsStatic ? null : Expression.Convert(expression = Expression.Parameter(typeof(object), "target"), targetType);
            UnaryExpression expression4 = Expression.Convert(expression2 = Expression.Parameter(typeof(object), "value"), property.PropertyType);
            return Expression.Lambda<Action<object, object>>(Expression.Call(instance, setMethod, new Expression[] { expression4 }), new ParameterExpression[] { Expression.Parameter(typeof(object), "target"), expression2 }).Compile();
        }

        public static void FormatAsSql(Type type, object obj)
        {
            GetShartTypeHelper(type).FormatAsSql(obj);
        }

        public static object GetDefaultValue(Type parameter)
        {
            return typeof(DefaultGenerator<>).MakeGenericType(new Type[] { parameter }).InvokeMember("GetDefault", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new object[0]);
        }

        public static string GetOracleTypeString(Column col)
        {
            return GetShartTypeHelper(col.DataType).GetOracleTypeString(col);
        }

        internal static ISharpType GetShartTypeHelper(Type type)
        {
            ISharpType type2;
            if (helpers.TryGetValue(type, out type2))
            {
                return type2;
            }
            if (type.IsEnum)
            {
                return enumHelper;
            }
            if (!BaseDataAccess.IsNullableEnum(type))
            {
                throw new ApplicationException(string.Format("不支持的类型: {0}", type));
            }
            return nullableEnumHelper;
        }

        public static string GetSqlTypeString(Column col)
        {
            return GetShartTypeHelper(col.DataType).GetSqlTypeString(col);
        }

        public static void SetValue(object obj, Column col, object value)
        {
            GetShartTypeHelper(col.DataType).SetValue(obj, col, value);
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
