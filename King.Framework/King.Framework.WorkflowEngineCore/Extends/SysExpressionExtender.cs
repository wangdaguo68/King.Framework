namespace King.Framework.WorkflowEngineCore.Extends
{
    using King.Framework.EntityLibrary;
    using System;
    using System.Runtime.CompilerServices;

    internal static class SysExpressionExtender
    {
        public static bool IsBinaryExpression(this SysExpression expr)
        {
            if (!expr.ExpressionType.HasValue)
            {
                throw new ApplicationException("未设置表达式类型");
            }
            if (expr.ExpressionType.Value == 6)
            {
                throw new ApplicationException("不支持ProcessVariable");
            }
            return (expr.ExpressionType == 5);
        }

        public static bool IsConst(this SysExpression expr)
        {
            return (expr.ExpressionType.HasValue && (expr.ExpressionType.Value == 0));
        }

        public static bool IsProcessField(this SysExpression expr)
        {
            return (expr.ExpressionType.HasValue && (expr.ExpressionType.Value == 1));
        }
    }
}

