namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Linq.Expressions;

    public class IFCommand : CommandExpression
    {
        private Expression check;
        private Expression ifFalse;
        private Expression ifTrue;

        public IFCommand(Expression check, Expression ifTrue, Expression ifFalse) : base(DbExpressionType.If, ifTrue.Type)
        {
            this.check = check;
            this.ifTrue = ifTrue;
            this.ifFalse = ifFalse;
        }

        public Expression Check
        {
            get
            {
                return this.check;
            }
        }

        public Expression IfFalse
        {
            get
            {
                return this.ifFalse;
            }
        }

        public Expression IfTrue
        {
            get
            {
                return this.ifTrue;
            }
        }
    }
}
