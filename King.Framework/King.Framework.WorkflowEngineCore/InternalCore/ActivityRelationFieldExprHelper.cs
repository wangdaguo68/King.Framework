namespace King.Framework.WorkflowEngineCore.InternalCore
{
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.Cache;
    using System;

    internal class ActivityRelationFieldExprHelper : BaseExprHelper
    {
        public ActivityRelationFieldExprHelper() : base(DExpressionType.ActivityRelationField)
        {
        }

        public override object GetValue(SysExpression expr, EntityCache cache, ExpressionCache expr_cache, SysProcessInstance pi, SysActivityInstance ai)
        {
            SysProcess process = pi.Process;
            SysEntity activityEntity = process.ActivityEntity;
            if (activityEntity == null)
            {
                throw new ApplicationException("活动实体为空");
            }
            SysOneMoreRelation relation = expr.Relation;
            if (relation == null)
            {
                throw new ApplicationException("表达式的关系为空");
            }
            if (!relation.ChildEntityId.HasValue)
            {
                throw new ApplicationException("表达式的子实体为空");
            }
            if (!process.ActivityEntityId.HasValue)
            {
                throw new ApplicationException("活动的实体为空");
            }
            if (expr.Relation.ChildEntityId.Value != process.ActivityEntityId.Value)
            {
                throw new ApplicationException("不正确的关系");
            }
            if (!relation.ParentEntityId.HasValue)
            {
                throw new ApplicationException("关系的父实体为空");
            }
            if (!relation.ChildFieldId.HasValue)
            {
                throw new ApplicationException("关系的ChildFieldId为空");
            }
            long local1 = relation.ChildFieldId.Value;
            if (relation.ChildField == null)
            {
                throw new ApplicationException("关系的对应字段为空");
            }
            int activityInstanceId = ai.ActivityInstanceId;
            EntityData data = cache.GetObject(activityEntity, activityInstanceId);
            SysEntity parentEntity = relation.ParentEntity;
            int num2 = Convert.ToInt32(data[relation.ChildField.FieldName]);
            object obj2 = cache.GetObject(parentEntity, num2)[expr.Field.FieldName];
            expr_cache.Add(expr, obj2);
            return obj2;
        }
    }
}

