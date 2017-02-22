namespace King.Framework.WorkflowEngineCore.Remind
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Framework.WorkflowEngineCore;
    using King.Framework.WorkflowEngineCore.Cache;
    using King.Framework.WorkflowEngineCore.InternalCore;
    using RazorEngine;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Web;

    internal class RemindTemplateModelCreator
    {
        private SysActivityInstance _ai;
        private RemindTemplateModel _model;
        private SysProcessInstance _pi;
        private ProcessInstanceCacheFactory _piCacheFactory;
        private ProcessTemplateResultType _resultType;
        private UseTimeType _useTime;
        private SysWorkItem _wi;
        private int? _wiOwnerId;
        public string AprovePageInnerURL;
        public string AprovePageOuterURL;
        public string ProcessDetailInnerURL;
        public string ProcessDetailOuterURL;
        private string serverHost;

        public RemindTemplateModelCreator(ProcessInstanceCacheFactory cache, SysProcessInstance pi, SysActivityInstance ai, UseTimeType useTime, SysWorkItem wi = null, int? result = new int?())
        {
            this._pi = pi;
            this._ai = ai;
            this._useTime = useTime;
            if (wi != null)
            {
                this._wi = wi;
                this._wiOwnerId = wi.OwnerId;
            }
            if (result.HasValue)
            {
                this._resultType = (ProcessTemplateResultType)result.Value;
            }
            this.serverHost = WebConfigAppSettings.GetServerHost();
            this._piCacheFactory = cache;
        }

        protected virtual void CreateApprovePageUrl()
        {
            string str = this.PICacheFactory.GetWorkItemCompletePageUrl(this._wi, this._pi, this._ai, false);
            if (WebConfigAppSettings.UseSSOModel)
            {
                string innerRootURL = WebConfigAppSettings.InnerRootURL;
                string outerRootURL = WebConfigAppSettings.OuterRootURL;
                this.AprovePageInnerURL = Path.Combine(innerRootURL, str);
                this.AprovePageOuterURL = Path.Combine(outerRootURL, str);
            }
            else
            {
                this.AprovePageInnerURL = Path.Combine(this.serverHost, str);
                this.AprovePageOuterURL = this.AprovePageInnerURL;
            }
        }

        public virtual void CreateModel()
        {
            EntityCache cache = new EntityCache(this._piCacheFactory.Manager);
            this._model = new RemindTemplateModel();
            this._model.CurrentTime = DateTime.Now.ToString();
            this.CreateProcessDetailUrl();
            SysEntity processEntity = this.PI.Process.ProcessEntity;
            EntityData data = cache.GetObjectFull(processEntity, this.PI.ObjectId, this._piCacheFactory);
            if (data != null)
            {
                this._model.PI_Data = data.Dictionary;
            }
            if ((this.WorkItem != null) && this.WorkItem.ActivityInstanceId.HasValue)
            {
                if (this.PI.Process.ProcessCategory == 0)
                {
                    SysEntity activityEntity = this.PI.Process.ActivityEntity;
                    EntityData data2 = cache.GetObjectFull(activityEntity, this.WorkItem.ActivityInstanceId.Value, this._piCacheFactory);
                    if (data2 != null)
                    {
                        this._model.AI_Data = data2.Dictionary;
                    }
                }
                else if (this.PI.Process.ProcessCategory == 1)
                {
                    SysEntity entity = this.PI.Process.ActivityEntity;
                    EntityData data3 = cache.GetObjectFull(entity, this.WorkItem.WorkItemId, this._piCacheFactory);
                    if (data3 != null)
                    {
                        this._model.AI_Data = data3.Dictionary;
                    }
                }
                this.CreateApprovePageUrl();
            }
        }

        protected virtual void CreateProcessDetailUrl()
        {
            if (WebConfigAppSettings.UseSSOModel)
            {
                string str = string.Format("/SystemManagement/ProcessInstanceDetail.aspx?id={0}", this.PI.ProcessInstanceId);
                string innerRootURL = WebConfigAppSettings.InnerRootURL;
                string outerRootURL = WebConfigAppSettings.OuterRootURL;
                this.ProcessDetailInnerURL = Path.Combine(innerRootURL, str);
                this.ProcessDetailOuterURL = Path.Combine(outerRootURL, str);
            }
            else
            {
                string str4 = string.Format("SystemManagement/ProcessInstanceDetail.aspx?id={0}", this.PI.ProcessInstanceId);
                this.ProcessDetailInnerURL = Path.Combine(this.serverHost, str4);
                this.ProcessDetailOuterURL = Path.Combine(this.serverHost, str4);
            }
        }

        public string GetRemindContentByTemplate(SysProcessRemindTemplate template, IUser user)
        {
            string str3;
            try
            {
                this._model.ReceiveUser = user.User_Name;
                str3 = HttpUtility.HtmlDecode(Razor.Parse<RemindTemplateModel>(template.TemplateContentText, this._model));
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("提醒模板解析失败，错误：{0}", exception.Message));
            }
            return str3;
        }

        public string GetRemindTitleByTemplate(SysProcessRemindTemplate template, IUser user)
        {
            string str3;
            try
            {
                this._model.ReceiveUser = user.User_Name;
                str3 = HttpUtility.HtmlDecode(Razor.Parse<RemindTemplateModel>(template.TemplateTitleText, this._model));
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("提醒模板解析失败，错误：{0}", exception.Message));
            }
            return str3;
        }

        private string UnicodeEncode(string str)
        {
            int num = Convert.ToInt32("4e00", 0x10);
            int num2 = Convert.ToInt32("9fff", 0x10);
            string str2 = "";
            for (int i = 0; i < str.Length; i++)
            {
                int num4 = char.ConvertToUtf32(str, i);
                if ((num4 >= num) && (num4 <= num2))
                {
                    str2 = str2 + "{" + num4.ToString() + "}";
                }
                else
                {
                    str2 = str2 + str[i].ToString();
                }
            }
            return str2;
        }

        public SysActivity Activity
        {
            get
            {
                return this._ai.Activity;
            }
        }

        public SysActivityInstance AI
        {
            get
            {
                return this._ai;
            }
        }

        public RemindTemplateModel Model
        {
            get
            {
                return this._model;
            }
        }

        public SysProcessInstance PI
        {
            get
            {
                return this._pi;
            }
        }

        public ProcessInstanceCacheFactory PICacheFactory
        {
            get
            {
                return this._piCacheFactory;
            }
        }

        public ProcessTemplateResultType ResultType
        {
            get
            {
                return this._resultType;
            }
        }

        public UseTimeType UseTime
        {
            get
            {
                return this._useTime;
            }
        }

        public int? WiOwnerId
        {
            get
            {
                return this._wiOwnerId;
            }
        }

        public SysWorkItem WorkItem
        {
            get
            {
                return this._wi;
            }
        }
    }
}

