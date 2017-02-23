<<<<<<< HEAD
﻿namespace King.Framework.WebTools
=======
﻿namespace Drision.Framework.WebTools
>>>>>>> 2b573943baeab4164300c7a14b5c182ddccdbe59
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Web;

    public static class HttpCookieExtend
    {
        public static int ToInt(this HttpCookie cookie, int defaultValue = -1)
        {
            int? nullable = cookie.ToIntNull();
            return (nullable.HasValue ? nullable.GetValueOrDefault() : -1);
        }

        public static int? ToIntNull(this HttpCookie cookie)
        {
            if (cookie != null)
            {
                int num;
                string str = cookie.Value;
                if (string.IsNullOrWhiteSpace(str))
                {
                    return null;
                }
                if (int.TryParse(str, out num))
                {
                    return new int?(num);
                }
            }
            return null;
        }
    }
}

