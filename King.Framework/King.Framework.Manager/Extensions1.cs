namespace King.Framework.Manager
{
    using King.Framework.Repository.Schemas;
    using System;
    using System.Runtime.CompilerServices;

    public static class Extensions1
    {
        private static string DynamicNameSuffix = "_Dynamic_DisplayValue";

        private static string BoolPropertyDynamicName(string propertyName)
        {
            return string.Format("{0}{1}", propertyName, DynamicNameSuffix);
        }

        private static string DateTimePropertyDynamicName(string propertyName)
        {
            return string.Format("{0}{1}", propertyName, DynamicNameSuffix);
        }

        private static string EnumPropertyDynamicName(string propertyName)
        {
            return string.Format("{0}{1}", propertyName, DynamicNameSuffix);
        }

        private static string FilePropertyDynamicName(string propertyName)
        {
            return string.Format("{0}{1}", propertyName, DynamicNameSuffix);
        }

        private static string GetDynamicEntityName(this IEntitySchema es)
        {
            return string.Format("King.Framework.Entity.Dynamic.{0}", es.EntityName);
        }

        public static string ReplaceDynamicNameSuffix(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            return s.Replace(DynamicNameSuffix, "");
        }

        public static string ReplaceToBr(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            return s.Replace("\r\n", "<br />").Replace("\r", "<br />").Replace("\n", "<br />");
        }
    }
}
