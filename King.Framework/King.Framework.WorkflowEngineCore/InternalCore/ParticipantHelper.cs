using King.Framework.Interfaces;

namespace King.Framework.WorkflowEngineCore.InternalCore
{
    using King.Framework.EntityLibrary;
    using King.Framework.Linq;
    using King.Framework.WorkflowEngineCore.ParticipantFunctions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    internal static class ParticipantHelper
    {
        public static List<IUser> GetUsers(DataContext ctx, SysProcessParticipant part, SysProcessInstance pi, SysActivityInstance ai, int? wiOwnerId = new int?())
        {
            List<IUser> list = new List<IUser>();
            int valueOrDefault = part.FunctionType.GetValueOrDefault();
            if (part.FunctionType.HasValue)
            {
                switch (valueOrDefault)
                {
                    case 0:
                    {
                        StartUserFunc func = new StartUserFunc(ctx);
                        return func.GetUsers(part, pi, ai).ToList<IUser>();
                    }
                    case 1:
                    {
                        SpecUserFunc func7 = new SpecUserFunc(ctx);
                        return func7.GetUsers(part, pi, ai).ToList<IUser>();
                    }
                    case 2:
                    {
                        StartDepartmentFunc func2 = new StartDepartmentFunc(ctx);
                        return func2.GetUsers(part, pi, ai).ToList<IUser>();
                    }
                    case 3:
                    {
                        SpecDepartmentFunc func3 = new SpecDepartmentFunc(ctx);
                        return func3.GetUsers(part, pi, ai).ToList<IUser>();
                    }
                    case 4:
                    {
                        DepartmentManagerFunc func4 = new DepartmentManagerFunc(ctx);
                        return func4.GetUsers(part, pi, ai).ToList<IUser>();
                    }
                    case 5:
                    {
                        ParentDepartmentFunc func5 = new ParentDepartmentFunc(ctx);
                        return func5.GetUsers(part, pi, ai).ToList<IUser>();
                    }
                    case 6:
                    {
                        SpecRoleFunc func8 = new SpecRoleFunc(ctx);
                        return func8.GetUsers(part, pi, ai).ToList<IUser>();
                    }
                    case 7:
                    {
                        DepartmentRoleFunc func6 = new DepartmentRoleFunc(ctx);
                        return func6.GetUsers(part, pi, ai).ToList<IUser>();
                    }
                    case 8:
                    {
                        PythonFunc func9 = new PythonFunc(ctx);
                        return func9.GetUsers(part, pi, ai).ToList<IUser>();
                    }
                    case 9:
                    {
                        DirectManagerFunc func10 = new DirectManagerFunc(ctx);
                        return func10.GetUsers(part, pi, ai).ToList<IUser>();
                    }
                    case 10:
                    {
                        SpecialLevelParentDeptFunc func11 = new SpecialLevelParentDeptFunc(ctx);
                        return func11.GetUsers(part, pi, ai).ToList<IUser>();
                    }
                    case 11:
                        try
                        {
                            CustomPluginFunc func16 = new CustomPluginFunc(ctx);
                            return func16.GetUsers(part, pi, ai).ToList<IUser>();
                        }
                        catch (Exception exception)
                        {
                            throw new ApplicationException("自定义参与人插件错误:" + exception.Message, exception);
                        }
                        break;

                    case 100:
                    {
                        PrevActivityParticipantFunc func14 = new PrevActivityParticipantFunc(ctx);
                        return func14.GetUsers(part, pi, ai).ToList<IUser>();
                    }
                    case 0x65:
                    {
                        PrevAllParticipantFunc func15 = new PrevAllParticipantFunc(ctx);
                        return func15.GetUsers(part, pi, ai).ToList<IUser>();
                    }
                    case 0x66:
                    {
                        CurrentActivityParticipantFunc func12 = new CurrentActivityParticipantFunc(ctx);
                        return func12.GetUsers(part, pi, ai).ToList<IUser>();
                    }
                    case 0x67:
                    {
                        CurrentWorkItemOwnerFunc func13 = new CurrentWorkItemOwnerFunc(ctx, wiOwnerId);
                        return func13.GetUsers(part, pi, ai).ToList<IUser>();
                    }
                }
            }
            throw new ApplicationException("在ParticipantHelper的GetUsers中参数类型有误！");
        }
    }
}

