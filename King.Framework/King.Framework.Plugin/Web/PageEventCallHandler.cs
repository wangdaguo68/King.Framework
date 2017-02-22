namespace King.Framework.Plugin.Web
{
    using System;
    using System.Collections.Generic;

    internal class PageEventCallHandler : Dictionary<string, List<EventCallHandlerItem>>
    {
        public void Add(EventAttribute invokeInfoAttr, Type handlerType)
        {
            this.SureButtonEventCallHandler(invokeInfoAttr.Key).Add(new EventCallHandlerItem(invokeInfoAttr, handlerType));
        }

        private List<EventCallHandlerItem> SureButtonEventCallHandler(string buttonId)
        {
            if (base.ContainsKey(buttonId))
            {
                return base[buttonId];
            }
            List<EventCallHandlerItem> list = new List<EventCallHandlerItem>();
            base.Add(buttonId, list);
            return list;
        }
    }
}

