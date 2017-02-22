namespace King.Framework.WorkflowEngineCore.Remind
{
    using King.Framework.Interfaces;
    using System;

    public interface IEmailHandler
    {
        void SendEmail(IUser receiveUser, string subject, string content, string approvePageUrl, string processDetailUrl);
    }
}

