namespace King.Framework.WorkflowEngineCore.InternalCore
{
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.Cache;
    using System;

    internal class ProcessRelationFieldExprHelper : BaseExprHelper
    {
        public ProcessRelationFieldExprHelper() : base(DExpressionType.ProcessRelationField)
        {
        }

        public override object GetValue(SysExpression expr, EntityCache cache, ExpressionCache expr_cache, SysProcessInstance pi, SysActivityInstance ai)
        {
            SysProcess process = pi.Process;
            if (process.ProcessEntity == null)
            {
                throw new ApplicationException("流程实体为空");
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
            if (!process.EntityId.HasValue)
            {
                throw new ApplicationException("流程的实体为空");
            }
            if (expr.Relation.ChildEntityId.Value != process.EntityId.Value)
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
            SysEntity processEntity = pi.Process.ProcessEntity;
            int objectId = pi.ObjectId;
            EntityData data = cache.GetObject(processEntity, objectId);
            SysEntity parentEntity = relation.ParentEntity;
            if (data.ContainsKey(relation.ChildField.FieldName))
            {
                int num2 = Convert.ToInt32(data[relation.ChildField.FieldName]);
                if (num2 != 0)
                {
                    object obj2 = cache.GetObject(parentEntity, num2)[expr.Field.FieldName];
                    expr_cache.Add(expr, obj2);
                    return obj2;
                }
            }
            throw new ApplicationException(string.Format("计算表达式时需要字段{0}，但其未赋值或为空", relation.ChildField.FieldName));
        }
    }
}

