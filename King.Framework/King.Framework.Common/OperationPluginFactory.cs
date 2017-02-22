namespace King.Framework.Common
{
    using King.Framework.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    internal static class OperationPluginFactory
    {
        private static readonly bool _IsEnableOperationPlugin = GetIsEnableOperationPlugin();
        internal static readonly Dictionary<string, IOperationPlugin> operationPlugins = new Dictionary<string, IOperationPlugin>();
        public static readonly string PluginAssemblyNotPrefix = "King.Framework.";
        public static readonly string PluginAssemblyPrefix = "King.";

        static OperationPluginFactory()
        {
            if (_IsEnableOperationPlugin)
            {
                LoadPluginHandlers();
            }
        }

        private static void FindOperationPlugin(Type plugin_type)
        {
            MethodAttribute attribute = plugin_type.GetCustomAttributes(typeof(MethodAttribute), false).FirstOrDefault<object>() as MethodAttribute;
            if (attribute != null)
            {
                string key = string.Format("{0}${1}", attribute.EntityType.Name, attribute.OperationName);
                if (operationPlugins.ContainsKey(key))
                {
                    throw new ApplicationExceptionFormat("实体{0}存在重复插件{1}", new object[] { attribute.EntityType.Name, attribute.OperationName });
                }
                IOperationPlugin plugin = Activator.CreateInstance(plugin_type) as IOperationPlugin;
                operationPlugins.Add(key, plugin);
            }
        }

        private static bool GetIsEnableOperationPlugin()
        {
            string str = ConfigurationManager.AppSettings["enableOperationPlugin"];
            return !(string.IsNullOrWhiteSpace(str) || !(str.Trim().ToLower() == "true"));
        }

        public static IOperationPlugin GetOperationPlugin(string MethodKey)
        {
            if (operationPlugins.ContainsKey(MethodKey))
            {
                return operationPlugins[MethodKey];
            }
            return null;
        }

        private static void LoadPluginHandlers()
        {
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Substring(8));
            if (!directoryName.Contains(@":\"))
            {
                directoryName = "/" + directoryName;
            }
            List<string> list = (from p in Directory.GetFiles(directoryName)
                where (((Path.GetFileName(p).StartsWith(PluginAssemblyPrefix) && !Path.GetFileName(p).StartsWith(PluginAssemblyNotPrefix)) && (Path.GetExtension(p).ToLower() == ".dll")) || (Path.GetFileName(p) == "King.Framework.Web.dll")) || (Path.GetFileName(p) == "King.Framework.PluginImpl.dll")
                select p).ToList<string>();
            foreach (string str2 in list)
            {
                Assembly assembly = Assembly.LoadFrom(str2);
                if (assembly != null)
                {
                    List<Type> list2 = new List<Type>();
                    try
                    {
                        list2 = (from p in assembly.GetTypes()
                            where p.IsClass
                            select p).ToList<Type>();
                    }
                    catch (ReflectionTypeLoadException exception)
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.AppendFormat("加载程序集[{0}]的类型时出错\r\n", assembly.FullName);
                        builder.AppendLine(exception.Message);
                        foreach (Exception exception2 in exception.LoaderExceptions)
                        {
                            builder.AppendLine(exception2.Message);
                            if (exception2.InnerException != null)
                            {
                                builder.AppendLine(exception2.InnerException.Message);
                            }
                        }
                        Trace.WriteLine(builder.ToString());
                        throw new Exception(builder.ToString());
                    }
                    foreach (Type type in list2)
                    {
                        FindOperationPlugin(type);
                    }
                }
            }
        }

        public static bool IsEnableOperationPlugin
        {
            get
            {
                return _IsEnableOperationPlugin;
            }
        }
    }
}
