namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public abstract class QueryLanguage
    {
        protected QueryLanguage()
        {
        }

        public virtual ProjectionExpression AddOuterJoinTest(ProjectionExpression proj)
        {
            QueryType columnType;
            Expression outerJoinTest = this.GetOuterJoinTest(proj.Select);
            SelectExpression select = proj.Select;
            ColumnExpression test = null;
            foreach (ColumnDeclaration declaration in select.Columns)
            {
                if (outerJoinTest.Equals(declaration.Expression))
                {
                    columnType = this.TypeSystem.GetColumnType(outerJoinTest.Type);
                    test = new ColumnExpression(outerJoinTest.Type, columnType, select.Alias, declaration.Name);
                    break;
                }
            }
            if (test == null)
            {
                test = outerJoinTest as ColumnExpression;
                string baseName = (test != null) ? test.Name : "Test";
                baseName = proj.Select.Columns.GetAvailableColumnName(baseName);
                columnType = this.TypeSystem.GetColumnType(outerJoinTest.Type);
                select = select.AddColumn(new ColumnDeclaration(baseName, outerJoinTest, columnType));
                test = new ColumnExpression(outerJoinTest.Type, columnType, select.Alias, baseName);
            }
            return new ProjectionExpression(select, new OuterJoinedExpression(test, proj.Projector), proj.Aggregator);
        }

        public virtual bool AggregateArgumentIsPredicate(string aggregateName)
        {
            return ((aggregateName == "Count") || (aggregateName == "LongCount"));
        }

        public virtual bool CanBeColumn(Expression expression)
        {
            return this.MustBeColumn(expression);
        }

        public virtual QueryLinguist CreateLinguist(QueryTranslator translator)
        {
            return new QueryLinguist(this, translator);
        }

        public abstract Expression GetGeneratedIdExpression(MemberInfo member);
        public virtual Expression GetOuterJoinTest(SelectExpression select)
        {
            List<ColumnExpression> list = JoinColumnGatherer.Gather(DeclaredAliasGatherer.Gather(select.From), select).ToList<ColumnExpression>();
            if (list.Count > 0)
            {
                foreach (ColumnExpression expression in list)
                {
                    foreach (ColumnDeclaration declaration in select.Columns)
                    {
                        if (expression.Equals(declaration.Expression))
                        {
                            return expression;
                        }
                    }
                }
                return list[0];
            }
            return Expression.Constant(1, typeof(int?));
        }

        public virtual Expression GetRowsAffectedExpression(Expression command)
        {
            return new FunctionExpression(typeof(int), "@@ROWCOUNT", null);
        }

        public virtual bool IsAggregate(MemberInfo member)
        {
            MethodInfo info = member as MethodInfo;
            if ((info != null) && ((info.DeclaringType == typeof(Queryable)) || (info.DeclaringType == typeof(Enumerable))))
            {
                string name = info.Name;
                if ((name != null) && ((((name == "Count") || (name == "LongCount")) || ((name == "Sum") || (name == "Min"))) || ((name == "Max") || (name == "Average"))))
                {
                    return true;
                }
            }
            PropertyInfo info2 = member as PropertyInfo;
            return (((info2 != null) && (info2.Name == "Count")) && typeof(IEnumerable).IsAssignableFrom(info2.DeclaringType));
        }

        public virtual bool IsRowsAffectedExpressions(Expression expression)
        {
            FunctionExpression expression2 = expression as FunctionExpression;
            return ((expression2 != null) && (expression2.Name == "@@ROWCOUNT"));
        }

        public virtual bool IsScalar(Type type)
        {
            type = King.Framework.Linq.TypeHelper.GetNonNullableType(type);
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Empty:
                case TypeCode.DBNull:
                    return false;

                case TypeCode.Object:
                    return ((((type == typeof(DateTimeOffset)) || (type == typeof(TimeSpan))) || (type == typeof(Guid))) || (type == typeof(byte[])));
            }
            return true;
        }

        public virtual bool MustBeColumn(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ((ExpressionType) 0x3ef):
                case ((ExpressionType) 0x3f0):
                case ((ExpressionType) 0x3f1):
                case ((ExpressionType) 0x3f4):
                case ((ExpressionType) 0x3ea):
                    return true;
            }
            return false;
        }

        public virtual string Quote(string name)
        {
            return name;
        }

        public virtual bool AllowDistinctInAggregates
        {
            get
            {
                return false;
            }
        }

        public virtual bool AllowsMultipleCommands
        {
            get
            {
                return false;
            }
        }

        public virtual bool AllowSubqueryInSelectWithoutFrom
        {
            get
            {
                return false;
            }
        }

        public abstract QueryTypeSystem TypeSystem { get; }

        private class JoinColumnGatherer
        {
            private HashSet<TableAlias> aliases;
            private HashSet<ColumnExpression> columns = new HashSet<ColumnExpression>();

            private JoinColumnGatherer(HashSet<TableAlias> aliases)
            {
                this.aliases = aliases;
            }

            private void Gather(Expression expression)
            {
                BinaryExpression expression2 = expression as BinaryExpression;
                if (expression2 != null)
                {
                    switch (expression2.NodeType)
                    {
                        case ExpressionType.And:
                        case ExpressionType.AndAlso:
                            if ((expression2.Type == typeof(bool)) || (expression2.Type == typeof(bool?)))
                            {
                                this.Gather(expression2.Left);
                                this.Gather(expression2.Right);
                            }
                            break;

                        case ExpressionType.Equal:
                        case ExpressionType.NotEqual:
                            if (this.IsExternalColumn(expression2.Left) && (this.GetColumn(expression2.Right) != null))
                            {
                                this.columns.Add(this.GetColumn(expression2.Right));
                            }
                            else if (this.IsExternalColumn(expression2.Right) && (this.GetColumn(expression2.Left) != null))
                            {
                                this.columns.Add(this.GetColumn(expression2.Left));
                            }
                            break;
                    }
                }
            }

            public static HashSet<ColumnExpression> Gather(HashSet<TableAlias> aliases, SelectExpression select)
            {
                QueryLanguage.JoinColumnGatherer gatherer = new QueryLanguage.JoinColumnGatherer(aliases);
                gatherer.Gather(select.Where);
                return gatherer.columns;
            }

            private ColumnExpression GetColumn(Expression exp)
            {
                while (exp.NodeType == ExpressionType.Convert)
                {
                    exp = ((UnaryExpression) exp).Operand;
                }
                return (exp as ColumnExpression);
            }

            private bool IsExternalColumn(Expression exp)
            {
                ColumnExpression column = this.GetColumn(exp);
                return !((column == null) || this.aliases.Contains(column.Alias));
            }
        }
    }
}
