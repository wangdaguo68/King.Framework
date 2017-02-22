namespace King.Framework.WorkflowEngineCore.InternalCore
{
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.Extends;
    using System;
    using System.Collections.Generic;

    internal static class ExpressionHelper
    {
        private static readonly Dictionary<int, BaseExprHelper> _helpers = new Dictionary<int, BaseExprHelper>();

        static ExpressionHelper()
        {
            Add(new ConstExprHelper());
            Add(new ProcessFieldExprHelper());
            Add(new ProcessRelationFieldExprHelper());
            Add(new ActivityFieldExprHelper());
            Add(new ActivityRelationFieldExprHelper());
            Add(new BinaryExprHelper());
        }

        private static void Add(BaseExprHelper helper)
        {
            _helpers.Add((int) helper.ExpressionType, helper);
        }

        internal static Queue<SysExpression> GetCalOrder(List<SysExpression> exprs)
        {
            Queue<SysExpression> queue = new Queue<SysExpression>();
            Dictionary<long, SysExpression> dictionary = new Dictionary<long, SysExpression>();
            int num = 1;
            List<SysExpression> list = new List<SysExpression>(exprs);
            while (num > 0)
            {
                num = 0;
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    SysExpression expr = list[i];
                    if (expr.IsBinaryExpression())
                    {
                        if (dictionary.ContainsKey(expr.LeftId.Value) && dictionary.ContainsKey(expr.RightId.Value))
                        {
                            queue.Enqueue(expr);
                            dictionary.Add(expr.ExpressionId, expr);
                            num++;
                            list.RemoveAt(i);
                        }
                    }
                    else
                    {
                        queue.Enqueue(expr);
                        dictionary.Add(expr.ExpressionId, expr);
                        num++;
                        list.RemoveAt(i);
                    }
                }
            }
            return queue;
        }

        public static BaseExprHelper GetHelper(DExpressionType exprType)
        {
            return _helpers[(int) exprType];
        }

        public static BaseExprHelper GetHelper(SysExpression expr)
        {
            if (!expr.ExpressionType.HasValue)
            {
                throw new ApplicationException("表达式类型为空");
            }
            return _helpers[expr.ExpressionType.Value];
        }
    }
}

