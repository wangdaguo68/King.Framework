namespace King.Framework.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class ObjectConverter
    {
        private static Dictionary<string, List<CommonProperty>> DicCommonProperty = new Dictionary<string, List<CommonProperty>>();

        public static List<TResult> ConvertTo<TResult>(this IEnumerable source) where TResult : new()
        {
            if (source == null)
            {
                return new List<TResult>();
            }
            if (source is IEnumerable<TResult>)
            {
                return source.Cast<TResult>().ToList<TResult>();
            }
            List<TResult> list = new List<TResult>();
            IEnumerable<CommonProperty> commonProperties = null;
            foreach (object obj2 in source)
            {
                if (obj2 is TResult)
                {
                    return source.Cast<TResult>().ToList<TResult>();
                }
                if (commonProperties == null)
                {
                    Type sourceType = obj2.GetType();
                    if (DicCommonProperty.ContainsKey(sourceType.FullName))
                    {
                        commonProperties = DicCommonProperty[sourceType.FullName];
                    }
                    else
                    {
                        commonProperties = GetCommonProperties(sourceType, typeof(TResult));
                    }
                }
                TResult local2 = default(TResult);
                TResult local = (local2 == null) ? Activator.CreateInstance<TResult>() : (local2 = default(TResult));
                foreach (CommonProperty property in commonProperties)
                {
                    object obj3 = property.SourceProperty.GetValue(obj2, null);
                    property.TargetProperty.SetValue(local, obj3, null);
                }
                list.Add(local);
            }
            return list;
        }

        public static TResult ConvertTo<TResult>(this object source) where TResult : new()
        {
            if (source == null)
            {
                return default(TResult);
            }
            TResult local = new TResult();
            IEnumerable<CommonProperty> commonProperties = null;
            Type sourceType = source.GetType();
            if (DicCommonProperty.ContainsKey(sourceType.FullName))
            {
                commonProperties = DicCommonProperty[sourceType.FullName];
            }
            else
            {
                commonProperties = GetCommonProperties(sourceType, typeof(TResult));
            }
            foreach (CommonProperty property in commonProperties)
            {
                object obj2 = property.SourceProperty.GetValue(source, null);
                property.TargetProperty.SetValue(local, obj2, null);
            }
            return local;
        }

        private static IEnumerable<CommonProperty> GetCommonProperties(Type sourceType, Type targetType)
        {
            PropertyInfo[] properties = sourceType.GetProperties();
            PropertyInfo[] infoArray2 = targetType.GetProperties();
            return (from SP in properties
                    join TP in infoArray2 on SP.Name.ToLower() equals TP.Name.ToLower()
                    where SP.CanRead && TP.CanWrite
                    select new CommonProperty { SourceProperty = SP, TargetProperty = TP });
        }

        private class CommonProperty
        {
            public PropertyInfo SourceProperty { get; set; }

            public PropertyInfo TargetProperty { get; set; }
        }
    }
}
