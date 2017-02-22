using System.Data.Common;

namespace King.Framework.WorkflowEngineCore.WorkflowMessages
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using King.Framework.Linq;
    using King.Framework.WorkflowEngineCore.Cache;
    using King.Framework.WorkflowEngineCore.InternalCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;

    public static class InternalServiceProxy
    {
        private static bool CompleteMessage(SysWorkflowMessage msg, DataContext db)
        {
            if (msg.State != SysWorkflowMessageStateEnum.Running)
            {
                throw new Exception(string.Format("消息ID：{0}的状态不为Running", msg.MessageId));
            }
            string sql = string.Format("UPDATE SysWorkflowMessage SET STATE = {0} WHERE MESSAGEID = {1} AND STATE = {2}", db.AddPrefixToParameterName("NewState"), db.AddPrefixToParameterName("MessageId"), db.AddPrefixToParameterName("OldState"));
            int num = db.ExecuteNonQuery(sql, new DbParameter[] { db.CreateParameterWithPrefix("NewState", Convert.ToInt32(SysWorkflowMessageStateEnum.Completed), (int?) null), db.CreateParameterWithPrefix("MessageId", msg.MessageId, (int?) null), db.CreateParameterWithPrefix("OldState", Convert.ToInt32(SysWorkflowMessageStateEnum.Running), (int?) null) });
            if (num > 0)
            {
                Console.WriteLine("将消息状态改为Completed");
                msg.State = SysWorkflowMessageStateEnum.Completed;
            }
            return (num > 0);
        }

        public static List<SysWorkflowMessage> GetNextMessages(string connectionStringOrName)
        {
            if (string.IsNullOrWhiteSpace(connectionStringOrName))
            {
                connectionStringOrName = DataContext.BizConnectionStringDefault;
            }
            using (BizDataContext context = new BizDataContext(connectionStringOrName, true))
            {
                return (from p in context.Where<SysWorkflowMessage>(p => ((int) p.State) == 0)
                    orderby p.MessageId
                    select p).Take<SysWorkflowMessage>(10).ToList<SysWorkflowMessage>();
            }
        }

        public static void HandleMessage(SysWorkflowMessage msg, string connectionStringOrName)
        {
            if (string.IsNullOrWhiteSpace(connectionStringOrName))
            {
                connectionStringOrName = DataContext.BizConnectionStringDefault;
            }
            try
            {
                Console.WriteLine("开始处理消息ID：{0}，类型：{1}", msg.MessageId, msg.MessageType);
                using (TransactionScope scope = new TransactionScope())
                {
                    using (BizDataContext context = new BizDataContext(connectionStringOrName, true))
                    {
                        if (LockMessage(msg, context))
                        {
                            try
                            {
                                ProcessInstanceCacheFactory runtimeContext = new ProcessInstanceCacheFactory(context);
                                SysProcessInstance processInstance = null;
                                SysWorkItem item = null;
                                switch (msg.MessageType)
                                {
                                    case WorkflowMessageTypeEnum.StartingProcess:
                                        processInstance = runtimeContext.GetProcessInstanceCache(msg.ProcessInstanceId);
                                        break;

                                    case WorkflowMessageTypeEnum.ExecutingActivity:
                                        throw new Exception(string.Format("暂时不支持ExecutingActivity类型的消息", new object[0]));

                                    case WorkflowMessageTypeEnum.CompletingWorkItem:
                                    case WorkflowMessageTypeEnum.CompletingApproveWorkItem:
                                        processInstance = runtimeContext.GetProcessInstanceCacheByWorkItem(msg.WorkItemId, out item);
                                        break;

                                    default:
                                        throw new Exception(string.Format(string.Format("暂时不支持{0}类型的消息", msg.MessageType), new object[0]));
                                }
                                ProcessEngine engine = new ProcessEngine(runtimeContext, processInstance, item);
                                msg.MessageType.CreateHandler(engine).ProcessMessage(msg);
                                if (!CompleteMessage(msg, context))
                                {
                                    AppLogHelper.Information("完成工作流消息时更新行数为0: MessageId={0}", new object[] { msg.MessageId });
                                }
                                scope.Complete();
                                return;
                            }
                            catch (Exception exception)
                            {
                                AppLogHelper.Error(exception, "处理工作流消息失败1: MessageId={0}", new object[] { msg.MessageId });
                                throw new MessageHandleException(exception);
                            }
                        }
                        AppLogHelper.Information("锁定工作流消息时更新行数为0: MessageId={0}", new object[] { msg.MessageId });
                    }
                }
            }
            catch (MessageHandleException exception2)
            {
                ProcessError(msg, exception2.InnerException, connectionStringOrName);
            }
            catch (Exception exception3)
            {
                AppLogHelper.Error(exception3, "处理工作流消息失败2: MessageId={0}", new object[] { msg.MessageId });
                ProcessError(msg, exception3, connectionStringOrName);
            }
        }

        private static bool LockMessage(SysWorkflowMessage msg, DataContext db)
        {
            if (msg.State != SysWorkflowMessageStateEnum.Inited)
            {
                throw new Exception(string.Format("消息ID：{0}的状态不为Inited", msg.MessageId));
            }
            string sql = string.Format("UPDATE SysWorkflowMessage SET STATE = {0} WHERE MESSAGEID = {1} AND STATE = {2}", db.AddPrefixToParameterName("NewState"), db.AddPrefixToParameterName("MessageId"), db.AddPrefixToParameterName("OldState"));
            int num = db.ExecuteNonQuery(sql, new DbParameter[] { db.CreateParameterWithPrefix("NewState", Convert.ToInt32(SysWorkflowMessageStateEnum.Running), (int?) null), db.CreateParameterWithPrefix("MessageId", msg.MessageId, (int?) null), db.CreateParameterWithPrefix("OldState", Convert.ToInt32(SysWorkflowMessageStateEnum.Inited), (int?) null) });
            if (num > 0)
            {
                Console.WriteLine("将消息状态改为Running");
                msg.State = SysWorkflowMessageStateEnum.Running;
            }
            return (num > 0);
        }

        private static bool LockWorkItem(SysWorkItem wi, DataContext db)
        {
            if (wi.Status != 0)
            {
                throw new Exception(string.Format("工作项ID：{0}的状态不为Created", wi.WorkItemId));
            }
            wi.Status = 0x63;
            db.UpdatePartial<SysWorkItem>(wi, p => new { Status = p.Status });
            return true;
        }

        private static void ProcessError(SysWorkflowMessage msg, Exception ex, string connectionStringOrName)
        {
            if (string.IsNullOrWhiteSpace(connectionStringOrName))
            {
                connectionStringOrName = DataContext.BizConnectionStringDefault;
            }
            try
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    using (BizDataContext context = new BizDataContext(connectionStringOrName, true))
                    {
                        if (msg.State != SysWorkflowMessageStateEnum.Running)
                        {
                            throw new Exception(string.Format("消息ID:{0}的状态不为Running", msg.MessageId));
                        }
                        msg.State = SysWorkflowMessageStateEnum.Error;
                        msg.LastErrorMessage = ex.Message;
                        string sql = string.Format("UPDATE SysWorkflowMessage SET STATE = {0}, LastErrorMessage = {1} WHERE MessageId = {2} AND (STATE = {3} or STATE = {4}) ", new object[] { context.AddPrefixToParameterName("NewState"), context.AddPrefixToParameterName("LastErrorMessage"), context.AddPrefixToParameterName("MessageId"), context.AddPrefixToParameterName("OldState1"), context.AddPrefixToParameterName("OldState2") });
                        if (context.ExecuteNonQuery(sql, new DbParameter[] { context.CreateParameterWithPrefix("NewState", Convert.ToInt32(SysWorkflowMessageStateEnum.Error), typeof(int)), context.CreateParameterWithPrefix("LastErrorMessage", ex.Message, typeof(string)), context.CreateParameterWithPrefix("MessageId", msg.MessageId, typeof(int)), context.CreateParameterWithPrefix("OldState1", Convert.ToInt32(SysWorkflowMessageStateEnum.Running), typeof(int)), context.CreateParameterWithPrefix("OldState2", Convert.ToInt32(SysWorkflowMessageStateEnum.Inited), typeof(int)) }) <= 0)
                        {
                            AppLogHelper.Information("将工作流消息更改为错误状态时更新行数为0: MessageId={0}", new object[] { msg.MessageId });
                        }
                    }
                    scope.Complete();
                }
                Console.WriteLine(ex.Message);
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(exception, "将工作流消息更改为错误状态时失败: MessageId={0}", new object[] { msg.MessageId });
                Console.WriteLine(exception.Message);
            }
        }

        private class MessageHandleException : ApplicationException
        {
            public MessageHandleException(Exception innerException) : base("工作流消息处理错误", innerException)
            {
            }
        }
    }
}

