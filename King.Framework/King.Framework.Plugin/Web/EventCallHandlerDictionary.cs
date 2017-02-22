namespace King.Framework.Plugin.Web
{
    using System;
    using System.Collections.Generic;

    internal class EventCallHandlerDictionary : Dictionary<string, PageEventCallHandler>
    {
        public void Add(EventAttribute invokeInfoAttr, Type type)
        {
            if (string.IsNullOrWhiteSpace(invokeInfoAttr.PageName))
            {
                throw new ApplicationException("插件的PageName为空");
            }
            if (string.IsNullOrWhiteSpace(invokeInfoAttr.Key))
            {
                throw new ApplicationException("插件的Key为空");
            }
            string lowerPageName = invokeInfoAttr.PageName.ToLower();
            this.SurePageDict(lowerPageName).Add(invokeInfoAttr, type);
        }

        private PageEventCallHandler SurePageDict(string lowerPageName)
        {
            if (base.ContainsKey(lowerPageName))
            {
                return base[lowerPageName];
            }
            PageEventCallHandler handler = new PageEventCallHandler();
            base.Add(lowerPageName, handler);
            return handler;
        }
    }
}

