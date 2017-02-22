namespace King.Framework.UrlAuthorization.Owin
{
    using King.Framework.AspNet.Identity;
    using King.Framework.EntityLibrary;
    using Microsoft.Owin;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;

    public class UrlAuthorizationMiddleware : OwinMiddleware
    {
        public UrlAuthorizationMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public async override Task Invoke(IOwinContext context)
        {
            await Task.Run(delegate {
                this.UrlAuthorizate(context);
            });
        }

        private void UrlAuthorizate(IOwinContext context)
        {
            try
            {
                string str = context.Request.Path.Value;
                if (string.IsNullOrWhiteSpace(str))
                {
                    context.Response.StatusCode=0x194;
                    context.Response.WriteAsync("The page is NotFound.");
                }
                else
                {
                    IPrincipal principal = context.Request.User;
                    if (!(((principal != null) && (principal.Identity != null)) && principal.Identity.IsAuthenticated))
                    {
                        context.Authentication.SignOut(new string[] { "ExternalCookie" });
                        context.Response.Redirect("~/Home/Login");
                    }
                    else
                    {
                        ClaimsIdentity identity = principal.Identity as ClaimsIdentity;
                        if (identity == null)
                        {
                            context.Authentication.SignOut(new string[] { "ExternalCookie" });
                            context.Response.Redirect("~/Home/Login");
                        }
                        else
                        {
                            SysUser userData = identity.GetUserData();
                            if (userData == null)
                            {
                                context.Authentication.SignOut(new string[] { "ExternalCookie" });
                                context.Response.Redirect("~/Home/Login");
                            }
                            else
                            {
                                bool flag = false;
                                SysPathRule[] rules = RuleLoader.GetRules();
                                if ((rules == null) || (rules.Length == 0))
                                {
                                    Next.Invoke(context);
                                }
                                else
                                {
                                    for (int i = 0; i < rules.Length; i++)
                                    {
                                        SysPathRule rule = rules[i];
                                        if (str.StartsWith(rule.Path))
                                        {
                                            if (rule.Users.Contains(userData.User_ID))
                                            {
                                                flag = true;
                                                break;
                                            }
                                            if (rule.Roles.Count > 0)
                                            {
                                                foreach (int num2 in rule.Roles)
                                                {
                                                    if (principal.IsInRole(num2.ToString()))
                                                    {
                                                        flag = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (flag)
                                    {
                                        Next.Invoke(context);
                                    }
                                    else
                                    {
                                        context.Response.StatusCode=0x193;
                                        context.Response.WriteAsync("禁止访问.");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                context.Response.StatusCode=500;
                context.Response.WriteAsync("服务器错误:" + exception);
            }
        }


        private class AuthorizateException : Exception
        {
            public AuthorizateException(int code, string msg) : base(msg)
            {
            }
        }
    }
}

