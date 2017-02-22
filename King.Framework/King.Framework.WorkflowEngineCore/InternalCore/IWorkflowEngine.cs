using King.Framework.Interfaces;

namespace King.Framework.WorkflowEngineCore.InternalCore
{
    using King.Framework.EntityLibrary;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    internal interface IWorkflowEngine
    {
        void CancelProcess(int processInstanceId);
        void CompleteApproveWorkItem(int workItemId, ApproveResultEnum approveResult, string approveComment, bool addUser = false, int? addUserId = new int?());
        void CompleteApproveWorkItem(int workItemId, ApproveResultEnum approveResult, string approveComment, List<IApproveUser> nextApproveUserList, bool addUser = false, int? addUserId = new int?());
        void CompleteApproveWorkItemSelf(int workItemId, ApproveResultEnum approveResult, string approveComment, bool addUser = false, int? addUserId = new int?());
        void CompleteWorkItem(int workItemId);
        void CompleteWorkItem(int workItemId, List<IApproveUser> nextApproveUserList);
        void CompleteWorkItemSelf(int workItemId);
        List<SysActivity> GetNextActivityList(int workItemId);
        List<SysActivity> GetNextActivityList(string processName);
        List<IUser> GetParticipantUsers(long activityId, int? processInstanceId = new int?(), int? activityInstanctId = new int?());
        int StartProcess(long processId, int startUserId, int relativeObjectId);
        int StartProcess(string processName, int startUserId, int relativeObjectId);
        int StartProcess(string processName, int startUserId, int relativeObjectId, List<IApproveUser> nextApproveUserList);
    }
}

