namespace King.Framework.Common
{
    using System;

    public class OperationPluginBase<T> : IOperationPlugin where T: class
    {
        public virtual void AfterExecute(T target)
        {
        }

        public virtual void BeforeExecute(T target)
        {
        }

        void IOperationPlugin.InnerAfterExecute(object obj)
        {
            this.AfterExecute(obj as T);
        }

        void IOperationPlugin.InnerBeforeExecute(object obj)
        {
            this.BeforeExecute(obj as T);
        }
    }
}
