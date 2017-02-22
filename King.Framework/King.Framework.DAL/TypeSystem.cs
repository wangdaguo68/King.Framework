namespace King.Framework.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal static class TypeSystem
    {
        private static Type FindIEnumerable(Type seqType)
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

        internal static Type GetElementType(Type seqType)
        {
            Type type = FindIEnumerable(seqType);
            if (type == null)
            {
                return seqType;
            }
            return type.GetGenericArguments()[0];
        }

        internal static Type GetMemberType(MemberInfo mi)
        {
            FieldInfo info = mi as FieldInfo;
            if (info != null)
            {
                return info.FieldType;
            }
            PropertyInfo info2 = mi as PropertyInfo;
            if (info2 != null)
            {
                return info2.PropertyType;
            }
            EventInfo info3 = mi as EventInfo;
            if (info3 != null)
            {
                return info3.EventHandlerType;
            }
            return null;
        }

        internal static Type GetNonNullableType(Type type)
        {
            if (IsNullableType(type))
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        internal static Type GetSequenceType(Type elementType)
        {
            return typeof(IEnumerable<>).MakeGenericType(new Type[] { elementType });
        }

        internal static bool IsNullableType(Type type)
        {
            return (((type != null) && type.IsGenericType) && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        internal static bool IsNullAssignable(Type type)
        {
            return (!type.IsValueType || IsNullableType(type));
        }
    }
}
