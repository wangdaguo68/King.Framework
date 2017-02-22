namespace King.Framework.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    public static class TableCache
    {
        private static readonly Dictionary<Type, Table> _tableDict = new Dictionary<Type, Table>();
        private static readonly Dictionary<string, Type> _tableTypes = new Dictionary<string, Type>();

        public static void AppendTableType(string tableName, Type tableType)
        {
            lock (_tableTypes)
            {
                if (!_tableTypes.ContainsKey(tableName))
                {
                    _tableTypes.Add(tableName, tableType);
                }
            }
        }

        private static Type FindType(string tableName)
        {
            Assembly assembly = null;
            try
            {
                assembly = Assembly.Load(new AssemblyName("King.Framework.Entity.Auto"));
            }
            catch (Exception exception)
            {
                assembly = null;
                Trace.WriteLine(exception.Message);
            }
            if (assembly != null)
            {
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    object[] customAttributes = type.GetCustomAttributes(typeof(KingTableAttribute), false);
                    if (customAttributes.Length > 0)
                    {
                        string name = (customAttributes[0] as KingTableAttribute).Name;
                        if (string.IsNullOrEmpty(name))
                        {
                            name = type.Name;
                        }
                        if (name == tableName)
                        {
                            return type;
                        }
                    }
                }
            }
            return null;
        }

        public static Table GetTableOrCreate(Type type)
        {
            Table table;
            lock (_tableDict)
            {
                if (!_tableDict.TryGetValue(type, out table))
                {
                    table = Table.FromType(type);
                    _tableDict.Add(type, table);
                }
            }
            return table;
        }

        public static Type GetTableType(string tableName)
        {
            lock (_tableTypes)
            {
                Type type;
                if (_tableTypes.TryGetValue(tableName, out type))
                {
                    return type;
                }
            }
            Type type2 = FindType(tableName);
            if (type2 == null)
            {
                throw new ApplicationException(string.Format("不存在表{0}对应的类型", tableName));
            }
            GetTableOrCreate(type2);
            AppendTableType(tableName, type2);
            return type2;
        }
    }
}
