using System.Linq.Expressions;

namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class FunctionExpression : DbExpression
    {
        private ReadOnlyCollection<Expression> arguments;
        private string name;

        public FunctionExpression(Type type, string name, IEnumerable<Expression> arguments) : base(DbExpressionType.Function, type)
        {
            this.name = name;
            this.arguments = arguments.ToReadOnly<Expression>();
        }

        public ReadOnlyCollection<Expression> Arguments
        {
            get
            {
                return this.arguments;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }
    }
}
