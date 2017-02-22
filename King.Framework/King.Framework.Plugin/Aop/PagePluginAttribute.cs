namespace King.Framework.Plugin.Aop
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public class PagePluginAttribute : Attribute
    {
        private readonly string _pageName;
        private readonly bool _reusable;

        public PagePluginAttribute(string pageName)
        {
            this._reusable = false;
            this._pageName = pageName;
            this._reusable = false;
        }

        public PagePluginAttribute(string pageName, bool reusable)
        {
            this._reusable = false;
            this._pageName = pageName;
            this._reusable = false;
        }

        public string PageName
        {
            get
            {
                return this._pageName;
            }
        }

        public bool Reusable
        {
            get
            {
                return this._reusable;
            }
        }
    }
}

