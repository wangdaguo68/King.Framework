namespace King.Framework.WorkflowEngineCore.Remind
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using System;

    public class CustomRemindManager : ICustomRemindHandler
    {
        private BizDataContext context;

        public CustomRemindManager(BizDataContext ctx)
        {
            this.context = ctx;
        }

        public void Execute(IUser receiveUser, string title, string content, SysProcessInstance pi, SysActivityInstance ai, SysWorkItem wi, RemindTemplateModel model)
        {
            try
            {
                SysInternalMail mail = new SysInternalMail {
                    MailId = this.context.GetNextIdentity_Int(false),
                    Subject = title,
                    Body = content,
                    CreateTime = new DateTime?(DateTime.Now)
                };
                SysInternalMailSender sender = new SysInternalMailSender {
                    SenderId = this.context.GetNextIdentity_Int(false),
                    MailId = new int?(mail.MailId),
                    State = 1,
                    SendTime = new DateTime?(DateTime.Now),
                    UserId = 1
                };
                SysInternalMailReceiver receiver = new SysInternalMailReceiver {
                    ReceiverId = this.context.GetNextIdentity_Int(false),
                    ReceiveTime = new DateTime?(DateTime.Now),
                    State = 1,
                    SenderId = new int?(sender.SenderId),
                    IsRead = false,
                    UserId = new int?(receiveUser.User_ID)
                };
                this.context.Insert(mail);
                this.context.Insert(sender);
                this.context.Insert(receiver);
            }
            catch (Exception exception)
            {
                throw new ApplicationException("处理自定义提醒错误：" + exception.Message);
            }
        }
    }
}

