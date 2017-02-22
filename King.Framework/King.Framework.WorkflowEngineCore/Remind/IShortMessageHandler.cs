namespace King.Framework.WorkflowEngineCore.Remind
{
    using King.Framework.Interfaces;
    using System;

    public interface IShortMessageHandler
    {
        void SendMessage(IUser receiveUser, string content);
    }
}

