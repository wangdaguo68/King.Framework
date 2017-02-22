namespace King.Framework.AspNet.Identity
{
    using King.Framework.Common;
    using King.Framework.Interfaces;
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Web;

    public class TenantConnectionProvider : IConnectionProvider
    {
        private static readonly Dictionary<int, ConnectionInfo> connDict = new Dictionary<int, ConnectionInfo>();

        public ConnectionInfo GetConnectionInfo()
        {
            return GetTenantConnectionInfo();
        }

        private static IUserIdentity GetCurrentUser()
        {
            if (HttpContext.Current == null)
            {
                throw new ConfigurationErrorsException("httpContext == null, 请确认system.web/httpRuntime  targetFramework=\"4.5\", 并且appSettings/aspnet:UseTaskFriendlySynchronizationContext=true");
            }
            IPrincipal principal = HttpContext.Current.User;
            if (principal == null)
            {
                throw new ApplicationException("HttpContext.Current.User==null");
            }
            IIdentity identity = principal.Identity;
            if (identity == null)
            {
                throw new ApplicationException("HttpContext.Current.User.Identity==null");
            }
            ClaimsIdentity identity2 = identity as ClaimsIdentity;
            if (identity2 == null)
            {
                throw new ApplicationException("User.Identity 不是 ClaimsIdentity");
            }
            SysUser userData = identity2.GetUserData();
            if (userData == null)
            {
                throw new ApplicationException("未发现用户信息，多租户模式下必须登录才能发现用户的数据库信息。");
            }
            return userData;
        }

        public static ConnectionInfo GetTenantConnectionInfo()
        {
            ConnectionInfo info;
            Dictionary<int, ConnectionInfo> dictionary;
            IUserIdentity currentUser = GetCurrentUser();
            if (!currentUser.CompanyID.HasValue)
            {
                throw new ApplicationException("未设置用户的CompanyID");
            }
            int key = currentUser.CompanyID.Value;
            lock ((dictionary = connDict))
            {
                if (connDict.TryGetValue(key, out info))
                {
                    return info;
                }
            }
            ConnectionInfo connectionInfoFromWebFoundation = WebFoundationHelper.GetConnectionInfoFromWebFoundation(key);
            lock ((dictionary = connDict))
            {
                if (connDict.TryGetValue(currentUser.CompanyID.Value, out info))
                {
                    return info;
                }
                connDict.Add(key, connectionInfoFromWebFoundation);
                return connectionInfoFromWebFoundation;
            }
        }
    }
}

