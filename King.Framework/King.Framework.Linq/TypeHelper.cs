namespace King.Framework.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class TypeHelper
    {
        public static Type FindIEnumerable(Type seqType)
        {
            if ((seqType != null) && (seqType != typeof(string)))
            {
                Type type2;
                if (seqType.IsArray)
                {
                    return typeof(IEnumerable<>).MakeGenericType(new Type[] { seqType.GetElementType() });
                }
                if (seqType.IsGenericType)
                {
                    foreach (Type type in seqType.GetGenericArguments())
                    {
                        type2 = typeof(IEnumerable<>).MakeGenericType(new Type[] { type });
                        if (type2.IsAssignableFrom(seqType))
                        {
                            return type2;
                        }
                    }
                }
                Type[] interfaces = seqType.GetInterfaces();
                if ((interfaces != null) && (interfaces.Length > 0))
                {
                    foreach (Type type3 in interfaces)
                    {
                        type2 = FindIEnumerable(type3);
                        if (type2 != null)
                        {
                            return type2;
                        }
                    }
                }
                if ((seqType.BaseType != null) && (seqType.BaseType != typeof(object)))
                {
                    return FindIEnumerable(seqType.BaseType);
                }
            }
            return null;
        }

        public static object GetDefault(Type type)
        {
            if (!(!type.IsValueType || IsNullableType(type)))
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        public static Type GetElementType(Type seqType)
        {
            Type type = FindIEnumerable(seqType);
            if (type == null)
            {
                return seqType;
            }
            return type.GetGenericArguments()[0];
        }

        public static Type GetMemberType(MemberInfo mi)
        {
            PropertyInfo info = mi as PropertyInfo;
            if (info != null)
            {
                return info.PropertyType;
            }
            FieldInfo info2 = mi as FieldInfo;
            if (info2 != null)
            {
                return info2.FieldType;
            }
            EventInfo info3 = mi as EventInfo;
            if (info3 != null)
            {
                return info3.EventHandlerType;
            }
            MethodInfo info4 = mi as MethodInfo;
            if (info4 != null)
            {
                return info4.ReturnType;
            }
            return null;
        }

        public static Type GetNonNullableType(Type type)
        {
            if (IsNullableType(type))
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        public static Type GetNullAssignableType(Type type)
        {
            if (!IsNullAssignable(type))
            {
                return typeof(Nullable<>).MakeGenericType(new Type[] { type });
            }
            return type;
        }

        public static ConstantExpression GetNullConstant(Type type)
        {
            return Expression.Constant(null, GetNullAssignableType(type));
        }

        public static Type GetSequenceType(Type elementType)
        {
            return typeof(IEnumerable<>).MakeGenericType(new Type[] { elementType });
        }

        public static bool IsInteger(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return true;
            }
            return false;
        }

        public static bool IsNullableType(Type type)
        {
            return (((type != null) && type.IsGenericType) && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        public static bool IsNullAssignable(Type type)
        {
            return (!type.IsValueType || IsNullableType(type));
        }

        public static bool IsReadOnly(MemberInfo member)
        {
            MemberTypes memberType = member.MemberType;
            if (memberType != MemberTypes.Field)
            {
                if (memberType != MemberTypes.Property)
                {
                    return true;
                }
            }
            else
            {
                return ((((FieldInfo) member).Attributes & FieldAttributes.InitOnly) != FieldAttributes.PrivateScope);
            }
            PropertyInfo info = (PropertyInfo) member;
            return (!info.CanWrite || (info.GetSetMethod() == null));
        }
    }
}
