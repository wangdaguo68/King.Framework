namespace King.Framework.WorkflowEngineCore.InternalCore
{
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.Cache;
    using System;

    internal class ActivityFieldExprHelper : BaseExprHelper
    {
        public ActivityFieldExprHelper() : base(DExpressionType.ActivityField)
        {
        }

        public override object GetValue(SysExpression expr, EntityCache cache, ExpressionCache expr_cache, SysProcessInstance pi, SysActivityInstance ai)
        {
            SysEntity activityEntity = pi.Process.ActivityEntity;
            int activityInstanceId = ai.ActivityInstanceId;
            object obj2 = cache.GetObject(activityEntity, activityInstanceId)[expr.Field.FieldName];
            expr_cache.Add(expr, obj2);
            return obj2;
        }
    }
}

