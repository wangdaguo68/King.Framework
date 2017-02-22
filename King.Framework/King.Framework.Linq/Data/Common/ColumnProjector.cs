using System.Linq;

namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class ColumnProjector : DbExpressionVisitor
    {
        private HashSet<Expression> candidates;
        private HashSet<string> columnNames;
        private List<ColumnDeclaration> columns;
        private HashSet<TableAlias> existingAliases;
        private int iColumn;
        private QueryLanguage language;
        private Dictionary<ColumnExpression, ColumnExpression> map;
        private TableAlias newAlias;

        private ColumnProjector(QueryLanguage language, Expression expression, IEnumerable<ColumnDeclaration> existingColumns, TableAlias newAlias, IEnumerable<TableAlias> existingAliases)
        {
            this.language = language;
            this.newAlias = newAlias;
            this.existingAliases = new HashSet<TableAlias>(existingAliases);
            this.map = new Dictionary<ColumnExpression, ColumnExpression>();
            if (existingColumns != null)
            {
                this.columns = new List<ColumnDeclaration>(existingColumns);
                this.columnNames = new HashSet<string>(from c in existingColumns select c.Name);
            }
            else
            {
                this.columns = new List<ColumnDeclaration>();
                this.columnNames = new HashSet<string>();
            }
            this.candidates = Nominator.Nominate(language, expression);
        }

        private string GetNextColumnName()
        {
            return this.GetUniqueColumnName("c" + this.iColumn++);
        }

        private string GetUniqueColumnName(string name)
        {
            string str = name;
            int num = 1;
            while (this.IsColumnNameInUse(name))
            {
                name = str + num++;
            }
            return name;
        }

        private bool IsColumnNameInUse(string name)
        {
            return this.columnNames.Contains(name);
        }

        public static ProjectedColumns ProjectColumns(QueryLanguage language, Expression expression, IEnumerable<ColumnDeclaration> existingColumns, TableAlias newAlias, IEnumerable<TableAlias> existingAliases)
        {
            ColumnProjector projector = new ColumnProjector(language, expression, existingColumns, newAlias, existingAliases);
            return new ProjectedColumns(projector.Visit(expression), projector.columns.AsReadOnly());
        }

        public static ProjectedColumns ProjectColumns(QueryLanguage language, Expression expression, IEnumerable<ColumnDeclaration> existingColumns, TableAlias newAlias, params TableAlias[] existingAliases)
        {
            return ProjectColumns(language, expression, existingColumns, newAlias, (IEnumerable<TableAlias>)existingAliases);
        }

        protected override Expression Visit(Expression expression)
        {
            if (this.candidates.Contains(expression))
            {
                string nextColumnName;
                if (expression.NodeType == ((ExpressionType)0x3ea))
                {
                    ColumnExpression expression3;
                    ColumnExpression key = (ColumnExpression)expression;
                    if (this.map.TryGetValue(key, out expression3))
                    {
                        return expression3;
                    }
                    foreach (ColumnDeclaration declaration in this.columns)
                    {
                        ColumnExpression expression4 = declaration.Expression as ColumnExpression;
                        if (((expression4 != null) && (expression4.Alias == key.Alias)) && (expression4.Name == key.Name))
                        {
                            return new ColumnExpression(key.Type, key.QueryType, this.newAlias, declaration.Name);
                        }
                    }
                    if (this.existingAliases.Contains(key.Alias))
                    {
                        int count = this.columns.Count;
                        nextColumnName = this.GetUniqueColumnName(key.Name);
                        this.columns.Add(new ColumnDeclaration(nextColumnName, key, key.QueryType));
                        expression3 = new ColumnExpression(key.Type, key.QueryType, this.newAlias, nextColumnName);
                        this.map.Add(key, expression3);
                        this.columnNames.Add(nextColumnName);
                        return expression3;
                    }
                    return key;
                }
                nextColumnName = this.GetNextColumnName();
                QueryType columnType = this.language.TypeSystem.GetColumnType(expression.Type);
                this.columns.Add(new ColumnDeclaration(nextColumnName, expression, columnType));
                return new ColumnExpression(expression.Type, columnType, this.newAlias, nextColumnName);
            }
            return base.Visit(expression);
        }

        private class Nominator : DbExpressionVisitor
        {
            private HashSet<Expression> candidates;
            private bool isBlocked;
            private QueryLanguage language;

            private Nominator(QueryLanguage language)
            {
                this.language = language;
                this.candidates = new HashSet<Expression>();
                this.isBlocked = false;
            }

            internal static HashSet<Expression> Nominate(QueryLanguage language, Expression expression)
            {
                ColumnProjector.Nominator nominator = new ColumnProjector.Nominator(language);
                nominator.Visit(expression);
                return nominator.candidates;
            }

            protected override Expression Visit(Expression expression)
            {
                if (expression != null)
                {
                    bool isBlocked = this.isBlocked;
                    this.isBlocked = false;
                    if (this.language.MustBeColumn(expression))
                    {
                        this.candidates.Add(expression);
                        return expression;
                    }
                    base.Visit(expression);
                    if (!this.isBlocked)
                    {
                        if (this.language.CanBeColumn(expression))
                        {
                            this.candidates.Add(expression);
                        }
                        else
                        {
                            this.isBlocked = true;
                        }
                    }
                    this.isBlocked |= isBlocked;
                }
                return expression;
            }

            protected override Expression VisitProjection(ProjectionExpression proj)
            {
                this.Visit(proj.Projector);
                return proj;
            }
        }
    }
}
