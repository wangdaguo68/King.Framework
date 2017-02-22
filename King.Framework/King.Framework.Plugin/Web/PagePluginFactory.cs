namespace King.Framework.Plugin.Web
{
    using King.Framework.Interfaces;
    using King.Framework.Plugin;
    using King.Framework.Plugin.Aop;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Web.UI;

    public static class PagePluginFactory
    {
        public static readonly string PluginAssemblyNotPrefix = "King.Framework.";
        public static readonly string PluginAssemblyPrefix = "King.";
        public static readonly Dictionary<string, PagePluginHandler> pluginHandlers = new Dictionary<string, PagePluginHandler>();
        public static readonly Dictionary<string, Type> queryPlugins = new Dictionary<string, Type>();

        static PagePluginFactory()
        {
            LoadPluginHandlers();
        }

        private static void FindPagePlugin(Type plugin_type)
        {
            PagePluginAttribute attribute = plugin_type.GetCustomAttributes(typeof(PagePluginAttribute), false).FirstOrDefault<object>() as PagePluginAttribute;
            if ((attribute != null) && !string.IsNullOrWhiteSpace(attribute.PageName))
            {
                string key = attribute.PageName.ToLower();
                if (pluginHandlers.ContainsKey(key))
                {
                    Type pluginType = pluginHandlers[key].pluginType;
                    throw new ApplicationExceptionFormat("页面{0}存在重复插件{1}和{2}", new object[] { attribute.PageName, plugin_type, pluginType });
                }
                PagePluginHandler handler = new PagePluginHandler(attribute, plugin_type);
                pluginHandlers.Add(key, handler);
            }
        }

        private static void FindViewQueryPlugin(Type plugin_type)
        {
            ViewQueryPluginAttribute attribute = plugin_type.GetCustomAttributes(typeof(ViewQueryPluginAttribute), false).FirstOrDefault<object>() as ViewQueryPluginAttribute;
            if (attribute != null)
            {
                Type baseType = plugin_type;
                while ((baseType != typeof(object)) && (baseType.Name != "ViewQueryBase"))
                {
                    baseType = baseType.BaseType;
                }
                if (baseType == typeof(object))
                {
                    throw new ApplicationException("查询类插件必须继承自要重写的查询类");
                }
                string name = plugin_type.BaseType.Name;
                if (name == "ViewQueryBase")
                {
                    throw new ApplicationException("查询类插件不能直接继承于ViewQueryBase，请继承自要重写的查询类");
                }
                bool flag = false;
                ConstructorInfo[] constructors = plugin_type.GetConstructors();
                foreach (ConstructorInfo info in constructors)
                {
                    ParameterInfo[] parameters = info.GetParameters();
                    if (((parameters.Count<ParameterInfo>() == 2) && (parameters[0].ParameterType.Name == "BizDataContext")) && (parameters[1].ParameterType.Name == "T_User"))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    throw new ApplicationException("查询类插件必须有以BizDataContext和T_User为参数的构造函数");
                }
                if (queryPlugins.ContainsKey(name))
                {
                    throw new ApplicationExceptionFormat("查询类{0}存在重复插件", new object[] { name });
                }
                queryPlugins.Add(name, plugin_type);
            }
        }

        private static CommandHandler GetCommandPluginHandler(Control sender)
        {
            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }
            string pagePluginKey = GetPagePluginKey(sender.GetAbsolutePage());
            if (pluginHandlers.ContainsKey(pagePluginKey))
            {
                PagePluginHandler handler = pluginHandlers[pagePluginKey];
                if (handler.CommandHandlers.ContainsKey(sender.ID))
                {
                    return handler.CommandHandlers[sender.ID];
                }
            }
            return null;
        }

        internal static string GetPagePluginKey(Page page)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }
            Type baseType = page.GetType();
            if (baseType.Name.ToLower().EndsWith("_aspx"))
            {
                baseType = baseType.BaseType;
            }
            return baseType.Name.ToLower();
        }

        public static IPagePlugin GetPlugin(Page page)
        {
            string pagePluginKey = GetPagePluginKey(page);
            if (pluginHandlers.ContainsKey(pagePluginKey))
            {
                PagePluginHandler handler = pluginHandlers[pagePluginKey];
                if (handler.pluginAttribute.Reusable)
                {
                    return handler.GetPluginInstance();
                }
                IPagePlugin plugin = Activator.CreateInstance(handler.pluginType) as IPagePlugin;
                if (plugin == null)
                {
                    throw new ApplicationExceptionFormat("创建页面{0}插件{1}的实例失败", new object[] { handler.pluginAttribute.PageName, handler.pluginType });
                }
                return plugin;
            }
            return null;
        }

        public static Type GetQueryPluginType(string key)
        {
            if (queryPlugins.ContainsKey(key))
            {
                return queryPlugins[key];
            }
            return null;
        }

        public static void InvokeAfterPlugins(object sender, PluginEventArgs e)
        {
            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            Control ctrl = sender as Control;
            if (ctrl == null)
            {
                throw new ApplicationExceptionFormat("传的sender {0}不是服务器控件", new object[] { sender.ToString() });
            }
            Page absolutePage = ctrl.GetAbsolutePage();
            IBasePage page2 = absolutePage as IBasePage;
            if (page2 == null)
            {
                throw new ApplicationExceptionFormat("页面不是继承自BasePage", new object[0]);
            }
            IPagePlugin pluginInstance = page2.PluginInstance;
            if (pluginInstance != null)
            {
                CommandHandler commandPluginHandler = GetCommandPluginHandler(ctrl);
                if (commandPluginHandler != null)
                {
                    commandPluginHandler.ExecuteAfter(pluginInstance, ctrl, e);
                }
            }
            EventCallHelper.InvokeAfter(absolutePage, ctrl, new object[] { e.entityModel, e.RedirectURL, e.CurrentUserID });
        }

        public static void InvokeBeforePlugins(object sender, PluginEventArgs e)
        {
            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            Control ctrl = sender as Control;
            if (ctrl == null)
            {
                throw new ApplicationExceptionFormat("传的sender {0}不是服务器控件", new object[] { sender.ToString() });
            }
            Page absolutePage = ctrl.GetAbsolutePage();
            IBasePage page2 = absolutePage as IBasePage;
            if (page2 == null)
            {
                throw new ApplicationExceptionFormat("页面不是继承自BasePage", new object[0]);
            }
            IPagePlugin pluginInstance = page2.PluginInstance;
            if (pluginInstance != null)
            {
                CommandHandler commandPluginHandler = GetCommandPluginHandler(ctrl);
                if (commandPluginHandler != null)
                {
                    commandPluginHandler.ExecuteBefore(pluginInstance, ctrl, e);
                }
            }
            EventCallHelper.InvokeBefore(absolutePage, ctrl, new object[] { e.entityModel, e.RedirectURL, e.CurrentUserID });
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
                        list2 = assembly.GetTypes().ToList<Type>();
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
                        FindPagePlugin(type);
                        EventCallHelper.FindPluginInType(type);
                        FindViewQueryPlugin(type);
                    }
                }
            }
        }
    }
}

