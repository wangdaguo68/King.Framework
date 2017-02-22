namespace King.Framework.WorkflowEngineCore.MessageHandler
{
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.Cache;
    using King.Framework.WorkflowEngineCore.InternalCore;
    using King.Framework.WorkflowEngineCore.Remind;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class CompleteActivityMessage : WorkflowMessage
    {
        private SysActivityInstance _ai;

        public CompleteActivityMessage(ProcessEngine engine, SysActivityInstance ai) : base(engine)
        {
            if (ai == null)
            {
                throw new ArgumentNullException("ai");
            }
            this._ai = ai;
        }

        private bool CalExpressionValue(SysTransition tran)
        {
            SysExpression expr;
            bool flag = true;
            if ((tran.Expressions != null) && (tran.Expressions.Count != 0))
            {
                if (!tran.ExpressionId.HasValue)
                {
                    return flag;
                }
                expr = tran.Expressions.FirstOrDefault<SysExpression>(e => e.ExpressionId == tran.ExpressionId.Value);
                if (expr == null)
                {
                    throw new ApplicationException("指定的入口表达式不在连接线的表达式中");
                }
                Queue<SysExpression> calOrder = ExpressionHelper.GetCalOrder(tran.Expressions.ToList<SysExpression>());
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
                            throw new ApplicationException("表达式的值返回null");
                        }
                        if (obj2.GetType() != typeof(bool))
                        {
                            throw new ApplicationException("入口条件表达式的值不是布尔类型");
                        }
                        return (bool) obj2;
                    }
                }
            }
            return flag;
        }

        private void CompleteApproveActivity(Queue<WorkflowMessage> queue, SysProcess process, SysActivity activity)
        {
            new ActivityRemindHandler(base.PICacheFactory, base.PI, this.AI, ActivityRemindUseTimeType.ActivityEnd, null, this.AI.ApproveResult).Execute();
            List<SysTransition> source = process.Transitions.Where<SysTransition>(delegate (SysTransition t) {
                long? preActivityId = t.PreActivityId;
                long activityId = activity.ActivityId;
                return ((preActivityId.GetValueOrDefault() == activityId) && preActivityId.HasValue);
            }).ToList<SysTransition>();
            if (source.Count < 1)
            {
                throw new ApplicationException("没有后续的转换条件");
            }
            SysTransition tran = null;
            if (this.AI.ExpressionValue.HasValue)
            {
                int direction = Convert.ToBoolean(this.AI.ExpressionValue.Value) ? 1 : 2;
                tran = source.FirstOrDefault<SysTransition>(delegate (SysTransition p) {
                    int? nullable1 = p.Direction;
                    int num = direction;
                    return (nullable1.GetValueOrDefault() == num) && nullable1.HasValue;
                });
            }
            if (tran == null)
            {
                tran = this.GetNextTransition(source);
            }
            if (tran == null)
            {
                throw new ApplicationException("所有后续连接线均不满足条件，流程无法继续");
            }
            SysActivityInstance ai = this.CreatePostAcitivtyInstance(base.PI, tran);
            WorkflowMessage item = base.Engine.NewActivityInstanceCreatedMessage(ai);
            queue.Enqueue(item);
        }

        private void CompleteDecisionActivity(Queue<WorkflowMessage> queue, SysProcess process, SysActivity activity)
        {
            List<SysTransition> source = process.Transitions.Where<SysTransition>(delegate (SysTransition t) {
                long? preActivityId = t.PreActivityId;
                long activityId = activity.ActivityId;
                return ((preActivityId.GetValueOrDefault() == activityId) && preActivityId.HasValue);
            }).ToList<SysTransition>();
            if (source.Count < 1)
            {
                throw new ApplicationException("没有后续的转换条件");
            }
            if (!this.AI.ExpressionValue.HasValue)
            {
                throw new ApplicationException("未计算决策活动的值");
            }
            int direction = Convert.ToBoolean(this.AI.ExpressionValue.Value) ? 1 : 2;
            SysTransition tran = source.FirstOrDefault<SysTransition>(delegate (SysTransition p) {
                int? nullable1 = p.Direction;
                int num = direction;
                return (nullable1.GetValueOrDefault() == num) && nullable1.HasValue;
            });
            if (tran == null)
            {
                throw new ApplicationException(string.Format("没有方向为{0}的后续活动", direction));
            }
            if (!tran.PostActivityId.HasValue)
            {
                throw new ApplicationException("后续活动为空");
            }
            SysActivityInstance ai = this.CreatePostAcitivtyInstance(base.PI, tran);
            WorkflowMessage item = base.Engine.NewActivityInstanceCreatedMessage(ai);
            queue.Enqueue(item);
        }

        private void CompleteEndActivity(Queue<WorkflowMessage> queue, SysProcess process, SysActivity activity)
        {
            base.PI.InstanceStatus = 10;
            base.PI.EndTime = new DateTime?(DateTime.Now);
            base.PI.ApproveResult = activity.ApproveResult;
            base.PICacheFactory.UpdateProcessInstance(base.PI);
            if (process.ProcessCategory == 2)
            {
                if (activity.ApproveResult == 1)
                {
                    base.PI.FormInstance.State = 2;
                }
                else if (activity.ApproveResult == 0)
                {
                    base.PI.FormInstance.State = 3;
                }
                base.PICacheFactory.UpdateFormInstance(base.PI.FormInstance);
            }
            new ProcessRemindHandler(base.PICacheFactory, base.PI, this.AI, ProcessRemindUseTimeType.ProcessFinished).Execute();
        }

        private void CompleteNormalActivity(Queue<WorkflowMessage> queue, SysProcess process, SysActivity activity)
        {
            if (activity.ExecType == 0)
            {
                new ActivityRemindHandler(base.PICacheFactory, base.PI, this.AI, ActivityRemindUseTimeType.ActivityEnd, null, null).Execute();
            }
            List<SysTransition> trans = process.Transitions.Where<SysTransition>(delegate (SysTransition t) {
                long? preActivityId = t.PreActivityId;
                long activityId = activity.ActivityId;
                return ((preActivityId.GetValueOrDefault() == activityId) && preActivityId.HasValue);
            }).ToList<SysTransition>();
            if (trans.Count <= 0)
            {
                throw new ApplicationException("没有后续的转换条件");
            }
            SysTransition nextTransition = this.GetNextTransition(trans);
            if (nextTransition == null)
            {
                throw new ApplicationException("所有后续连接线均不满足条件，流程无法继续");
            }
            SysActivityInstance ai = this.CreatePostAcitivtyInstance(base.PI, nextTransition);
            WorkflowMessage item = base.Engine.NewActivityInstanceCreatedMessage(ai);
            queue.Enqueue(item);
        }

        private void CompleteStartActivity(Queue<WorkflowMessage> queue, SysProcess process, SysActivity activity)
        {
            this.CompleteNormalActivity(queue, process, activity);
        }

        private SysActivityInstance CreateActivityInstance(SysProcessInstance pi, SysActivity act)
        {
            SysActivityInstance ai = new SysActivityInstance {
                ActivityId = new long?(act.ActivityId),
                ActivityInstanceId = base.Manager.GetNextIdentity(),
                EndTime = null,
                ExpressionValue = null,
                InstanceStatus = 0,
                ProcessInstanceId = new int?(pi.ProcessInstanceId),
                StartTime = new DateTime?(DateTime.Now),
                WwfActivityInstanceId = null
            };
            base.PICacheFactory.AddActivityInstance(pi, ai);
            return ai;
        }

        private SysActivityInstance CreatePostAcitivtyInstance(SysProcessInstance pi, SysTransition tran)
        {
            if (!tran.PostActivityId.HasValue)
            {
                throw new ApplicationException("后续活动为空");
            }
            SysActivity postActivity = pi.Process.Activities.FirstOrDefault<SysActivity>(p => p.ActivityId == tran.PostActivityId.Value);
            SysActivityInstance postAi = this.CreatePostActivityInstance(pi, postActivity);
            this.CreateTransitionInstance(postAi, tran);
            return postAi;
        }

        protected SysActivityInstance CreatePostActivityInstance(SysProcessInstance pi, SysActivity postActivity)
        {
            if (postActivity == null)
            {
                throw new ApplicationException("无此活动");
            }
            SysActivityInstance postAi = this.CreateActivityInstance(pi, postActivity);
            this.RedirectNextApproveUsers(this.AI, postAi);
            return postAi;
        }

        private void CreateTransitionInstance(SysActivityInstance postAi, SysTransition tran)
        {
            SysTransitionInstance ti = new SysTransitionInstance {
                ProcessInstanceId = new int?(base.PI.ProcessInstanceId),
                PreActivityInstanceId = new int?(this.AI.ActivityInstanceId),
                PostActivityInstanceId = new int?(postAi.ActivityInstanceId),
                TransitionId = new long?(tran.TransitionId),
                TransitionInstanceId = base.Manager.GetNextIdentity()
            };
            base.PICacheFactory.AddTransitionInstance(ti, this.AI, postAi);
        }

        public override void Execute(Queue<WorkflowMessage> queue)
        {
            this.JustCompleteCurrentAI();
            SysActivity activity = this.AI.Activity;
            SysProcess process = activity.Process;
            if (activity.ActivityType == 3)
            {
                this.CompleteNormalActivity(queue, process, activity);
            }
            else if (activity.ActivityType == 1)
            {
                this.CompleteStartActivity(queue, process, activity);
            }
            else if (activity.ActivityType == 2)
            {
                this.CompleteDecisionActivity(queue, process, activity);
            }
            else if (activity.ActivityType == 4)
            {
                this.CompleteEndActivity(queue, process, activity);
            }
            else
            {
                if (activity.ActivityType != 6)
                {
                    throw new ApplicationException("未知的活动类型");
                }
                this.CompleteApproveActivity(queue, process, activity);
            }
        }

        private SysTransition GetNextTransition(List<SysTransition> trans)
        {
            foreach (KeyValuePair<SysTransition, bool> pair in trans.ToDictionary<SysTransition, SysTransition, bool>(p => p, p => this.CalExpressionValue(p)))
            {
                if (pair.Value)
                {
                    return pair.Key;
                }
            }
            return null;
        }

        protected void JustCompleteCurrentAI()
        {
            this.AI.InstanceStatus = 10;
            this.AI.EndTime = new DateTime?(DateTime.Now);
            base.PICacheFactory.UpdateActiviyInstance(this.AI);
        }

        private void RedirectNextApproveUsers(SysActivityInstance preAi, SysActivityInstance postAi)
        {
            base.PICacheFactory.RedirectNextApproveUsers(preAi, postAi);
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

