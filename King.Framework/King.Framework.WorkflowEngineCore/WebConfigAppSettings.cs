namespace King.Framework.WorkflowEngineCore
{
    using System;
    using System.Configuration;
    using System.Web;

    internal class WebConfigAppSettings
    {
        public static string GetServerHost()
        {
            if (!string.IsNullOrEmpty(BizServerHost))
            {
                return BizServerHost;
            }
            if (HttpContext.Current == null)
            {
                return "/";
            }
            string leftPart = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string appDomainAppVirtualPath = HttpRuntime.AppDomainAppVirtualPath;
            if (!appDomainAppVirtualPath.EndsWith("/"))
            {
                appDomainAppVirtualPath = appDomainAppVirtualPath + "/";
            }
            return (leftPart + appDomainAppVirtualPath);
        }

        public static string ApplicationName
        {
            get
            {
                string str = ConfigurationManager.AppSettings[WebConfigConstKey.ApplicationName];
                if (string.IsNullOrWhiteSpace(str))
                {
                    throw new ApplicationException("未配置当前应用名称");
                }
                return str;
            }
        }

        public static string BizServerHost
        {
            get
            {
                string str = ConfigurationManager.AppSettings[WebConfigConstKey.BizServerHost];
                if (!string.IsNullOrWhiteSpace(str))
                {
                    Uri uri = new Uri(str);
                    return uri.GetLeftPart(UriPartial.Authority);
                }
                return null;
            }
        }

        public static string HomeBizDBConnString
        {
            get
            {
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[WebConfigConstKey.HomeBizDB];
                if (settings == null)
                {
                    throw new ApplicationException("未配置门户网站的业务数据库连接字符串");
                }
                return settings.ConnectionString;
            }
        }

        public static string InnerHomePageURL
        {
            get
            {
                string str = ConfigurationManager.AppSettings[WebConfigConstKey.InnerHomePageURL];
                if (string.IsNullOrWhiteSpace(str))
                {
                    throw new ApplicationException("未配置内网访问时企业门户的首页地址");
                }
                return str;
            }
        }

        public static string InnerRootURL
        {
            get
            {
                string str = ConfigurationManager.AppSettings[WebConfigConstKey.InnerRootURL];
                if (string.IsNullOrWhiteSpace(str))
                {
                    throw new ApplicationException("未配置内网访问时应用程序的根目录地址");
                }
                Uri uri = new Uri(str);
                return uri.GetLeftPart(UriPartial.Authority);
            }
        }

        public static string InnerSSORootURL
        {
            get
            {
                string str = ConfigurationManager.AppSettings[WebConfigConstKey.InnerSSORootURL];
                if (string.IsNullOrWhiteSpace(str))
                {
                    throw new ApplicationException("未配置内网访问时认证中心的根目录地址");
                }
                return str;
            }
        }

        public static string InnerSSOURL
        {
            get
            {
                string str = ConfigurationManager.AppSettings[WebConfigConstKey.InnerSSOURL];
                if (string.IsNullOrWhiteSpace(str))
                {
                    throw new ApplicationException("未配置认证中心的内部登录页地址");
                }
                return str;
            }
        }

        public static string OuterHomePageURL
        {
            get
            {
                string str = ConfigurationManager.AppSettings[WebConfigConstKey.OuterHomePageURL];
                if (string.IsNullOrWhiteSpace(str))
                {
                    throw new ApplicationException("未配置外网访问时企业门户的首页地址");
                }
                return str;
            }
        }

        public static string OuterRootURL
        {
            get
            {
                string str = ConfigurationManager.AppSettings[WebConfigConstKey.OuterRootURL];
                if (string.IsNullOrWhiteSpace(str))
                {
                    throw new ApplicationException("未配置外网访问时应用程序的根目录地址");
                }
                Uri uri = new Uri(str);
                return uri.GetLeftPart(UriPartial.Authority);
            }
        }

        public static string OuterSSORootURL
        {
            get
            {
                string str = ConfigurationManager.AppSettings[WebConfigConstKey.OuterSSORootURL];
                if (string.IsNullOrWhiteSpace(str))
                {
                    throw new ApplicationException("未配置外网访问时认证中心的根目录地址");
                }
                return str;
            }
        }

        public static string OuterSSOURL
        {
            get
            {
                string str = ConfigurationManager.AppSettings[WebConfigConstKey.OuterSSOURL];
                if (string.IsNullOrWhiteSpace(str))
                {
                    throw new ApplicationException("未配置认证中心的外部登录页地址");
                }
                return str;
            }
        }

        public static string SSOServiceURL
        {
            get
            {
                string str = ConfigurationManager.AppSettings[WebConfigConstKey.SSOServiceURL];
                if (string.IsNullOrWhiteSpace(str))
                {
                    throw new ApplicationException("未配置业务网站访问认证中心的Service地址");
                }
                return str;
            }
        }

        public static bool UseSSOModel
        {
            get
            {
                bool flag;
                string str = ConfigurationManager.AppSettings[WebConfigConstKey.UseSSOModel];
                return ((!string.IsNullOrWhiteSpace(str) && bool.TryParse(str.ToLower(), out flag)) && flag);
            }
        }
    }
}

