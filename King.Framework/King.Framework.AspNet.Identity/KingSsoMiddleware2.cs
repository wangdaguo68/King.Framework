namespace King.Framework.AspNet.Identity
{
    using King.Framework.Common;
    using King.Framework.Interfaces;
    using Microsoft.AspNet.Identity;
    using Microsoft.Owin;
    using Microsoft.Owin.Security;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    internal class KingSsoMiddleware2 : OwinMiddleware
    {
        private static readonly long max_ticks = 10L;

        public KingSsoMiddleware2(OwinMiddleware next) : base(next)
        {
        }

        private void BuildSsoAuthenticate(IOwinContext context)
        {
            string ssoToken = GetSsoToken(context);
            this.LogSsoToken(context, ssoToken);
            if (!string.IsNullOrWhiteSpace(ssoToken))
            {
                SysUser userFromToken;
                UserManager<SysUser> userManager = new UserManager<SysUser>(new UserStore());
                this.TimeExpand(context, userManager);
                try
                {
                    userFromToken = LoginUserHelper.GetUserFromToken(ssoToken);
                }
                catch (Exception exception)
                {
                    AppLogHelper.Information(exception.ToString());
                    userFromToken = null;
                }
                if (userFromToken != null)
                {
                    IAuthenticationManager manager2 = context.Authentication;
                    manager2.SignOut("ExternalCookie");
                    ClaimsIdentity identity = userManager.CreateIdentity(userFromToken, "ApplicationCookie");
                    identity.AddUserData(userFromToken);
                    AuthenticationProperties properties = new AuthenticationProperties {IsPersistent = true};
                    manager2.SignIn(properties, identity);
                    try
                    {
                        Trace.WriteLine(new StringBuilder().Append(" find user: url = ").Append(context.Request.Uri.AbsolutePath).Append("; user_id = ").Append(identity.GetUserId()).Append("; user_name = ").Append(identity.GetUserName()));
                    }
                    catch
                    {
                        // ignored
                    }
                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                    context.Request.Set("User",principal);
                }
            }
        }

        public bool CheckLogout(long lastWaitTime, long now)
        {
            return (lastWaitTime > now);
        }

        private static string GetSsoToken(IOwinContext context)
        {
            string tokenKey = LoginUserHelper.TokenKey;
            string str2 = context.Request.Headers.Get(tokenKey);
            if (string.IsNullOrWhiteSpace(str2))
            {
                str2 = context.Request.Query.Get(tokenKey);
            }
            return str2;
        }

        public string GetUrlWithoutToken(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return url;
            }
            int index = url.IndexOf('?');
            if (index < 0)
            {
                return url;
            }
            string[] strArray = url.Substring(index + 1).Split(new char[] { '?', '&' });
            string str = LoginUserHelper.TokenKey.ToLower();
            StringBuilder builder = new StringBuilder(0xff);
            builder.Append(url.Substring(0, index));
            int num2 = 0;
            foreach (string str2 in strArray)
            {
                if (!string.IsNullOrEmpty(str2))
                {
                    string str3;
                    string str4;
                    int length = str2.IndexOf('=');
                    if (length < 0)
                    {
                        str3 = str2;
                        str4 = null;
                    }
                    else
                    {
                        str3 = str2.Substring(0, length);
                        str4 = str2.Substring(length + 1);
                    }
                    if (str != str3.ToLower())
                    {
                        builder.Append((num2 > 0) ? "&" : "?");
                        num2++;
                        builder.Append(str3);
                        builder.Append("=");
                        builder.Append(str4);
                    }
                }
            }
            return builder.ToString();
        }

        public async override Task Invoke(IOwinContext context)
        {
            this.BuildSsoAuthenticate(context);
            await this.Next.Invoke(context);
        }

        private bool IsSsoLogin(IOwinContext context)
        {
            string str = context.Request.Path.Value.ToLower();
            return ((str == "/account/ssologin") || (str == "/account/tokenvalidate"));
        }

        private void LogSsoToken(IOwinContext context, string token)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("get token: url = ").Append(context.Request.Uri.AbsolutePath).Append("; token=").Append(token);
            Trace.WriteLine(builder.ToString());
        }

        private void Redirect(IOwinContext context, string url)
        {
            context.Response.Write("<html><head><title>Object moved</title></head><body>\r\n");
            context.Response.Write("<h2>Object moved to <a href=\"" + url + "\">here</a>.</h2>\r\n");
            context.Response.Write("</body></html>\r\n");
            context.Response.Redirect(url);
        }

        private void TimeExpand(IOwinContext context, UserManager<SysUser, string> userManager)
        {
            try
            {
                if (context.Authentication.User != null)
                {
                    ClaimsIdentity identity = context.Authentication.User.Identity as ClaimsIdentity;
                    if (identity != null)
                    {
                        SysUser userData = identity.GetUserData();
                        if ((userData != null) && !string.IsNullOrWhiteSpace(userData.SsoToken))
                        {
                            DateTime now = DateTime.Now;
                            DateTime? lastTokenBeatTime = userData.LastTokenBeatTime;
                            DateTime time2 = lastTokenBeatTime.HasValue ? lastTokenBeatTime.GetValueOrDefault() : DateTime.Now.AddHours(-1.0);
                            TimeSpan span = now - time2;
                            if (span.TotalMinutes > max_ticks)
                            {
                                string accentUrl = LoginUserHelper.AppendTokenToUrl(LoginUserHelper.TokenValidateUrl, userData);
                                string errMsg = string.Empty;
                                if (!LoginUserHelper.ExtSsoTime(accentUrl, userData.SsoToken, out errMsg))
                                {
                                    AppLogHelper.Information(errMsg);
                                }
                                IAuthenticationManager manager = context.Authentication;
                                manager.SignOut("ExternalCookie");
                                ClaimsIdentity identity2 = userManager.CreateIdentity(userData, "ApplicationCookie");
                                identity2.AddUserData(userData);
                                AuthenticationProperties properties = new AuthenticationProperties {IsPersistent = true};
                                manager.SignIn(properties, identity2);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                AppLogHelper.Information(exception.ToString());
            }
        }

    }
}

