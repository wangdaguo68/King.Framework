using System.Collections.Generic;

namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Linq.Expressions;

    public class ProjectionExpression : DbExpression
    {
        private LambdaExpression aggregator;
        private Expression projector;
        private SelectExpression select;

        public ProjectionExpression(SelectExpression source, Expression projector) : this(source, projector, null)
        {
        }

        public ProjectionExpression(SelectExpression source, Expression projector, LambdaExpression aggregator) : base(DbExpressionType.Projection, (aggregator != null) ? aggregator.Body.Type : typeof(IEnumerable<>).MakeGenericType(new Type[] { projector.Type }))
        {
            this.select = source;
            this.projector = projector;
            this.aggregator = aggregator;
        }

        public override string ToString()
        {
            return DbExpressionWriter.WriteToString(this);
        }

        public LambdaExpression Aggregator
        {
            get
            {
                return this.aggregator;
            }
        }

        public bool IsSingleton
        {
            get
            {
                return ((this.aggregator != null) && (this.aggregator.Body.Type == this.projector.Type));
            }
        }

        public Expression Projector
        {
            get
            {
                return this.projector;
            }
        }

        public string QueryText
        {
            get
            {
                return SqlFormatter.Format(this.select, true);
            }
        }

        public SelectExpression Select
        {
            get
            {
                return this.select;
            }
        }
    }
}
