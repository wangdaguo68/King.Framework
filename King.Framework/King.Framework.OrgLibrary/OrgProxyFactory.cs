namespace King.Framework.OrgLibrary
{
    using King.Framework.Common;
    using King.Framework.Interfaces;
    using King.Framework.Ioc;
    using King.Framework.Linq;
    using System;
    using System.Configuration;
    using System.Runtime.InteropServices;

    public static class OrgProxyFactory
    {
        public static IOrgProxy GetProxy(DataContext ctx = null)
        {
            if (IocHelper.IsRegistered(typeof(IOrgProxy)))
            {
                return IocHelper.GetInstance<IOrgProxy>();
            }
            return new OrgProxyNew();
        }

        private static IOrgProxy LoadCustomOrgProxy(DataContext ctx = null)
        {
            string str = ConfigurationManager.AppSettings[MetaConfig.OrgProxyInterfacePath];
            if (!string.IsNullOrWhiteSpace(str))
            {
                if (ctx == null)
                {
                    return CustomHandlerLoader.GetHandlerWithConfiguration<IOrgProxy>(str, new object[0]);
                }
                return CustomHandlerLoader.GetHandlerWithConfiguration<IOrgProxy>(str, new object[] { ctx });
            }
            return null;
        }
    }
}

