namespace King.Framework.WorkflowEngineCore.InternalCore
{
    using King.Framework.EntityLibrary;
    using System;
    using System.Collections.Generic;

    internal class ExpressionCache
    {
        private Dictionary<long, KeyValuePair<SysExpression, object>> _dict = new Dictionary<long, KeyValuePair<SysExpression, object>>(20);

        public void Add(SysExpression expr, object value)
        {
            this._dict.Add(expr.ExpressionId, new KeyValuePair<SysExpression, object>(expr, value));
        }

        public bool ContainsKey(SysExpression expr)
        {
            return this._dict.ContainsKey(expr.ExpressionId);
        }

        public bool ContainsKey(long exprId)
        {
            return this._dict.ContainsKey(exprId);
        }

        public SysExpression GetExpr(long exprId)
        {
            KeyValuePair<SysExpression, object> pair = this._dict[exprId];
            return pair.Key;
        }

        public object GetValue(SysExpression expr)
        {
            KeyValuePair<SysExpression, object> pair = this._dict[expr.ExpressionId];
            return pair.Value;
        }

        public object GetValue(long exprId)
        {
            KeyValuePair<SysExpression, object> pair = this._dict[exprId];
            return pair.Value;
        }
    }
}

