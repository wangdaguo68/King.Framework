namespace King.Framework.Plugin.Aop
{
    using System;

    public interface IBasePage
    {
        void AjaxAlert(string msg);

        IPagePlugin PluginInstance { get; }
    }
}

