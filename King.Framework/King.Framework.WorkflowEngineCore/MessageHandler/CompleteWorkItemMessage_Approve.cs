namespace King.Framework.WorkflowEngineCore.MessageHandler
{
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.Extends;
    using King.Framework.WorkflowEngineCore.InternalCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    internal class CompleteWorkItemMessage_Approve : CompleteWorkItemMessage
    {
        public CompleteWorkItemMessage_Approve(ProcessEngine engine, SysWorkItem wi) : base(engine, wi)
        {
        }

        protected void CancelUncompletedWorkItems(SysActivityInstance ai)
        {
            IEnumerable<SysWorkItem> enumerable = from x in ai.WorkItems
                where !x.Status.HasValue || (x.Status == 0)
                select x;
            DateTime now = DateTime.Now;
            int num = 0;
            foreach (SysWorkItem item in enumerable)
            {
                if (item.WorkItemId != base.CurrentWorkItem.WorkItemId)
                {
                    item.Status = 3;
                    item.EndTime = new DateTime?(now);
                    base.PICacheFactory.UpdateWorkItem(item, base.PI, ai);
                    num++;
                }
            }
        }

        protected void CancelUncompletedWorkItems(SysWorkItemApproveGroup group)
        {
            var items=group.WorkItems;
            var enumerable = (from x in items 
                where !x.Status.HasValue || (x.Status == 0)
                select x);
            var te = group.WorkItems;
            
            var now = DateTime.Now;
            int num = 0;
            foreach (SysWorkItem item in enumerable)
            {
                if (item.WorkItemId != base.CurrentWorkItem.WorkItemId)
                {
                    item.Status = 3;
                    item.EndTime = new DateTime?(now);
                    base.PICacheFactory.UpdateWorkItem(item, base.PI, base.AI);
                    num++;
                }
            }
        }

        protected void CheckStatus()
        {
            base.CurrentWorkItem.SureStatus();
            if (base.CurrentWorkItem.Status == 1)
            {
                throw new ApplicationException("工作项已经完成，不可重复完成");
            }
            if ((base.CurrentWorkItem.Status != 0) && (base.CurrentWorkItem.Status != 0x63))
            {
                throw new ApplicationException("该工作项已被取消");
            }
        }

        private void CompleteMutexWorkItems(SysWorkItem wi)
        {
            Func<SysWorkItem, bool> predicate = null;
            wi.AssertHasGroup();
            int local1 = wi.ApproveGroupId.Value;
            ICollection<SysWorkItem> workItems = wi.ApproveGroup.WorkItems;
            SysWorkItem main_item = null;
            bool? isProxy = wi.IsProxy;
            if (isProxy.HasValue ? isProxy.GetValueOrDefault() : false)
            {
                if (predicate == null)
                {
                    predicate = p => p.WorkItemId == wi.ProxyWorkItemId;
                }
                main_item = (from p in workItems
                    where !p.IsProxy.HasValue || (p.IsProxy == false)
                    select p).FirstOrDefault<SysWorkItem>(predicate);
                if (main_item == null)
                {
                    throw new ApplicationException("未被代理的原始工作项,如是4.0版本之前的数据，请先清除数据库，重新发布。");
                }
                if (main_item.WorkItemId != wi.WorkItemId)
                {
                    main_item.Status = 3;
                    main_item.EndTime = new DateTime?(DateTime.Now);
                    base.PICacheFactory.UpdateWorkItem(main_item, base.PI, base.AI);
                }
            }
            else
            {
                main_item = wi;
            }
            if (workItems.Where<SysWorkItem>(delegate (SysWorkItem p) {
                if (p.IsProxy != true)
                {
                    return false;
                }
                int? proxyWorkItemId = p.ProxyWorkItemId;
                int workItemId = main_item.WorkItemId;
                return ((proxyWorkItemId.GetValueOrDefault() == workItemId) && proxyWorkItemId.HasValue);
            }).ToList<SysWorkItem>().Count > 0)
            {
                int num = 0;
                foreach (SysWorkItem item in workItems)
                {
                    if (((item.Status == 0) || !item.Status.HasValue) && (item.WorkItemId != wi.WorkItemId))
                    {
                        item.Status = 3;
                        item.EndTime = new DateTime?(DateTime.Now);
                        base.PICacheFactory.UpdateWorkItem(item, base.PI, base.AI);
                        num++;
                    }
                }
            }
        }

        public static int ConvertBoolToApproveResult(bool approve_result)
        {
            if (!approve_result)
            {
                return 0;
            }
            return 1;
        }

        public override void Execute(Queue<WorkflowMessage> queue)
        {
            if (base.AI.Activity.ActivityType == 6)
            {
                this.InternalExecuteForApprove(queue);
            }
            else
            {
                base.Execute(queue);
            }
        }

        private SysWorkItemApproveGroup GetCurrentWorkItemGroup()
        {
            SysWorkItemApproveGroup approveGroup = base.CurrentWorkItem.ApproveGroup;
            if (approveGroup == null)
            {
                throw new ApplicationException("从工作项获取分组失败");
            }
            return approveGroup;
        }

        private void InternalExecuteForApprove(Queue<WorkflowMessage> queue)
        {
            bool flag;
            this.CheckStatus();
            this.JustCompleteCurrentWorkItem();
            this.CompleteMutexWorkItems(base.CurrentWorkItem);
            base.CurrentWorkItem.AssertHasGroup();
            SysWorkItemApproveGroup currentWorkItemGroup = this.GetCurrentWorkItemGroup();
            if (this.TryCompleteGroup(currentWorkItemGroup, out flag))
            {
                bool flag3;
                this.CancelUncompletedWorkItems(currentWorkItemGroup);
                currentWorkItemGroup.ApproveResult = new int?(ConvertBoolToApproveResult(flag));
                currentWorkItemGroup.ApproveTime = new DateTime?(DateTime.Now);
                base.PICacheFactory.UpdateWorkItemGroup(currentWorkItemGroup);
                SysActivityInstance activityInstance = base.CurrentWorkItem.ActivityInstance;
                if (this.TryCompleteActivity(activityInstance, out flag3))
                {
                    this.CancelUncompletedWorkItems(activityInstance);
                    activityInstance.ApproveResult = new int?(ConvertBoolToApproveResult(flag3));
                    activityInstance.ExpressionValue = activityInstance.ApproveResult;
                    WorkflowMessage item = base.Engine.NewCompleteActivityMessage(base.AI);
                    queue.Enqueue(item);
                }
            }
        }

        protected void JustCompleteCurrentWorkItem()
        {
            base.CurrentWorkItem.Status = 1;
            base.CurrentWorkItem.EndTime = new DateTime?(DateTime.Now);
            base.PICacheFactory.UpdateWorkItem(base.CurrentWorkItem, base.PI, base.AI);
        }

        private bool TryCompleteActivity(SysActivityInstance ai, out bool approve_result)
        {
            approve_result = false;
            bool flag = false;
            ICollection<SysWorkItemApproveGroup> approveGroups = ai.ApproveGroups;
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            foreach (SysWorkItemApproveGroup group in approveGroups)
            {
                if (group.ApproveResult == 1)
                {
                    num++;
                    num2++;
                }
                else if (group.ApproveResult == 0)
                {
                    num3++;
                    num2++;
                }
            }
            SysActivity activity = ai.Activity;
            if (activity.PassType == 1)
            {
                activity.SureMinPassNum();
                int num4 = activity.MinPassNum.Value;
                if (num2 >= approveGroups.Count)
                {
                    flag = true;
                    approve_result = num >= num4;
                    return flag;
                }
                bool? sumAfterAllComplete = activity.SumAfterAllComplete;
                if (sumAfterAllComplete.HasValue ? sumAfterAllComplete.GetValueOrDefault() : false)
                {
                    return false;
                }
                if (num >= num4)
                {
                    flag = true;
                    approve_result = true;
                    return flag;
                }
                if (num3 > (approveGroups.Count - num4))
                {
                    flag = true;
                    approve_result = false;
                }
                return flag;
            }
            if (activity.PassType == 2)
            {
                activity.SureMinPassRatio();
                double num5 = activity.MinPassRatio.Value;
                int count = approveGroups.Count;
                if (num2 >= count)
                {
                    flag = true;
                    approve_result = (num * 100) >= (num5 * count);
                    return flag;
                }
                bool? nullable8 = activity.SumAfterAllComplete;
                if (nullable8.HasValue ? nullable8.GetValueOrDefault() : false)
                {
                    return false;
                }
                if ((num * 100) >= (count * num5))
                {
                    flag = true;
                    approve_result = true;
                    return flag;
                }
                if ((num3 * 100) > (count * (100.0 - num5)))
                {
                    flag = true;
                    approve_result = false;
                }
                return flag;
            }
            foreach (SysWorkItemApproveGroup group2 in approveGroups)
            {
                if (group2.ApproveResult == 1)
                {
                    flag = true;
                    approve_result = true;
                    return flag;
                }
                if (group2.ApproveResult == 0)
                {
                    flag = true;
                    approve_result = false;
                    return flag;
                }
            }
            return flag;
        }

        private bool TryCompleteGroup(SysWorkItemApproveGroup group, out bool approve_result)
        {
            approve_result = false;
            ICollection<SysWorkItem> workItems = group.WorkItems;
            Dictionary<int, WorkItemWrapper> dictionary = new Dictionary<int, WorkItemWrapper>(workItems.Count + 10);
            foreach (SysWorkItem item in workItems)
            {
                WorkItemWrapper wrapper = new WorkItemWrapper(base.PICacheFactory, item, base.PI);
                dictionary.Add(item.WorkItemId, wrapper);
            }
            List<WorkItemWrapper> source = new List<WorkItemWrapper>(workItems.Count);
            foreach (KeyValuePair<int, WorkItemWrapper> pair in dictionary)
            {
                WorkItemWrapper wrapper2 = pair.Value;
                bool? isAdded = wrapper2.WorkItem.IsAdded;
                if (isAdded.HasValue ? isAdded.GetValueOrDefault() : false)
                {
                    if (!wrapper2.WorkItem.ProxyWorkItemId.HasValue)
                    {
                        throw new ApplicationException("加签未设置代理工作项");
                    }
                    wrapper2.Parent = dictionary[wrapper2.WorkItem.ProxyWorkItemId.Value];
                    wrapper2.Parent.AddItems.Add(wrapper2);
                }
                else
                {
                    bool? isProxy = wrapper2.WorkItem.IsProxy;
                    if (isProxy.HasValue ? isProxy.GetValueOrDefault() : false)
                    {
                        if (!wrapper2.WorkItem.ProxyWorkItemId.HasValue)
                        {
                            throw new ApplicationException("代理未设置代理工作项");
                        }
                        wrapper2.Parent = dictionary[wrapper2.WorkItem.ProxyWorkItemId.Value];
                        wrapper2.Parent.ProxyItems.Add(wrapper2);
                    }
                    else
                    {
                        source.Add(wrapper2);
                    }
                }
            }
            foreach (WorkItemWrapper wrapper3 in source)
            {
                wrapper3.TryCompleteDeeply();
            }
            foreach (WorkItemWrapper wrapper5 in source)
            {
                wrapper5.CalResultDeeply();
            }
            SysActivityParticipant participant = base.AI.Activity.ActivityParticipants.FirstOrDefault<SysActivityParticipant>(p => p.ActivityParticipantId == group.ActivityParticipantId);
            if (participant == null)
            {
                throw new ApplicationException("根据群组实例查找【活动参与人】失败");
            }
            bool flag = false;
            if (participant.PassType == 1)
            {
                if (!participant.MinPassNum.HasValue)
                {
                    participant.MinPassNum = 1;
                }
                if (participant.MinPassNum.Value <= 0)
                {
                    participant.MinPassNum = 1;
                }
                int num = 0;
                int num2 = 0;
                int num3 = 0;
                foreach (WorkItemWrapper wrapper7 in source)
                {
                    if (wrapper7.IsCompleted)
                    {
                        num3++;
                        bool? approveResult = wrapper7.ApproveResult;
                        if (approveResult.HasValue ? approveResult.GetValueOrDefault() : false)
                        {
                            num++;
                        }
                        if (wrapper7.ApproveResult.HasValue && !wrapper7.ApproveResult.Value)
                        {
                            num2++;
                        }
                    }
                }
                int num4 = participant.MinPassNum.Value;
                if (num3 >= source.Count)
                {
                    flag = true;
                    approve_result = num >= num4;
                    return flag;
                }
                bool? sumAfterAllComplete = base.AI.Activity.SumAfterAllComplete;
                if (sumAfterAllComplete.HasValue ? sumAfterAllComplete.GetValueOrDefault() : false)
                {
                    return false;
                }
                if (num >= num4)
                {
                    flag = true;
                    approve_result = true;
                    return flag;
                }
                if (num2 > (source.Count - num4))
                {
                    flag = true;
                    approve_result = false;
                }
                return flag;
            }
            if (participant.PassType == 2)
            {
                if (!participant.MinPassRatio.HasValue)
                {
                    throw new ApplicationException("未指定最小通过率");
                }
                if (participant.MinPassRatio <= 0.0)
                {
                    throw new ApplicationException("最小通过率<=0, 它必须介于0-100之间(大于0，小于等于100");
                }
                if (participant.MinPassRatio > 100.0)
                {
                    throw new ApplicationException("最小通过率>100, 它必须介于0-100之间(大于0，小于等于100");
                }
                int num5 = 0;
                int num6 = 0;
                int num7 = 0;
                int count = source.Count;
                foreach (WorkItemWrapper wrapper8 in source)
                {
                    if (wrapper8.IsCompleted)
                    {
                        num7++;
                        bool? nullable17 = wrapper8.ApproveResult;
                        if (nullable17.HasValue ? nullable17.GetValueOrDefault() : false)
                        {
                            num5++;
                        }
                        if (wrapper8.ApproveResult.HasValue && !wrapper8.ApproveResult.Value)
                        {
                            num6++;
                        }
                    }
                }
                double num9 = participant.MinPassRatio.Value;
                if (num7 >= source.Count)
                {
                    flag = true;
                    approve_result = (num5 * 100.0) >= (count * num9);
                    return flag;
                }
                bool? nullable19 = base.AI.Activity.SumAfterAllComplete;
                if (nullable19.HasValue ? nullable19.GetValueOrDefault() : false)
                {
                    return false;
                }
                double num10 = 100.0;
                if ((num5 * num10) >= (count * num9))
                {
                    flag = true;
                    approve_result = true;
                    return flag;
                }
                if (((1.0 * num6) / ((double) count)) > ((100.0 - num9) / 100.0))
                {
                    flag = true;
                    approve_result = true;
                }
                return flag;
            }
            WorkItemWrapper wrapper9 = source.FirstOrDefault<WorkItemWrapper>(p => p.IsCompleted && p.ApproveResult.HasValue);
            if (wrapper9 != null)
            {
                flag = true;
                approve_result = wrapper9.ApproveResult.Value;
            }
            return flag;
        }
    }
}

