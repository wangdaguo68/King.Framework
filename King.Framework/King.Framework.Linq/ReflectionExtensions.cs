namespace King.Framework.Linq
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class ReflectionExtensions
    {
        public static object GetValue(this MemberInfo member, object instance)
        {
            MemberTypes memberType = member.MemberType;
            if (memberType != MemberTypes.Field)
            {
                if (memberType != MemberTypes.Property)
                {
                    throw new InvalidOperationException();
                }
                return ((PropertyInfo) member).GetValue(instance, null);
            }
            return ((FieldInfo) member).GetValue(instance);
        }

        public static void SetValue(this MemberInfo member, object instance, object value)
        {
            MemberTypes memberType = member.MemberType;
            if (memberType != MemberTypes.Field)
            {
                if (memberType != MemberTypes.Property)
                {
                    throw new InvalidOperationException();
                }
                ((PropertyInfo) member).SetValue(instance, value, null);
            }
            else
            {
                ((FieldInfo) member).SetValue(instance, value);
            }
        }
    }
}
