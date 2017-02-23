<<<<<<< HEAD
﻿namespace King.Framework.WebTools
=======
﻿namespace Drision.Framework.WebTools
>>>>>>> 2b573943baeab4164300c7a14b5c182ddccdbe59
{
    using System;
    using System.Web;

    public static class UserIpManager
    {
        public static string GetIp()
        {
            if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
            {
                return HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
        }

        public static bool IsInner()
        {
            return IsInner(GetIp());
        }

        public static bool IsInner(string strIp)
        {
            if (string.IsNullOrEmpty(strIp))
            {
                throw new ArgumentNullException("strIp");
            }
            string[] strArray = strIp.Split(new char[] { '.' });
            if (strArray.Length < 4)
            {
                throw new ApplicationException("不是有效IP地址");
            }
            long num = 0L;
            for (int i = 0; i < 2; i++)
            {
                num = (num * 0x3e8L) + long.Parse(strArray[i]);
            }
            bool flag = false;
            if ((((num >= 0x2710L) && (num <= 0x280fL)) || ((num >= 0x29ff0L) && (num <= 0x29fffL))) || ((num >= 0x2eea8L) && (num <= 0x2eea8L)))
            {
                flag = true;
            }
            return flag;
        }
    }
}

