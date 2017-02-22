namespace King.Framework.AspNet.Identity
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using Microsoft.AspNet.Identity;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Transactions;
    using System.Web;

    public static class LoginUserHelper
    {
        private static readonly Random rnd = new Random(0x400);

        public static string AppendTokenToUrl(string url, IUserIdentity user)
        {
            if (string.IsNullOrEmpty(url))
            {
                return url;
            }
            string str = CreateUserToken(user);
            string tokenKey = TokenKey;
            StringBuilder builder = new StringBuilder(0x100);
            builder.Append(url);
            builder.Append(url.Contains('?') ? "&" : "?");
            builder.Append(HttpUtility.UrlEncode(tokenKey)).Append("=").Append(HttpUtility.UrlEncode(str));
            return builder.ToString();
        }

        public static string AppendTokenToUrl(string url, string token)
        {
            if (string.IsNullOrEmpty(url))
            {
                return url;
            }
            var tokenKey = TokenKey;
            var builder = new StringBuilder(0x100);
            builder.Append(url);
            builder.Append(url.Contains('?') ? "&" : "?");
            builder.Append(HttpUtility.UrlEncode(tokenKey)).Append("=").Append(HttpUtility.UrlEncode(token));
            return builder.ToString();
        }

        public static string CreateUserToken(IUserIdentity user)
        {
            if (!IsSsoServer) return null;
            var span = new TimeSpan(2, 0, 0);
            var str = DateTime.Now.Ticks.ToString("X16");
            var str2 = Guid.NewGuid().ToString("N").ToUpper();
            var str3 = rnd.Next(0x989680, 0x7ffffffe).ToString("X");
            var str4 = str + str2 + str3;
            using (var scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                using (var context = new BizDataContext())
                {
                    var token = new SysUserToken {
                        TokenKey = str4,
                        UserId = user.User_ID,
                        ExpireTime = DateTime.Now.Add(span)
                    };
                    context.Insert(token);
                }
                scope.Complete();
            }
            user.SsoToken = str4;
            return str4;
        }

        public static bool ExtSsoTime(string accentUrl, string token, out string errMsg)
        {
            errMsg = string.Empty;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add(TokenKey, token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var httpClient =new HttpClient();
                var result = client.PostAsJsonAsync(accentUrl + token, new object()).Result;
                if (result.IsSuccessStatusCode)
                {
                    var str = result.Content.ReadAsStringAsync().Result;
                }
            }
            return true;
        }

        public static SysUser GetUserFromToken(string token)
        {
            SysUser user;
            if (IsSsoServer)
            {
                AppLogHelper.Information("IsSsoServer=true");
                var userIdFromToken = GetUserIdFromToken(token);
                if (userIdFromToken <= 0) return null;
                var manager = new UserManager<SysUser>(new UserStore());
                user = manager.FindById(userIdFromToken.ToString());
                user.SsoToken = token;
                user.LastTokenBeatTime = DateTime.Now;
                return user;
            }
            if (!IsSsoClient) return null;
            string str2;
            AppLogHelper.Information("IsSsoClient=true");
            var address = AppendTokenToUrl(TokenValidateUrl, token);
            var client = new WebClient();
            try
            {
                var tokenKey = TokenKey;
                var data = new NameValueCollection {{tokenKey, token}};
                var bytes = client.UploadValues(address, data);
                if (bytes.Length > 0)
                {
                    str2 = Encoding.UTF8.GetString(bytes);
                    AppLogHelper.Information("token验证返回信息:" + str2);
                }
                else
                {
                    AppLogHelper.Information("token验证时返回信息长度为0");
                    str2 = string.Empty;
                }
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(exception);
                str2 = string.Empty;
            }
            finally
            {
                if (client != null)
                {
                    client.Dispose();
                }
            }
            if (!string.IsNullOrWhiteSpace(str2))
            {
                user = JsonConvert.DeserializeObject<SysUser>(str2);
                user.SsoToken = token;
                user.LastTokenBeatTime = DateTime.Now;
                return user;
            }
            return null;
        }

        public static int GetUserIdFromToken(string token, BizDataContext db = null)
        {
            SysUserToken token2;
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new SsoException("令牌为空", SsoException.NullToken);
            }
            if (db == null)
            {
                using (db = new BizDataContext())
                {
                    token2 = db.FindById<SysUserToken>(token);
                    token2.ExpireTime = DateTime.Now.AddHours(2.0);
                    db.UpdatePartial(token2, p => new {p.ExpireTime });
                }
            }
            else
            {
                token2 = db.FindById<SysUserToken>(token);
                token2.ExpireTime = DateTime.Now.AddHours(2.0);
                db.UpdatePartial(token2, p => new {p.ExpireTime });
            }
            if (token2 == null)
            {
                throw new SsoException("令牌无效", SsoException.InvalidToken);
            }
            if (token2.ExpireTime < DateTime.Now)
            {
                throw new SsoException("令牌已过期", SsoException.ExpiredToken);
            }
            return token2.UserId;
        }

        public static bool IsSsoClient
        {
            get
            {
                if (!UseSSO) return false;
                var str = ConfigurationManager.AppSettings["sso:role"];
                if (string.IsNullOrEmpty(str))
                {
                    throw new ApplicationException("未配置sso:role");
                }
                return !(string.IsNullOrEmpty(str) || str.ToLower() != "client");
            }
        }

        public static bool IsSsoServer
        {
            get
            {
                if (!UseSSO) return false;
                var str = ConfigurationManager.AppSettings["sso:role"];
                if (string.IsNullOrEmpty(str))
                {
                    throw new ApplicationException("未配置sso:role");
                }
                return !(string.IsNullOrEmpty(str) || str.ToLower() != "server");
            }
        }

        public static string TokenKey
        {
            get
            {
                var str = ConfigurationManager.AppSettings["sso:token_key"];
                return !string.IsNullOrWhiteSpace(str) ? str : "sso_token";
            }
        }

        public static string TokenValidateUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["sso:TokenValidateUrl"];
            }
        }

        public static bool UseSSO
        {
            get
            {
                var str = ConfigurationManager.AppSettings["sso:use_sso"];
                return !(string.IsNullOrEmpty(str) || str.ToLower() != "true");
            }
        }
    }
}

