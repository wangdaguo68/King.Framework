namespace King.Framework.Plugin.Web
{
    using King.Framework.Interfaces;
    using King.Framework.Plugin.Aop;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class PagePluginHandler
    {
        private readonly Dictionary<string, CommandHandler> _commandHandlers;
        private readonly IPagePlugin _pluginInstance;
        public readonly PagePluginAttribute pluginAttribute;
        public readonly Type pluginType;

        public PagePluginHandler(PagePluginAttribute plugin_attribute, Type plugin_type)
        {
            if (plugin_attribute == null)
            {
                throw new ArgumentNullException("plugin_attribute");
            }
            if (plugin_type == null)
            {
                throw new ArgumentNullException("plugin_type");
            }
            this.pluginAttribute = plugin_attribute;
            this.pluginType = plugin_type;
            if (plugin_attribute.Reusable)
            {
                this._pluginInstance = this.NewPagePluginInstance();
            }
            this._commandHandlers = new Dictionary<string, CommandHandler>();
            this.InitCommandPlugins();
        }

        public IPagePlugin GetPluginInstance()
        {
            if (this.pluginAttribute.Reusable)
            {
                if (this._pluginInstance == null)
                {
                    throw new ApplicationExceptionFormat("页面{0}插件{1}为可重用，但没有创建默认实例,请联系框架开发人员", new object[] { this.pluginAttribute.PageName, this.pluginType });
                }
                return this._pluginInstance;
            }
            return this.NewPagePluginInstance();
        }

        private void InitCommandPlugins()
        {
            MethodInfo[] methods = this.pluginType.GetMethods();
            foreach (MethodInfo info in methods)
            {
                CommandPluginAttribute attribute = info.GetCustomAttributes(typeof(CommandPluginAttribute), false).FirstOrDefault<object>() as CommandPluginAttribute;
                if (attribute != null)
                {
                    this.SureCommandHandler(attribute.ButtonName).Add(attribute, info);
                }
            }
        }

        private IPagePlugin NewPagePluginInstance()
        {
            IPagePlugin plugin;
            if (this.pluginType == null)
            {
                throw new ApplicationException("pluginType还未赋值呢，请联系框架开发人员");
            }
            try
            {
                plugin = Activator.CreateInstance(this.pluginType) as IPagePlugin;
            }
            catch (Exception exception)
            {
                throw new ApplicationException(string.Format("创建页面插件{0}的实例发生错误", this.pluginType), exception);
            }
            if (plugin == null)
            {
                throw new ApplicationException(string.Format("类{0}已标记为页面插件，但未实现{1}接口", this.pluginType, typeof(IPagePlugin)));
            }
            return plugin;
        }

        private CommandHandler SureCommandHandler(string key)
        {
            if (this._commandHandlers.ContainsKey(key))
            {
                return this._commandHandlers[key];
            }
            CommandHandler handler = new CommandHandler(this);
            this._commandHandlers.Add(key, handler);
            return handler;
        }

        public Dictionary<string, CommandHandler> CommandHandlers
        {
            get
            {
                return this._commandHandlers;
            }
        }
    }
}

