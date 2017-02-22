namespace King.Framework.WorkflowEngineCore.InternalCore
{
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.Cache;
    using System;

    internal class ConstExprHelper : BaseExprHelper
    {
        public ConstExprHelper() : base(DExpressionType.Constant)
        {
        }

        public override object GetValue(SysExpression expr, EntityCache cache, ExpressionCache expr_cache, SysProcessInstance pi, SysActivityInstance ai)
        {
            if (!expr.DataType.HasValue)
            {
                throw new ApplicationException("未指定常量的数据类型");
            }
            if (string.IsNullOrWhiteSpace(expr.ConstValue))
            {
                throw new ApplicationException("未指定常量的值");
            }
            object obj2 = DataTypeHelper.GetHelper((DataTypeEnum)expr.DataType.Value).ConvertFromString(expr.ConstValue);
            expr_cache.Add(expr, obj2);
            return obj2;
        }
    }
}

