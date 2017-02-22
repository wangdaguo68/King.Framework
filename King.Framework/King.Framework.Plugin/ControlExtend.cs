namespace King.Framework.Plugin
{
    using King.Framework.Interfaces;
    using System;
    using System.Runtime.CompilerServices;
    using System.Web.UI;

    public static class ControlExtend
    {
        public static Page GetAbsolutePage(this Control ctrl)
        {
            Page page = ctrl.Page;
            while (page == null)
            {
                ctrl = ctrl.NamingContainer;
                if (ctrl.NamingContainer == null)
                {
                    break;
                }
                page = ctrl.Page;
            }
            if (page == null)
            {
                throw new ApplicationExceptionFormat("无法从此控件{0}找到页面", new object[] { ctrl.ID });
            }
            return page;
        }
    }
}

