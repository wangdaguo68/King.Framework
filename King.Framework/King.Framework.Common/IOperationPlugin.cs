namespace King.Framework.Common
{
    using System;

    public interface IOperationPlugin
    {
        void InnerAfterExecute(object obj);
        void InnerBeforeExecute(object obj);
    }
}
