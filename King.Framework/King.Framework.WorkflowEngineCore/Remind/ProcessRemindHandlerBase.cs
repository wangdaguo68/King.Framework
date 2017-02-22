namespace King.Framework.WorkflowEngineCore.Remind
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Framework.Linq;
    using King.Framework.WorkflowEngineCore;
    using King.Framework.WorkflowEngineCore.Cache;
    using King.Framework.WorkflowEngineCore.InternalCore;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;

    internal abstract class ProcessRemindHandlerBase
    {
        private DataContext _context;
        private ICustomRemindHandler _customHandler;
        private IEmailHandler _emailHandler;
        private DynamicManager _manager;
        private ProcessInstanceCacheFactory _piCacheFactory;
        private IShortMessageHandler _shortMsgHandler;
        protected RemindTemplateModelCreator creator;

        public ProcessRemindHandlerBase(ProcessInstanceCacheFactory cache, SysProcessInstance pi, SysActivityInstance ai, UseTimeType useTime, SysWorkItem wi = null, int? result = new int?())
        {
            this.creator = new RemindTemplateModelCreator(cache, pi, ai, useTime, wi, result);
            this._piCacheFactory = cache;
            this._context = cache.Context;
            this._manager = cache.Manager;
            this.InitRemindList();
        }

        protected virtual void AddRemindToken(IUser user)
        {
            if (WebConfigAppSettings.UseSSOModel)
            {
                this.Model.AprovePageInnerURL = this.creator.AprovePageInnerURL;
                this.Model.AprovePageOuterURL = this.creator.AprovePageOuterURL;
                this.Model.ProcessDetailInnerURL = this.creator.ProcessDetailInnerURL;
                this.Model.ProcessDetailOuterURL = this.creator.ProcessDetailOuterURL;
            }
            else
            {
                SysProcessRemindToken token = new SysProcessRemindToken {
                    TokenId = this._manager.GetNextIdentity(),
                    Token = string.Format("{0}-{1}", Guid.NewGuid(), Guid.NewGuid()),
                    UserId = new int?(user.User_ID)
                };
                if (this.creator.WorkItem != null)
                {
                    token.WorkItemId = new int?(this.creator.WorkItem.WorkItemId);
                }
                token.ProcessId = this.creator.PI.ProcessId;
                token.ActivityId = this.creator.AI.ActivityId;
                token.FailTime = new DateTime?(DateTime.Now.Date.AddDays(7.0));
                this._piCacheFactory.AddProcessRemindToken(token);
                string str = ConfigurationManager.AppSettings["token_id"] ?? "token_id";
                string str2 = ConfigurationManager.AppSettings["token_value"] ?? "token_value";
                string str3 = string.Format("&{0}={1}&{2}={3}", new object[] { str, token.TokenId, str2, token.Token });
                if (!string.IsNullOrWhiteSpace(this.creator.AprovePageInnerURL))
                {
                    this.Model.AprovePageInnerURL = string.Format("{0}{1}", this.creator.AprovePageInnerURL, str3);
                }
                if (!string.IsNullOrWhiteSpace(this.creator.AprovePageOuterURL))
                {
                    this.Model.AprovePageOuterURL = string.Format("{0}{1}", this.creator.AprovePageOuterURL, str3);
                }
                if (!string.IsNullOrWhiteSpace(this.creator.ProcessDetailInnerURL))
                {
                    this.Model.ProcessDetailInnerURL = string.Format("{0}{1}", this.creator.ProcessDetailInnerURL, str3);
                }
                if (!string.IsNullOrWhiteSpace(this.creator.ProcessDetailOuterURL))
                {
                    this.Model.ProcessDetailOuterURL = string.Format("{0}{1}", this.creator.ProcessDetailOuterURL, str3);
                }
            }
        }

        protected void CreateModel()
        {
            this.creator.CreateModel();
        }

        public abstract void Execute();
        public abstract void InitRemindList();
        protected void RemindForParticipant(int? remindType, SysProcessParticipant part, SysProcessRemindTemplate template)
        {
            if (template == null)
            {
                throw new ApplicationException("RemindTemplate为空");
            }
            foreach (IUser user in ParticipantHelper.GetUsers(this._context, part, this.PI, this.AI, this.creator.WiOwnerId).Distinct<IUser>())
            {
                string str3;
                this.AddRemindToken(user);
                string remindTitleByTemplate = this.creator.GetRemindTitleByTemplate(template, user);
                string remindContentByTemplate = this.creator.GetRemindContentByTemplate(template, user);
                int valueOrDefault = remindType.GetValueOrDefault();
                if (remindType.HasValue)
                {
                    switch (valueOrDefault)
                    {
                        case 0:
                            if (this.EmailHandler == null)
                            {
                                throw new ApplicationException("未找到邮件服务，无法发送邮件提醒");
                            }
                            this.EmailHandler.SendEmail(user, remindTitleByTemplate, remindContentByTemplate, this.Model.AprovePageOuterURL, this.Model.ProcessDetailOuterURL);
                            break;

                        case 1:
                            if (this.ShortMessageHandler == null)
                            {
                                throw new ApplicationException("未找到短信服务，无法发送短信提醒");
                            }
                            goto Label_00EA;

                        case 2:
                            if (this.CustomRemindHandler == null)
                            {
                                throw new ApplicationException("未找到自定义提醒服务，无法处理自定义提醒");
                            }
                            goto Label_013F;
                    }
                }
                continue;
            Label_00EA:
                str3 = null;
                if (string.IsNullOrEmpty(remindTitleByTemplate))
                {
                    str3 = remindContentByTemplate;
                }
                else
                {
                    StringBuilder builder = new StringBuilder(remindTitleByTemplate);
                    builder.AppendLine();
                    builder.AppendLine(remindContentByTemplate);
                    str3 = builder.ToString();
                }
                this.ShortMessageHandler.SendMessage(user, str3);
                continue;
            Label_013F:
                this.CustomRemindHandler.Execute(user, remindTitleByTemplate, remindContentByTemplate, this.PI, this.AI, this.WorkItem, this.Model);
            }
        }

        protected SysActivityInstance AI
        {
            get
            {
                return this.creator.AI;
            }
        }

        protected ICustomRemindHandler CustomRemindHandler
        {
            get
            {
                string defaultCustomRemindInterface = ConfigurationManager.AppSettings[MetaConfig.CustomRemindInterfacePath];
                if (string.IsNullOrWhiteSpace(defaultCustomRemindInterface))
                {
                    defaultCustomRemindInterface = MetaConfig.DefaultCustomRemindInterface;
                }
                this._customHandler = CustomHandlerLoader.GetHandlerWithConfiguration<ICustomRemindHandler>(defaultCustomRemindInterface, new object[] { this._context });
                return this._customHandler;
            }
        }

        protected IEmailHandler EmailHandler
        {
            get
            {
                string defaultEmailInterface = ConfigurationManager.AppSettings[MetaConfig.EmailInterfacePath];
                if (string.IsNullOrWhiteSpace(defaultEmailInterface))
                {
                    defaultEmailInterface = MetaConfig.DefaultEmailInterface;
                }
                this._emailHandler = CustomHandlerLoader.GetHandlerWithConfiguration<IEmailHandler>(defaultEmailInterface, new object[] { this._context });
                return this._emailHandler;
            }
        }

        protected RemindTemplateModel Model
        {
            get
            {
                return this.creator.Model;
            }
        }

        protected SysProcessInstance PI
        {
            get
            {
                return this.creator.PI;
            }
        }

        protected IShortMessageHandler ShortMessageHandler
        {
            get
            {
                string defaultShortMessageInterface = ConfigurationManager.AppSettings[MetaConfig.ShortMessageInterfacePath];
                if (string.IsNullOrWhiteSpace(defaultShortMessageInterface))
                {
                    defaultShortMessageInterface = MetaConfig.DefaultShortMessageInterface;
                }
                this._shortMsgHandler = CustomHandlerLoader.GetHandlerWithConfiguration<IShortMessageHandler>(defaultShortMessageInterface, new object[] { this._context });
                return this._shortMsgHandler;
            }
        }

        protected SysWorkItem WorkItem
        {
            get
            {
                return this.creator.WorkItem;
            }
        }
    }
}

