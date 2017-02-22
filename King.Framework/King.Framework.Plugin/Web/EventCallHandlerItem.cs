namespace King.Framework.Plugin.Web
{
    using System;

    internal class EventCallHandlerItem
    {
        public readonly IEventCallHandler EventCallHandler;
        public readonly Type HandlerType;
        public readonly EventAttribute InvokeInfo;

        public EventCallHandlerItem(EventAttribute invoke_attr, Type handler_type)
        {
            this.InvokeInfo = invoke_attr;
            this.HandlerType = handler_type;
            this.EventCallHandler = Activator.CreateInstance(handler_type) as IEventCallHandler;
        }
    }
}

