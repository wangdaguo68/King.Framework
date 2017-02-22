namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class ReferencedAliasGatherer : DbExpressionVisitor
    {
        private HashSet<TableAlias> aliases = new HashSet<TableAlias>();

        private ReferencedAliasGatherer()
        {
        }

        public static HashSet<TableAlias> Gather(Expression source)
        {
            ReferencedAliasGatherer gatherer = new ReferencedAliasGatherer();
            gatherer.Visit(source);
            return gatherer.aliases;
        }

        protected override Expression VisitColumn(ColumnExpression column)
        {
            this.aliases.Add(column.Alias);
            return column;
        }
    }
}
