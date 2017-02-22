namespace King.Framework.WorkflowEngineCore.InternalCore
{
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Framework.WorkflowEngineCore.Cache;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal class WorkItemWrapper
    {
        private List<WorkItemWrapper> _add_items = new List<WorkItemWrapper>();
        private ProcessInstanceCacheFactory _piCacheFactory;
        private List<WorkItemWrapper> _proxy_items = new List<WorkItemWrapper>();
        public bool? ApproveResult = null;
        public readonly bool IsAdd;
        public bool IsCompleted;
        public readonly bool IsProxy;
        public readonly SysWorkItem WorkItem;

        public WorkItemWrapper(ProcessInstanceCacheFactory cache, SysWorkItem work_item, SysProcessInstance pi)
        {
            this._piCacheFactory = cache;
            this.WorkItem = work_item;
            bool? isAdded = work_item.IsAdded;
            this.IsAdd = isAdded.HasValue ? isAdded.GetValueOrDefault() : false;
            bool? isProxy = work_item.IsProxy;
            this.IsProxy = isProxy.HasValue ? isProxy.GetValueOrDefault() : false;
            int? status = work_item.Status;
            this.IsCompleted = (status.HasValue ? status.GetValueOrDefault() : 0) != 0;
            this.ApproveResult = null;
            if (work_item.Status == 1)
            {
                this.ApproveResult = new bool?(this.IsPass(work_item, pi));
            }
        }

        public void CalResultDeeply()
        {
            if (((this.AddItems.Count != 0) || (this.ProxyItems.Count != 0)) && this.IsCompleted)
            {
                if (this.ApproveResult == true)
                {
                    foreach (WorkItemWrapper wrapper in this.AddItems)
                    {
                        wrapper.CalResultDeeply();
                        if (wrapper.ApproveResult == false)
                        {
                            this.ApproveResult = false;
                            break;
                        }
                    }
                }
                else if (this.ApproveResult != false)
                {
                    foreach (WorkItemWrapper wrapper2 in this.ProxyItems)
                    {
                        wrapper2.CalResultDeeply();
                        if (wrapper2.ApproveResult.HasValue)
                        {
                            this.ApproveResult = wrapper2.ApproveResult;
                            break;
                        }
                    }
                }
            }
        }

        private bool IsPass(SysWorkItem work_item, SysProcessInstance pi)
        {
            EntityData data = new EntityCache(this._piCacheFactory.Manager).GetObject(pi.Process.ActivityEntity, work_item.WorkItemId);
            if (data == null)
            {
                throw new ApplicationException("获取活动数据失败");
            }
            object obj2 = data[ApproveActivityFields.ApproveResult];
            if (obj2 == null)
            {
                throw new ApplicationException("签核结果为空");
            }
            return Convert.ToBoolean(obj2);
        }

        public void TryCompleteDeeply()
        {
            if ((this.AddItems.Count != 0) || (this.ProxyItems.Count != 0))
            {
                if (this.IsCompleted)
                {
                    foreach (WorkItemWrapper wrapper in this.AddItems)
                    {
                        wrapper.TryCompleteDeeply();
                        this.IsCompleted &= wrapper.IsCompleted;
                    }
                }
                else
                {
                    foreach (WorkItemWrapper wrapper2 in this.ProxyItems)
                    {
                        wrapper2.TryCompleteDeeply();
                        this.IsCompleted |= wrapper2.IsCompleted;
                    }
                }
            }
        }

        public List<WorkItemWrapper> AddItems
        {
            get
            {
                return this._add_items;
            }
        }

        public WorkItemWrapper Parent { get; set; }

        public List<WorkItemWrapper> ProxyItems
        {
            get
            {
                return this._proxy_items;
            }
        }
    }
}

