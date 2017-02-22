namespace King.Framework.Plugin.Web
{
    using System;
    using System.Runtime.CompilerServices;

    public class PluginEventArgs : EventArgs
    {
        public int CurrentUserID { get; set; }

        public object entityModel { get; set; }

        public string RedirectURL { get; set; }
    }
}

