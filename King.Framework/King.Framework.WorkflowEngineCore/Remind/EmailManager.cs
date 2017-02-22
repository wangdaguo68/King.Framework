using System.Configuration;

namespace King.Framework.WorkflowEngineCore.Remind
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using System;

    public class EmailManager : IEmailHandler
    {
        private BizDataContext context;

        public EmailManager(BizDataContext ctx)
        {
            this.context = ctx;
        }

        public void SendEmail(string mailto, string cc, string subject, string content)
        {
            this.SendEmail(mailto, cc, subject, content, null);
        }

        public void SendEmail(IUser receiveUser, string subject, string content, string approvePageUrl, string processDetailUrl)
        {
            string mailto = receiveUser.User_EMail;
            this.SendEmail(mailto, null, subject, content);
        }

        public void SendEmail(string mailto, string cc, string subject, string content, DateTime? sendDate)
        {
            //ConfigurationManager instance = ConfigurationManager.GetInstance(this.context);
            T_Email email = new T_Email {
                Email_Id = this.context.GetNextIdentity_Int(false),
                //Email_Sender = instance.GetConfigurationByKey("SENDMAILNAME"),
                Email_Receiver = mailto,
                Email_Cc = cc,
                Email_Title = subject,
                Email_Content = content,
                Email_TypeEnum = 0,
                Email_StatusEnum = 0,
                Email_SendDate = sendDate
            };
            this.context.Insert(email);
        }
    }
}

