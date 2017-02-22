using System.Linq;

namespace King.Framework.Manager
{
    using King.Framework.Common;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class EnumUtil
    {
        private static readonly Dictionary<string, Type> enumDict = new Dictionary<string, Type>();

        static EnumUtil()
        {
            IEnumerable<Assembly> enumerable = from a in AppDomain.CurrentDomain.GetAssemblies()
                where a.FullName.StartsWith("King.Framework.Enum")
                select a;
            foreach (Assembly assembly in enumerable)
            {
                foreach (Type type in from x in assembly.GetTypes()
                    where x.BaseType == typeof(Enum)
                    select x)
                {
                    enumDict.Add(type.Name, type);
                }
            }
        }

        public static string ValueToDescription(string enumName, int? value)
        {
            if (value.HasValue && enumDict.ContainsKey(enumName))
            {
                return EnumHelper.GetDescription(enumDict[enumName], value.Value);
            }
            return "";
        }
    }
}
