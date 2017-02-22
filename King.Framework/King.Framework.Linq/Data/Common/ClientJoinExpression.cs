using System.Linq.Expressions;

namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class ClientJoinExpression : DbExpression
    {
        private ReadOnlyCollection<Expression> innerKey;
        private ReadOnlyCollection<Expression> outerKey;
        private ProjectionExpression projection;

        public ClientJoinExpression(ProjectionExpression projection, IEnumerable<Expression> outerKey, IEnumerable<Expression> innerKey) : base(DbExpressionType.ClientJoin, projection.Type)
        {
            this.outerKey = outerKey.ToReadOnly<Expression>();
            this.innerKey = innerKey.ToReadOnly<Expression>();
            this.projection = projection;
        }

        public ReadOnlyCollection<Expression> InnerKey
        {
            get
            {
                return this.innerKey;
            }
        }

        public ReadOnlyCollection<Expression> OuterKey
        {
            get
            {
                return this.outerKey;
            }
        }

        public ProjectionExpression Projection
        {
            get
            {
                return this.projection;
            }
        }
    }
}
