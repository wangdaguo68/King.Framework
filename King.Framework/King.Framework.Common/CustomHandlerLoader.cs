namespace King.Framework.Common
{
    using King.Framework.Linq;
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public class CustomHandlerLoader
    {
        public static T GetHandlerWithConfiguration<T>(string configuration, params object[] args) where T: class
        {
            if (string.IsNullOrEmpty(configuration))
            {
                throw new ApplicationException("自定义处理程序的配置为空");
            }
            string[] strArray = configuration.Split(new char[] { ',' });
            string assemblyName = strArray[1];
            string className = strArray[0];
            return GetHandlerWithName<T>(assemblyName, className, args);
        }

        public static T GetHandlerWithName<T>(string assemblyName, string className, params object[] args) where T: class
        {
            T local = default(T);
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Substring(8));
            if (!directoryName.Contains(@":\"))
            {
                directoryName = "/" + directoryName;
            }
            string str2 = Directory.GetFiles(directoryName).FirstOrDefault<string>(p => Path.GetFileNameWithoutExtension(p).ToLower() == assemblyName.ToLower());
            if (!string.IsNullOrWhiteSpace(str2))
            {
                Type type = Assembly.LoadFrom(str2).GetType(className);
                if (type != null)
                {
                    Type[] interfaces = type.GetInterfaces();
                    if ((interfaces.Count<Type>() > 0) && interfaces.Contains<Type>(typeof(T)))
                    {
                        if (args.Count<object>() == 0)
                        {
                            local = Activator.CreateInstance(type) as T;
                        }
                        else if ((args.Count<object>() == 1) && (args[0] is DataContext))
                        {
                            ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(BizDataContext) });
                            ConstructorInfo info2 = type.GetConstructor(new Type[] { typeof(DataContext) });
                            if ((constructor != null) || (info2 != null))
                            {
                                local = Activator.CreateInstance(type, args) as T;
                            }
                            else
                            {
                                local = Activator.CreateInstance(type) as T;
                            }
                        }
                        else
                        {
                            local = Activator.CreateInstance(type, args) as T;
                        }
                    }
                }
            }
            if (local == default(T))
            {
                throw new Exception(string.Format("找不到指定的插件，Assembly:{0},ClassName:{1}", assemblyName, className));
            }
            return local;
        }
    }
}
