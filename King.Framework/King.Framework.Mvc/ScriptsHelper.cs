namespace King.Framework.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Optimization;

    public static class ScriptsHelper
    {
        private static readonly string ScriptFilesKey = "__scriptFiles";

        public static IHtmlString RenderScript(this HtmlHelper html, string nameOrAddress)
        {
            HashSet<string> set = html.ViewContext.TempData[ScriptFilesKey] as HashSet<string>;
            if (set == null)
            {
                set = new HashSet<string>();
                html.ViewContext.TempData[ScriptFilesKey] = set;
            }
            set.Add(nameOrAddress);
            return Scripts.Render(new string[] { nameOrAddress });
        }
    }
}

