namespace King.Framework.Plugin.Web
{
    using System;
    using System.Reflection;

    public class NotMatchEventHandlerException : ApplicationException
    {
        public NotMatchEventHandlerException(MethodInfo method_info) : base(string.Format("类{0}的方法{0}不能转换为EventHandler, 确保方法申明为 void method(object, PluginEventArgs) ", method_info.ReflectedType, method_info), null)
        {
        }

        public NotMatchEventHandlerException(MethodInfo method_info, Exception innerException) : base(string.Format("类{0}的方法{0}不能转换为EventHandler, 确保方法申明为 void method(object, PluginEventArgs) ", method_info.ReflectedType, method_info), innerException)
        {
        }
    }
}

