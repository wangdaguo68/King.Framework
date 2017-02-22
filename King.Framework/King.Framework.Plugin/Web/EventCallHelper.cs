namespace King.Framework.Plugin.Web
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI;

    internal class EventCallHelper
    {
        private static readonly EventCallHandlerContext handlerContext = new EventCallHandlerContext();

        public static void FindPluginInType(Type type)
        {
            handlerContext.FindPluginInType(type);
        }

        private static void Invoke(Page page, Control sender, InvokeType invokeType, params object[] _parmas)
        {
            if (page == null)
            {
                throw new ApplicationException("未指定页面");
            }
            string pagePluginKey = PagePluginFactory.GetPagePluginKey(page);
            List<EventCallHandlerItem> eventCallHandlerItemList = handlerContext.GetEventCallHandlerItemList(pagePluginKey, sender.ID);
            if ((eventCallHandlerItemList != null) && (eventCallHandlerItemList.Count > 0))
            {
                foreach (EventCallHandlerItem item in eventCallHandlerItemList)
                {
                    if (item.InvokeInfo.InvokeType == invokeType)
                    {
                        item.EventCallHandler.Invoke(page, sender, _parmas);
                    }
                }
            }
        }

        internal static void InvokeAfter(Page page, Control sender, params object[] _parmas)
        {
            Invoke(page, sender, InvokeType.After, _parmas);
        }

        internal static void InvokeBefore(Page page, Control sender, params object[] _parmas)
        {
            Invoke(page, sender, InvokeType.Before, _parmas);
        }
    }
}

