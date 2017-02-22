namespace King.Framework.Plugin.Web
{
    using System;
    using System.Web.UI;

    public interface IEventCallHandler
    {
        void Invoke(Page page, Control sender, params object[] _parmas);
    }
}

