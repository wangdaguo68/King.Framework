namespace King.Framework.Plugin.Web
{
    using System;
    using System.Runtime.CompilerServices;

    public class EventAttribute : Attribute
    {
        public string Description { get; set; }

        public King.Framework.Plugin.Web.InvokeType InvokeType { get; set; }

        public string Key { get; set; }

        public string PageName { get; set; }
    }
}

