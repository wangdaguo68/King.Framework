namespace King.Framework.Plugin.Web
{
    using King.Framework.Plugin.Aop;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Web.UI;

    public class CommandHandler
    {
        public readonly List<CommandPluginHandler> afterActions;
        public readonly List<CommandPluginHandler> beforeActions;
        public readonly PagePluginHandler pageHandler;

        public CommandHandler(PagePluginHandler _pageHandler)
        {
            this.pageHandler = _pageHandler;
            this.beforeActions = new List<CommandPluginHandler>();
            this.afterActions = new List<CommandPluginHandler>();
        }

        public void Add(CommandPluginAttribute plugin_attr, MethodInfo method_info)
        {
            CommandPluginHandler item = new CommandPluginHandler(this, plugin_attr, method_info);
            if (plugin_attr.InvokeType == InvokeType.Before)
            {
                this.beforeActions.Add(item);
            }
            else
            {
                this.afterActions.Add(item);
            }
        }

        public void ExecuteAfter(IPagePlugin pluginInstance, Control sender, PluginEventArgs e)
        {
            this.InvokePluginList(pluginInstance, this.afterActions, sender, e);
        }

        public void ExecuteBefore(IPagePlugin pluginInstance, Control sender, PluginEventArgs e)
        {
            this.InvokePluginList(pluginInstance, this.beforeActions, sender, e);
        }

        public List<CommandPluginHandler> GetActions(InvokeType invoke_type)
        {
            if (invoke_type == InvokeType.Before)
            {
                return this.beforeActions;
            }
            return this.afterActions;
        }

        private void InvokePluginList(IPagePlugin pluginInstance, List<CommandPluginHandler> plugin_handlers, Control sender, PluginEventArgs e)
        {
            if (plugin_handlers.Count > 0)
            {
                foreach (CommandPluginHandler handler in plugin_handlers)
                {
                    handler.GetEventHandler(pluginInstance)(sender, e);
                }
            }
        }
    }
}

