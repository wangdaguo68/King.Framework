namespace King.Framework.Plugin.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class EventCallHandlerContext
    {
        private static readonly EventCallHandlerDictionary eventCallDict = new EventCallHandlerDictionary();

        public void FindPluginInType(Type type)
        {
            EventAttribute invokeInfoAttr = type.GetCustomAttributes(typeof(EventAttribute), false).FirstOrDefault<object>() as EventAttribute;
            if ((invokeInfoAttr != null) && (!string.IsNullOrWhiteSpace(invokeInfoAttr.PageName) && !string.IsNullOrWhiteSpace(invokeInfoAttr.Key)))
            {
                eventCallDict.Add(invokeInfoAttr, type);
            }
        }

        public List<EventCallHandlerItem> GetEventCallHandlerItemList(string lowerPageName, string key)
        {
            if (eventCallDict.ContainsKey(lowerPageName))
            {
                PageEventCallHandler handler = eventCallDict[lowerPageName];
                if (handler.ContainsKey(key))
                {
                    return handler[key];
                }
                return new List<EventCallHandlerItem>();
            }
            return new List<EventCallHandlerItem>();
        }
    }
}

