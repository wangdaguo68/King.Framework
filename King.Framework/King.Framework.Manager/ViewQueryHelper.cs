namespace King.Framework.Manager
{
    using King.Framework.Common;
    using King.Framework.Interfaces;
    using King.Framework.Manager.Ioc;
    using King.Framework.Plugin.Web;
    using System;

    public class ViewQueryHelper
    {
        private BizDataContext _context;
        private IUserIdentity _user;

        public ViewQueryHelper(BizDataContext context, IUserIdentity currentUser)
        {
            this._context = context;
            this._user = currentUser;
        }

        public ViewQueryBase GetAccordionQuery(string className)
        {
            return this.GetViewQuery(className);
        }

        public ViewQueryBase GetExportViewControlQuery(string className)
        {
            return this.GetViewQuery(className);
        }

        public ViewQueryBase GetParentViewControlQuery(string className)
        {
            return this.GetViewQuery(className);
        }

        public ViewQueryBase GetViewControlQuery(string className)
        {
            return this.GetViewQuery(className);
        }

        public ViewQueryBase GetViewQuery(string className)
        {
            Type queryPluginType = PagePluginFactory.GetQueryPluginType(className);
            if (queryPluginType != null)
            {
                return (Activator.CreateInstance(queryPluginType, new object[] { this._context, this._user }) as ViewQueryBase);
            }
            return (Activator.CreateInstance(UnityObjectFactory.GetViewQueryType(className), new object[] { this._context, this._user }) as ViewQueryBase);
        }
    }
}
