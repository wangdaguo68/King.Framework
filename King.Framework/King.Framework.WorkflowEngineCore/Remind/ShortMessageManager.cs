namespace King.Framework.WorkflowEngineCore.Remind
{
    using King.Framework.Common;
    using King.Framework.Interfaces;
    using System;

    public class ShortMessageManager : IShortMessageHandler
    {
        public ShortMessageManager(BizDataContext ctx)
        {
        }

        public void SendMessage(IUser receiveUser, string content)
        {
            throw new ApplicationException("未实现短信发送接口");
        }
    }
}

