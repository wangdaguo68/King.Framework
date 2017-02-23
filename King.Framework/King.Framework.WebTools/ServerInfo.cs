namespace King.Framework.WebTools
{
    using System;
    using System.Text.RegularExpressions;
    using System.Web;

    public static class ServerInfo
    {
        public static string GetRootPath()
        {
            string input = "";
            HttpContext current = HttpContext.Current;
            if (current != null)
            {
                return current.Server.MapPath("~");
            }
            input = AppDomain.CurrentDomain.BaseDirectory;
            if (Regex.Match(input, @"\\$", RegexOptions.Compiled).Success)
            {
                input = input.Substring(0, input.Length - 1);
            }
            return input;
        }

        public static string GetRootURI()
        {
            HttpContext current = HttpContext.Current;
            if (current == null)
            {
                throw new ApplicationException("HttpContext.Current==null");
            }
            return GetRootURI(current.Request);
        }

        public static string GetRootURI(HttpRequest req)
        {
            if (req == null)
            {
                throw new ArgumentNullException("req");
            }
            if (req.Url == null)
            {
                throw new ArgumentNullException("req.Url==null");
            }
            string leftPart = req.Url.GetLeftPart(UriPartial.Authority);
            if ((req.ApplicationPath == null) || (req.ApplicationPath == "/"))
            {
                return leftPart;
            }
            return (leftPart + req.ApplicationPath);
        }
    }
}

