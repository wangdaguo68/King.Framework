namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class DeclaredAliasGatherer : DbExpressionVisitor
    {
        private HashSet<TableAlias> aliases = new HashSet<TableAlias>();

        private DeclaredAliasGatherer()
        {
        }

        public static HashSet<TableAlias> Gather(Expression source)
        {
            DeclaredAliasGatherer gatherer = new DeclaredAliasGatherer();
            gatherer.Visit(source);
            return gatherer.aliases;
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            this.aliases.Add(select.Alias);
            return select;
        }

        protected override Expression VisitTable(TableExpression table)
        {
            this.aliases.Add(table.Alias);
            return table;
        }
    }
}
