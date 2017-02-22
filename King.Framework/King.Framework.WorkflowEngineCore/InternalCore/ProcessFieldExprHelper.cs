namespace King.Framework.WorkflowEngineCore.InternalCore
{
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.Cache;
    using System;

    internal class ProcessFieldExprHelper : BaseExprHelper
    {
        public ProcessFieldExprHelper() : base(DExpressionType.ProcessField)
        {
        }

        public override object GetValue(SysExpression expr, EntityCache cache, ExpressionCache expr_cache, SysProcessInstance pi, SysActivityInstance ai)
        {
            SysEntity processEntity = pi.Process.ProcessEntity;
            int objectId = pi.ObjectId;
            object obj2 = cache.GetObject(processEntity, objectId)[expr.Field.FieldName];
            expr_cache.Add(expr, obj2);
            return obj2;
        }
    }
}

