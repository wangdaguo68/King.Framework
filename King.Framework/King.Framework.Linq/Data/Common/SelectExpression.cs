namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;

    public class SelectExpression : AliasedExpression
    {
        private ReadOnlyCollection<ColumnDeclaration> columns;
        private Expression from;
        private ReadOnlyCollection<Expression> groupBy;
        private bool isDistinct;
        private ReadOnlyCollection<OrderExpression> orderBy;
        private bool reverse;
        private Expression skip;
        private Expression take;
        private Expression where;

        public SelectExpression(TableAlias alias, IEnumerable<ColumnDeclaration> columns, Expression from, Expression where) : this(alias, columns, from, where, null, null)
        {
        }

        public SelectExpression(TableAlias alias, IEnumerable<ColumnDeclaration> columns, Expression from, Expression where, IEnumerable<OrderExpression> orderBy, IEnumerable<Expression> groupBy) : this(alias, columns, from, where, orderBy, groupBy, false, null, null, false)
        {
        }

        public SelectExpression(TableAlias alias, IEnumerable<ColumnDeclaration> columns, Expression from, Expression where, IEnumerable<OrderExpression> orderBy, IEnumerable<Expression> groupBy, bool isDistinct, Expression skip, Expression take, bool reverse) : base(DbExpressionType.Select, typeof(void), alias)
        {
            this.columns = columns.ToReadOnly<ColumnDeclaration>();
            this.isDistinct = isDistinct;
            this.from = from;
            this.where = where;
            this.orderBy = orderBy.ToReadOnly<OrderExpression>();
            this.groupBy = groupBy.ToReadOnly<Expression>();
            this.take = take;
            this.skip = skip;
            this.reverse = reverse;
        }

        public ReadOnlyCollection<ColumnDeclaration> Columns
        {
            get
            {
                return this.columns;
            }
        }

        public Expression From
        {
            get
            {
                return this.from;
            }
        }

        public ReadOnlyCollection<Expression> GroupBy
        {
            get
            {
                return this.groupBy;
            }
        }

        public bool IsDistinct
        {
            get
            {
                return this.isDistinct;
            }
        }

        public bool IsReverse
        {
            get
            {
                return this.reverse;
            }
        }

        public ReadOnlyCollection<OrderExpression> OrderBy
        {
            get
            {
                return this.orderBy;
            }
        }

        public string QueryText
        {
            get
            {
                return SqlFormatter.Format(this, true);
            }
        }

        public Expression Skip
        {
            get
            {
                return this.skip;
            }
        }

        public Expression Take
        {
            get
            {
                return this.take;
            }
        }

        public Expression Where
        {
            get
            {
                return this.where;
            }
        }
    }
}
