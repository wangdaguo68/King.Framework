namespace King.Framework.Manager
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.Cache;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Timers;

    public class ScheduleTimerManager
    {
        private static ScheduleTimerManager _manager = new ScheduleTimerManager();
        private static readonly int mainInterval = 0x6ddd00;
        private Timer mainTimer;
        private static readonly int secondInterval = 0xea60;
        private Timer secondTimer;

        private ScheduleTimerManager()
        {
            this.Init();
        }
        public static FieldInfo fieldof<T>(Expression<Func<T>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            return (FieldInfo)body.Member;
        }
        public static MethodInfo methodof<T>(Expression<Func<T>> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
        private void CheckTimeOut()
        {
            using (BizDataContext context = new BizDataContext(true))
            {
                ParameterExpression expression;
                ProcessInstanceCacheFactory factory = new ProcessInstanceCacheFactory(context);
                List<int> list = new List<int>();
                List<SysWorkItem> list2 = context.Where<SysWorkItem>(
                    Expression.Lambda<Func<SysWorkItem, bool>>(
                        Expression.AndAlso(Expression.AndAlso(
                            Expression.NotEqual(Expression.Property(
                                expression = Expression.Parameter(typeof(SysWorkItem), "p"),
                                (typeof(SysWorkItem).GetProperty("DeadLine"))),
                                Expression.Convert(Expression.Constant(null, typeof(DateTime?)),
                                typeof(DateTime?)), false, (typeof(DateTime).GetMethod("Now"))), 
                            Expression.Equal(Expression.Property(expression, typeof(SysWorkItem).GetProperty("")), 
                            Expression.Convert(Expression.Constant(0, typeof(int)), typeof(int?)))), 
                            Expression.GreaterThan(Expression.Convert(Expression.Property(null, typeof(DateTime).GetMethod("Now")), 
                            typeof(DateTime?)), Expression.Property(expression, typeof(SysWorkItem).GetProperty("DeadLine")),
                            false, typeof(SysWorkItem).GetMethod("Now"))), new ParameterExpression[] { expression })).ToList<SysWorkItem>();
                if (list2.Count > 0)
                {
                    foreach (SysWorkItem item in list2)
                    {
                        SysWorkItem item2;
                        SysProcessInstance processInstanceCacheByWorkItem = factory.GetProcessInstanceCacheByWorkItem(item.WorkItemId, out item2);
                        if (!list.Contains(processInstanceCacheByWorkItem.ProcessInstanceId))
                        {
                            list.Add(processInstanceCacheByWorkItem.ProcessInstanceId);
                        }
                        factory.Manager.ExecutePython(processInstanceCacheByWorkItem, item2.ActivityInstance.Activity.TimeOutScript);
                        item2.Status = 3;
                        item2.EndTime = new DateTime?(DateTime.Now);
                        factory.UpdateWorkItemBaseState(item2);
                        factory.Context.Update(item2);
                    }
                }
                foreach (int num in list)
                {
                    factory.ClearCache(num);
                }
            }
        }

        public static ScheduleTimerManager CreateInstance()
        {
            if (_manager == null)
            {
                throw new ApplicationException();
            }
            return _manager;
        }

        ~ScheduleTimerManager()
        {
            this.mainTimer.Dispose();
            this.secondTimer.Dispose();
        }

        private void Init()
        {
            this.mainTimer = new Timer((double) mainInterval);
            this.mainTimer.AutoReset = true;
            this.mainTimer.Elapsed += new ElapsedEventHandler(this.mainTimer_Elapsed);
            this.secondTimer = new Timer((double) secondInterval);
            this.secondTimer.AutoReset = true;
            this.secondTimer.Elapsed += new ElapsedEventHandler(this.secondTimer_Elapsed);
        }

        private void mainTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (!this.secondTimer.Enabled)
                {
                    this.secondTimer.Start();
                }
                this.CheckTimeOut();
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(exception);
                this.Init();
            }
        }

        private void secondTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (!this.mainTimer.Enabled)
                {
                    this.mainTimer.Start();
                }
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(exception);
                this.Init();
            }
        }

        public void Start()
        {
            this.mainTimer.Start();
            this.secondTimer.Start();
        }

        public void Stop()
        {
            this.mainTimer.Stop();
            this.secondTimer.Stop();
        }
    }
}
