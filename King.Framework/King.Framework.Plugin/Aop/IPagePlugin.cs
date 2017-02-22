namespace King.Framework.Plugin.Aop
{
    using System;
    using System.Web.UI;

    public interface IPagePlugin
    {
        void AfterOnInit();
        void AfterOnInitComplete();
        void AfterOnLoad();
        void AfterOnLoadComplete();
        void AfterOnPreInit();
        void AfterOnPreLoad();
        void AfterOnPreRender();
        void AfterOnUnload();
        void BeforeOnInit();
        void BeforeOnInitComplete();
        void BeforeOnLoad();
        void BeforeOnLoadComplete();
        void BeforeOnPreInit();
        void BeforeOnPreLoad();
        void BeforeOnPreRender();
        void BeforeOnUnload();
        void SetPage(Page page);
    }
}

