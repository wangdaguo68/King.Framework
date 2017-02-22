namespace King.Framework.Manager
{
    using System;
    using System.Collections.Generic;

    public interface IMessageSender
    {
        void Send(Message msg);
        void Send(IList<Message> msgList);
        bool TrySend(string to);
    }
}
