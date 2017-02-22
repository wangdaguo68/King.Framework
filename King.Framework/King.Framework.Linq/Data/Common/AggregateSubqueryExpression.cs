namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Linq.Expressions;

    public class AggregateSubqueryExpression : DbExpression
    {
        private ScalarExpression aggregateAsSubquery;
        private Expression aggregateInGroupSelect;
        private TableAlias groupByAlias;

        public AggregateSubqueryExpression(TableAlias groupByAlias, Expression aggregateInGroupSelect, ScalarExpression aggregateAsSubquery) : base(DbExpressionType.AggregateSubquery, aggregateAsSubquery.Type)
        {
            this.aggregateInGroupSelect = aggregateInGroupSelect;
            this.groupByAlias = groupByAlias;
            this.aggregateAsSubquery = aggregateAsSubquery;
        }

        public ScalarExpression AggregateAsSubquery
        {
            get
            {
                return this.aggregateAsSubquery;
            }
        }

        public Expression AggregateInGroupSelect
        {
            get
            {
                return this.aggregateInGroupSelect;
            }
        }

        public TableAlias GroupByAlias
        {
            get
            {
                return this.groupByAlias;
            }
        }
    }
}
