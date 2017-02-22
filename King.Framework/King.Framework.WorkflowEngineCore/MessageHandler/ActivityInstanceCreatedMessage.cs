namespace King.Framework.WorkflowEngineCore.MessageHandler
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Framework.OrgLibrary;
    using King.Framework.WorkflowEngineCore.Cache;
    using King.Framework.WorkflowEngineCore.InternalCore;
    using King.Framework.WorkflowEngineCore.Remind;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class ActivityInstanceCreatedMessage : WorkflowMessage
    {
        private SysActivityInstance _ai;

        public ActivityInstanceCreatedMessage(ProcessEngine engine, SysActivityInstance activityInstance) : base(engine)
        {
            if (activityInstance == null)
            {
                throw new ArgumentNullException("ai");
            }
            this._ai = activityInstance;
        }

        private void AssertGroupHasUsers(SysActivityParticipant act_part, SysWorkItemApproveGroup group, List<IUser> targetUsers)
        {
            if (targetUsers.Count <= 0)
            {
                bool? isPassedWithNoParticipants = act_part.Activity.IsPassedWithNoParticipants;
                if (!(isPassedWithNoParticipants.HasValue ? isPassedWithNoParticipants.GetValueOrDefault() : false))
                {
                    throw new ApplicationException(string.Format("参与人[{0}]没有匹配的用户", act_part.ProcessParticipant.ParticipantName));
                }
                group.ApproveResult = 1;
                group.ApproveTime = new DateTime?(DateTime.Now);
                base.PICacheFactory.UpdateWorkItemGroup(group);
            }
        }

        private void CalDecisionValue()
        {
            SysActivity activity = this.AI.Activity;
            if ((activity.Expressions == null) || (activity.Expressions.Count == 0))
            {
                throw new ApplicationException("活动没有表达式");
            }
            if (!activity.ExpressionId.HasValue)
            {
                throw new ApplicationException("未指定活动的表达式");
            }
            SysExpression expr = activity.Expressions.FirstOrDefault<SysExpression>(e => e.ExpressionId == activity.ExpressionId.Value);
            if (expr == null)
            {
                throw new ApplicationException("指定的表达式不在活动的表达式中");
            }
            Queue<SysExpression> calOrder = ExpressionHelper.GetCalOrder(activity.Expressions.ToList<SysExpression>());
            if (calOrder.Count<SysExpression>(p => (p.ExpressionId == expr.ExpressionId)) <= 0)
            {
                throw new ApplicationException("无法计算表达式的值");
            }
            EntityCache cache = new EntityCache(base.Manager);
            ExpressionCache cache2 = new ExpressionCache();
            while (calOrder.Count > 0)
            {
                SysExpression expression = calOrder.Dequeue();
                object obj2 = ExpressionHelper.GetHelper(expression).GetValue(expression, cache, cache2, base.PI, this.AI);
                if (expression.ExpressionId == expr.ExpressionId)
                {
                    if (obj2 == null)
                    {
                        throw new ApplicationException("决策活动的值返回null");
                    }
                    if (obj2.GetType() != typeof(bool))
                    {
                        throw new ApplicationException("决策活动的值不是布尔类型");
                    }
                    this.AI.ExpressionValue = new int?(Convert.ToInt32(obj2));
                    return;
                }
            }
        }

        private void CopyActivityInstanceData()
        {
            List<SysTransitionInstance> list = this.AI.ToTransitionInstances.ToList<SysTransitionInstance>();
            if (list.Count == 0)
            {
                throw new ApplicationException("没有进入此活动的转换实例");
            }
            SysTransitionInstance instance = list[0];
            SysEntity activityEntity = instance.PreActivityInstance.Activity.Process.ActivityEntity;
            EntityData data = new EntityCache(base.Manager).GetObject(activityEntity, instance.PreActivityInstanceId.Value);
            if (data != null)
            {
                string keyFieldName = activityEntity.GetKeyFieldName();
                data[keyFieldName] = this.AI.ActivityInstanceId;
                string activityTypeField = "ActivityType";
                if (activityEntity.Fields.ToList<SysField>().FirstOrDefault<SysField>(i => (i.FieldName == activityTypeField)) != null)
                {
                    data[activityTypeField] = this.AI.Activity.ActivityType;
                }
                base.Manager.Insert(data);
            }
        }

        private void CreateProxyWorkItems(SysWorkItem source_work_item, List<SysProcessProxy> proxies)
        {
            foreach (SysProcessProxy proxy in proxies)
            {
                SysWorkItem item2 = new SysWorkItem {
                    ProcessInstanceId = new int?(base.PI.ProcessInstanceId),
                    ActivityInstanceId = new int?(this.AI.ActivityInstanceId),
                    CreateTime = new DateTime?(DateTime.Now)
                };
                item2.EndTime = null;
                item2.IsProxy = true;
                item2.OwnerId = new int?(proxy.ProxyId.Value);
                item2.ProxyUserId = source_work_item.OwnerId;
                item2.RelativeObjectId = new int?(base.PI.ObjectId);
                item2.Status = 0;
                item2.WorkItemType = 0;
                item2.WwfBookmarkId = null;
                item2.ActivityParticipantId = source_work_item.ActivityParticipantId;
                item2.ParticipantId = source_work_item.ParticipantId;
                item2.ApproveGroupId = source_work_item.ApproveGroupId;
                item2.ProxyWorkItemId = new int?(source_work_item.WorkItemId);
                SysWorkItem wi = item2;
                if (this.AI.Activity.TimeLimit.HasValue)
                {
                    wi.DeadLine = new DateTime?(DateTime.Now.AddDays((double) this.AI.Activity.TimeLimit.Value));
                }
                base.PICacheFactory.AddWorkItem(wi, base.PI, this.AI);
            }
        }

        private void CreateWorkItem(IUser user, SysWorkItemApproveGroup group)
        {
            SysWorkItem wi = new SysWorkItem {
                ProcessInstanceId = new int?(base.PI.ProcessInstanceId),
                ActivityInstanceId = new int?(this.AI.ActivityInstanceId),
                CreateTime = new DateTime?(DateTime.Now),
                EndTime = null,
                IsProxy = false,
                OwnerId = new int?(user.User_ID),
                ProxyUserId = null,
                RelativeObjectId = new int?(base.PI.ObjectId),
                Status = 0,
                WorkItemType = 0,
                WwfBookmarkId = null,
                ActivityParticipantId = group.ActivityParticipantId,
                ParticipantId = group.ParticipantId,
                ApproveGroupId = new int?(group.ApproveGroupId)
            };
            IApproveUser approveUser = this.GetApproveUser(user);
            if (approveUser != null)
            {
                wi.IsMajor = approveUser.IsMajor;
                wi.FlagInt = approveUser.FlagInt;
                wi.FlagString = approveUser.FlagString;
            }
            if (this.AI.Activity.TimeLimit.HasValue)
            {
                wi.DeadLine = new DateTime?(DateTime.Now.AddDays((double) this.AI.Activity.TimeLimit.Value));
            }
            base.PICacheFactory.AddWorkItem(wi, base.PI, this.AI);
            DateTime curr_time = DateTime.Now;
            List<SysProcessProxy> proxies = base.PI.Process.ProcessProxies.Where<SysProcessProxy>(delegate (SysProcessProxy p) {
                if ((p.OwnerId.Value == user.User_ID) && (p.StartTime.Value <= curr_time))
                {
                    DateTime? endTime = p.EndTime;
                    DateTime time = curr_time;
                    if (endTime.HasValue && ((endTime.GetValueOrDefault() >= time)))
                    {
                        return (p.Status == 0);
                    }
                }
                return false;
            }).ToList<SysProcessProxy>();
            if ((proxies != null) && (proxies.Count > 0))
            {
                this.CreateProxyWorkItems(wi, proxies);
            }
        }

        private SysWorkItemApproveGroup CreateWorkItemGroup()
        {
            SysWorkItemApproveGroup group = new SysWorkItemApproveGroup {
                ProcessInstanceId = new int?(base.PI.ProcessInstanceId),
                ApproveGroupId = base.Manager.GetNextIdentity(),
                ActivityInstanceId = new int?(this.AI.ActivityInstanceId),
                CreateTime = new DateTime?(DateTime.Now),
                ApproveResult = null,
                ApproveTime = null
            };
            base.PICacheFactory.AddWorkItemGroup(group, base.PI, this.AI);
            return group;
        }

        private SysWorkItemApproveGroup CreateWorkItemGroup(SysActivityParticipant ap)
        {
            SysWorkItemApproveGroup group = new SysWorkItemApproveGroup {
                ApproveGroupId = base.Manager.GetNextIdentity(),
                ProcessInstanceId = new int?(base.PI.ProcessInstanceId),
                ActivityInstanceId = new int?(this.AI.ActivityInstanceId),
                ActivityParticipantId = new long?(ap.ActivityParticipantId),
                ParticipantId = ap.ParticipantId,
                CreateTime = new DateTime?(DateTime.Now),
                ApproveResult = null,
                ApproveTime = null
            };
            base.PICacheFactory.AddWorkItemGroup(group, base.PI, this.AI);
            return group;
        }

        public override void Execute(Queue<WorkflowMessage> queue)
        {
            SysActivity activity = this.AI.Activity;
            SysProcess process = activity.Process;
            if (((activity.ActivityType == 3) || (activity.ActivityType == 1)) || (activity.ActivityType == 6))
            {
                if (activity.ExecType == 0)
                {
                    new ActivityRemindHandler(base.PICacheFactory, base.PI, this.AI, ActivityRemindUseTimeType.ActivityStart, null, null).Execute();
                    this.ExecuteForManual(activity, process, queue);
                }
                else
                {
                    if (activity.ExecType != 1)
                    {
                        throw new ApplicationException("未知的活动执行类型");
                    }
                    base.Manager.ExecutePython(base.PI, activity.JScriptText);
                    this.ExecuteForAuto(activity, process);
                    WorkflowMessage item = base.Engine.NewCompleteActivityMessage(this.AI);
                    queue.Enqueue(item);
                }
            }
            else if (activity.ActivityType == 2)
            {
                this.CopyActivityInstanceData();
                this.CalDecisionValue();
                WorkflowMessage message2 = base.Engine.NewCompleteActivityMessage(this.AI);
                queue.Enqueue(message2);
            }
            else
            {
                if (activity.ActivityType != 4)
                {
                    throw new ApplicationException("未知活动类型");
                }
                base.Manager.ExecutePython(base.PI, activity.JScriptText);
                this.ExecuteForAuto(activity, process);
                WorkflowMessage message3 = base.Engine.NewCompleteActivityMessage(this.AI);
                queue.Enqueue(message3);
            }
        }

        private void ExecuteForAuto(SysActivity activity, SysProcess process)
        {
            foreach (SysActivityOperation operation in 
                (from a in activity.ActivityOperations.Where<SysActivityOperation>(delegate (SysActivityOperation p)
                {
                    long? activityId = p.ActivityId;
                    long num = activity.ActivityId;
                    return (activityId.GetValueOrDefault() == num) && activityId.HasValue;
            })
                orderby a.OperationId
                select a).ToList<SysActivityOperation>())
            {
                if (operation.OperationType != 2)
                {
                    throw new ApplicationException("目前只支持Python脚本");
                }
                base.Manager.ExecutePython(base.PI, operation.JScriptText);
            }
        }

        private void ExecuteForManual(SysActivity activity, SysProcess process, Queue<WorkflowMessage> queue)
        {
            if ((activity.ActivityParticipants == null) || (activity.ActivityParticipants.Count == 0))
            {
                throw new ApplicationException("未指定参与人");
            }
            Dictionary<SysWorkItemApproveGroup, List<IUser>> dictionary = new Dictionary<SysWorkItemApproveGroup, List<IUser>>(20);
            List<IUser> list = new List<IUser>();
            if (this.AI.UserDefinedApproveUsers.Count > 0)
            {
                SysWorkItemApproveGroup key = this.CreateWorkItemGroup();
                IOrgProxy orgProxy = OrgProxyFactory.GetProxy(base.Context);
                List<IUser> users = (from p in this.AI.UserDefinedApproveUsers select orgProxy.GetUserById(p.UserId.Value)).ToList<IUser>();
                this.RemoveRepeatedUsers(users);
                list.AddRange(users);
                dictionary.Add(key, users);
            }
            else
            {
                using (IEnumerator<SysActivityParticipant> enumerator = activity.ActivityParticipants.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Func<SysProcessParticipant, bool> predicate = null;
                        SysActivityParticipant activity_part = enumerator.Current;
                        SysWorkItemApproveGroup group = this.CreateWorkItemGroup(activity_part);
                        if (predicate == null)
                        {
                            predicate = p => p.ParticipantId == activity_part.ParticipantId.Value;
                        }
                        SysProcessParticipant part = process.ProcessParticipants.FirstOrDefault<SysProcessParticipant>(predicate);
                        if (part == null)
                        {
                            throw new ApplicationException("参与人为空");
                        }
                        int? wiOwnerId = null;
                        var users = ParticipantHelper.GetUsers(base.Context, part, base.PI, this.AI, wiOwnerId);
                        List<IUser> targetUsers = users;
                        this.AssertGroupHasUsers(activity_part, group, targetUsers);
                        this.RemoveRepeatedUsers(targetUsers);
                        list.AddRange(targetUsers);
                        dictionary.Add(group, targetUsers);
                    }
                }
            }
            if (list.Count == 0)
            {
                bool? isPassedWithNoParticipants = this.AI.Activity.IsPassedWithNoParticipants;
                if (!(isPassedWithNoParticipants.HasValue ? isPassedWithNoParticipants.GetValueOrDefault() : false))
                {
                    throw new ApplicationException("未计算出任何参与人");
                }
                SysActivityInstance aI = this.AI;
                aI.InstanceStatus = 10;
                aI.EndTime = new DateTime?(DateTime.Now);
                aI.ApproveResult = 1;
                aI.ExpressionValue = 1;
                aI.Remark = "无人审核,自动通过";
                base.PICacheFactory.UpdateActiviyInstance(this.AI);
                WorkflowMessage item = base.Engine.NewCompleteActivityMessage(this.AI);
                queue.Enqueue(item);
            }
            else
            {
                SysActivity activity3 = this.AI.Activity;
                if (!activity3.PassType.HasValue)
                {
                    activity3.PassType = 1;
                    if (!activity3.MinPassNum.HasValue)
                    {
                        activity3.MinPassNum = 1;
                    }
                }
                foreach (KeyValuePair<SysWorkItemApproveGroup, List<IUser>> pair in dictionary)
                {
                    SysWorkItemApproveGroup group3 = pair.Key;
                    foreach (IUser user in pair.Value)
                    {
                        this.CreateWorkItem(user, group3);
                    }
                }
            }
            if (this.AI.UserDefinedApproveUsers.Count > 0)
            {
                base.PICacheFactory.ClearApproveUsers(this.AI);
            }
        }

        private IApproveUser GetApproveUser(IUser user)
        {
            foreach (SysActivityInstanceApproveUsers users in this.AI.UserDefinedApproveUsers)
            {
                int? userId = users.UserId;
                int num = user.User_ID;
                if ((userId.GetValueOrDefault() == num) && userId.HasValue)
                {
                    return users;
                }
            }
            return null;
        }

        private void RemoveRepeatedUsers(List<IUser> users)
        {
            Dictionary<int, IUser> dictionary = new Dictionary<int, IUser>();
            for (int i = users.Count - 1; i >= 0; i--)
            {
                IUser user = users[i];
                if (dictionary.ContainsKey(user.User_ID))
                {
                    users.RemoveAt(i);
                }
                else
                {
                    dictionary.Add(user.User_ID, user);
                }
            }
        }

        internal SysActivityInstance AI
        {
            get
            {
                return this._ai;
            }
        }
    }
}

