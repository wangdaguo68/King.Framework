namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Linq.Expressions;

    public class AggregateExpression : DbExpression
    {
        private string aggregateName;
        private Expression argument;
        private bool isDistinct;

        public AggregateExpression(Type type, string aggregateName, Expression argument, bool isDistinct) : base(DbExpressionType.Aggregate, type)
        {
            this.aggregateName = aggregateName;
            this.argument = argument;
            this.isDistinct = isDistinct;
        }

        public string AggregateName
        {
            get
            {
                return this.aggregateName;
            }
        }

        public Expression Argument
        {
            get
            {
                return this.argument;
            }
        }

        public bool IsDistinct
        {
            get
            {
                return this.isDistinct;
            }
        }
    }
}
