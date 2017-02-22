namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Linq.Expressions;

    public class JoinExpression : DbExpression
    {
        private Expression condition;
        private JoinType joinType;
        private Expression left;
        private Expression right;

        public JoinExpression(JoinType joinType, Expression left, Expression right, Expression condition) : base(DbExpressionType.Join, typeof(void))
        {
            this.joinType = joinType;
            this.left = left;
            this.right = right;
            this.condition = condition;
        }

        public Expression Condition
        {
            get
            {
                return this.condition;
            }
        }

        public JoinType Join
        {
            get
            {
                return this.joinType;
            }
        }

        public Expression Left
        {
            get
            {
                return this.left;
            }
        }

        public Expression Right
        {
            get
            {
                return this.right;
            }
        }
    }
}
