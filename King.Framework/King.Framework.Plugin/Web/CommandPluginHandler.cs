namespace King.Framework.Plugin.Web
{
    using King.Framework.Plugin.Aop;
    using System;
    using System.Reflection;

    public class CommandPluginHandler
    {
        public readonly CommandHandler commandHandler;
        public readonly MethodInfo methodInfo;
        public readonly CommandPluginAttribute pluginAttribute;

        public CommandPluginHandler(CommandHandler _commandHandler, CommandPluginAttribute _pluginAttribute, MethodInfo _methodInfo)
        {
            if (_commandHandler == null)
            {
                throw new ArgumentNullException("_commandHandler");
            }
            if (_pluginAttribute == null)
            {
                throw new ArgumentNullException("_pluginAttribute");
            }
            if (_methodInfo == null)
            {
                throw new ArgumentNullException("_methodInfo");
            }
            if (_commandHandler.pageHandler.pluginType != _methodInfo.ReflectedType)
            {
                throw new ArgumentNullException("_commandHandler.pageHandler.pluginType != _methodInfo.ReflectedType");
            }
            this.commandHandler = _commandHandler;
            this.pluginAttribute = _pluginAttribute;
            this.methodInfo = _methodInfo;
        }

        private PluginEventHandler CreateEventHandler(object target, MethodInfo method_info)
        {
            PluginEventHandler handler;
            try
            {
                handler = Delegate.CreateDelegate(typeof(PluginEventHandler), target, method_info) as PluginEventHandler;
            }
            catch (Exception exception)
            {
                throw new NotMatchEventHandlerException(method_info, exception);
            }
            if (handler == null)
            {
                throw new NotMatchEventHandlerException(method_info);
            }
            return handler;
        }

        public PluginEventHandler GetEventHandler(IPagePlugin pluginInstance)
        {
            PagePluginHandler pageHandler = this.commandHandler.pageHandler;
            if (pageHandler.pluginAttribute.Reusable)
            {
                IPagePlugin target = pageHandler.GetPluginInstance();
                return this.CreateEventHandler(target, this.methodInfo);
            }
            return this.CreateEventHandler(pluginInstance, this.methodInfo);
        }
    }
}

