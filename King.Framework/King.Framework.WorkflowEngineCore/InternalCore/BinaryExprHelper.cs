namespace King.Framework.WorkflowEngineCore.InternalCore
{
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.Cache;
    using System;
    using System.Linq.Expressions;

    internal class BinaryExprHelper : BaseExprHelper
    {
        public BinaryExprHelper() : base(DExpressionType.BinaryExpression)
        {
        }

        public override object GetValue(SysExpression expr, EntityCache cache, ExpressionCache expr_cache, SysProcessInstance pi, SysActivityInstance ai)
        {
            if (!expr.OperationType.HasValue)
            {
                throw new ApplicationException("操作类型为空");
            }
            if (!expr.LeftId.HasValue)
            {
                throw new ApplicationException("左变量为空");
            }
            if (!expr.RightId.HasValue)
            {
                throw new ApplicationException("右变量为空");
            }
            object obj2 = expr_cache.GetValue(expr.LeftId.Value);
            object obj3 = expr_cache.GetValue(expr.RightId.Value);
            ConstantExpression left = Expression.Constant(obj2);
            ConstantExpression right = Expression.Constant(obj3);
            object obj4 = Expression.Lambda(Expression.MakeBinary((ExpressionType)expr.OperationType.Value, left, right), new ParameterExpression[0]).Compile().DynamicInvoke(new object[0]);
            expr_cache.Add(expr, obj4);
            return obj4;
        }
    }
}

