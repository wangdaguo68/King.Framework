using System.Data.Common;

namespace King.Framework.WorkflowEngineCore
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Framework.Linq;
    using King.Framework.WorkflowEngineCore.InternalCore;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Transactions;

    public class EngineProxy : IDisposable
    {
        private readonly DataContext _context;
        private readonly bool _contextIsSelfCreated;
        private readonly InternalEngineProxy _engine;

        public EngineProxy(DataContext ctx = null)
        {
            if (ctx == null)
            {
                this._contextIsSelfCreated = true;
            }
            else
            {
                this._context = ctx;
                this._engine = new InternalEngineProxy(ctx);
                this._contextIsSelfCreated = false;
            }
        }

        public void CancelProcess(int processInstanceId)
        {
            DataContext context = this.GetContext();
            try
            {
                InternalEngineProxy engineProxy = this.GetEngineProxy(context);
                using (TransactionScope scope = new TransactionScope())
                {
                    engineProxy.CancelProcess(processInstanceId);
                    scope.Complete();
                }
            }
            finally
            {
                this.ReleaseContext(context);
            }
        }

        public void CompleteApproveWorkItem(int workItemId, ApproveResultEnum approveResult, string approveComment, bool addUser = false, int? addUserId = new int?())
        {
            DataContext context = this.GetContext();
            try
            {
                int? nullable = addUserId;
                this.GetEngineProxy(context).CompleteApproveWorkItemAsync(workItemId, approveResult, approveComment, null, addUser, new int?(nullable.HasValue ? nullable.GetValueOrDefault() : -1));
            }
            finally
            {
                this.ReleaseContext(context);
            }
        }

        public void CompleteApproveWorkItem(int workItemId, ApproveResultEnum approveResult, string approveComment, List<IApproveUser> nextApproveUserList, bool addUser = false, int? addUserId = new int?())
        {
            DataContext context = this.GetContext();
            try
            {
                this.GetEngineProxy(context).CompleteApproveWorkItemAsync(workItemId, approveResult, approveComment, nextApproveUserList, addUser, addUserId);
            }
            finally
            {
                this.ReleaseContext(context);
            }
        }

        public void CompleteApproveWorkItemSelf(int workItemId, ApproveResultEnum approveResult, string approveComment, bool addUser = false, int? addUserId = new int?())
        {
            DataContext context = this.GetContext();
            try
            {
                InternalEngineProxy engineProxy = this.GetEngineProxy(context);
                using (TransactionScope scope = new TransactionScope())
                {
                    int? nullable = addUserId;
                    engineProxy.CompleteApproveWorkItemSelf(workItemId, approveResult, approveComment, addUser, new int?(nullable.HasValue ? nullable.GetValueOrDefault() : -1));
                    scope.Complete();
                }
            }
            finally
            {
                this.ReleaseContext(context);
            }
        }

        public void CompleteWorkItem(int workItemId)
        {
            DataContext context = this.GetContext();
            try
            {
                this.GetEngineProxy(context).CompleteWorkItemAsync(workItemId);
            }
            finally
            {
                this.ReleaseContext(context);
            }
        }

        public void CompleteWorkItem(int workItemId, List<IApproveUser> nextApproveUserList)
        {
            DataContext context = this.GetContext();
            try
            {
                this.GetEngineProxy(context).CompleteWorkItemAsync(workItemId, nextApproveUserList);
            }
            finally
            {
                this.ReleaseContext(context);
            }
        }

        public void CompleteWorkItemSelf(int workItemId)
        {
            DataContext context = this.GetContext();
            try
            {
                InternalEngineProxy engineProxy = this.GetEngineProxy(context);
                using (TransactionScope scope = new TransactionScope())
                {
                    engineProxy.CompleteWorkItemSelf(workItemId);
                    scope.Complete();
                }
            }
            finally
            {
                this.ReleaseContext(context);
            }
        }

        public void Dispose()
        {
        }

        private DataContext GetContext()
        {
            if (this._contextIsSelfCreated)
            {
                return new BizDataContext(true);
            }
            return this._context;
        }

        private InternalEngineProxy GetEngineProxy(DataContext context)
        {
            if (this._contextIsSelfCreated)
            {
                return new InternalEngineProxy(context);
            }
            return this._engine;
        }

        public List<SysActivity> GetNextActivityList(int workItemId)
        {
            List<SysActivity> nextActivityList;
            DataContext context = this.GetContext();
            try
            {
                nextActivityList = this.GetEngineProxy(context).GetNextActivityList(workItemId);
            }
            finally
            {
                this.ReleaseContext(context);
            }
            return nextActivityList;
        }

        public List<SysActivity> GetNextActivityList(string processName)
        {
            List<SysActivity> nextActivityList;
            DataContext context = this.GetContext();
            try
            {
                nextActivityList = this.GetEngineProxy(context).GetNextActivityList(processName);
            }
            finally
            {
                this.ReleaseContext(context);
            }
            return nextActivityList;
        }

        public List<IUser> GetParticipantUsers(long activityId, int? processInstanceId = new int?(), int? activityInstanctId = new int?())
        {
            List<IUser> list2;
            DataContext context = this.GetContext();
            try
            {
                int? nullable = processInstanceId;
                int? nullable2 = activityInstanctId;
                List<IUser> users = this.GetEngineProxy(context).GetParticipantUsers(activityId, new int?(nullable.HasValue ? nullable.GetValueOrDefault() : -1), new int?(nullable2.HasValue ? nullable2.GetValueOrDefault() : -1));
                RemoveRepeatedUsers(users);
                list2 = users;
            }
            finally
            {
                this.ReleaseContext(context);
            }
            return list2;
        }

        public static IApproveUser New(int userId, bool IsMajor = false, int? flagInt = new int?(), string flagString = null)
        {
            return new MessageApproveUser { UserId = new int?(userId), IsMajor = new bool?(IsMajor), FlagInt = flagInt, FlagString = flagString };
        }

        public void RejectApproveWorkItem(int workItemId, string approveComment, long nextActivityId)
        {
            DataContext context = this.GetContext();
            try
            {
                InternalEngineProxy engineProxy = this.GetEngineProxy(context);
                using (TransactionScope scope = new TransactionScope())
                {
                    engineProxy.RejectApproveWorkItem(workItemId, approveComment, nextActivityId);
                    scope.Complete();
                }
            }
            finally
            {
                this.ReleaseContext(context);
            }
        }

        private void ReleaseContext(DataContext context)
        {
            if (this._contextIsSelfCreated)
            {
                context.Dispose();
            }
        }

        private static void RemoveRepeatedUsers(List<IUser> users)
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

        public void ResetMessageState(int messageId, int operationUserId)
        {
            int num;
            DataContext context = this.GetContext();
            try
            {
                string sql = string.Format("UPDATE SysWorkflowMessage SET STATE = {0} WHERE MESSAGEID = {1} AND STATE = {2}", Convert.ToInt32(SysWorkflowMessageStateEnum.Inited), messageId, Convert.ToInt32(SysWorkflowMessageStateEnum.Error));
                num = context.ExecuteNonQuery(sql, new DbParameter[0]);
                if (num > 0)
                {
                    AppLogHelper.Information("User({0})更新工作流消息({1})的状态,从Error->Inited", new object[] { operationUserId, messageId });
                }
            }
            finally
            {
                this.ReleaseContext(context);
            }
            if (num <= 0)
            {
                throw new ApplicationException("该消息不存在或已变更。");
            }
        }

        public int StartProcess(long processId, int startUserId, int relativeObjectId)
        {
            int num2;
            DataContext context = this.GetContext();
            try
            {
                num2 = this.GetEngineProxy(context).StartProcessAsync(processId, startUserId, relativeObjectId);
            }
            finally
            {
                this.ReleaseContext(context);
            }
            return num2;
        }

        public int StartProcess(string processName, int startUserId, int relativeObjectId)
        {
            int num2;
            DataContext context = this.GetContext();
            try
            {
                num2 = this.GetEngineProxy(context).StartProcessAsync(processName, startUserId, relativeObjectId);
            }
            finally
            {
                this.ReleaseContext(context);
            }
            return num2;
        }

        public int StartProcess(string processName, int startUserId, int relativeObjectId, List<IApproveUser> nextApproveUserList)
        {
            int num2;
            DataContext context = this.GetContext();
            try
            {
                num2 = this.GetEngineProxy(context).StartProcessAsync(processName, startUserId, relativeObjectId, nextApproveUserList);
            }
            finally
            {
                this.ReleaseContext(context);
            }
            return num2;
        }

        public int StartProcessAsync(string processName, int startUserId, int relativeObjectId, List<IApproveUser> nextApproveUserList)
        {
            int num2;
            DataContext context = this.GetContext();
            try
            {
                int num;
                InternalEngineProxy engineProxy = this.GetEngineProxy(context);
                using (TransactionScope scope = new TransactionScope())
                {
                    num = engineProxy.StartProcess(processName, startUserId, relativeObjectId, nextApproveUserList);
                    scope.Complete();
                }
                num2 = num;
            }
            finally
            {
                this.ReleaseContext(context);
            }
            return num2;
        }
    }
}

