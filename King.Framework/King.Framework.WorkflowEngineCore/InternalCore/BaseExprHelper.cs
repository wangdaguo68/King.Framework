namespace King.Framework.WorkflowEngineCore.InternalCore
{
    using King.Framework.EntityLibrary;
    using King.Framework.WorkflowEngineCore.Cache;
    using System;

    internal abstract class BaseExprHelper
    {
        private readonly DExpressionType _exprType;

        public BaseExprHelper(DExpressionType exprType)
        {
            this._exprType = exprType;
        }

        public abstract object GetValue(SysExpression expr, EntityCache cache, ExpressionCache expr_cache, SysProcessInstance pi, SysActivityInstance ai);

        public DExpressionType ExpressionType
        {
            get
            {
                return this._exprType;
            }
        }
    }
}

