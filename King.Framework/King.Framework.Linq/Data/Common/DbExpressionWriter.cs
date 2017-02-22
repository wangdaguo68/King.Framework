namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq.Expressions;

    public class DbExpressionWriter : ExpressionWriter
    {
        private Dictionary<TableAlias, int> aliasMap;
        private QueryLanguage language;

        protected DbExpressionWriter(TextWriter writer, QueryLanguage language) : base(writer)
        {
            this.aliasMap = new Dictionary<TableAlias, int>();
            this.language = language;
        }

        protected void AddAlias(TableAlias alias)
        {
            if (!this.aliasMap.ContainsKey(alias))
            {
                this.aliasMap.Add(alias, this.aliasMap.Count);
            }
        }

        protected virtual string FormatQuery(Expression query)
        {
            if (this.language != null)
            {
            }
            return SqlFormatter.Format(query, true);
        }

        protected override Expression Visit(Expression exp)
        {
            if (exp == null)
            {
                return null;
            }
            switch (exp.NodeType)
            {
                case ((ExpressionType) 0x3e9):
                    return this.VisitClientJoin((ClientJoinExpression) exp);

                case ((ExpressionType) 0x3ea):
                    return this.VisitColumn((ColumnExpression) exp);

                case ((ExpressionType) 0x3eb):
                    return this.VisitSelect((SelectExpression) exp);

                case ((ExpressionType) 0x3ec):
                    return this.VisitProjection((ProjectionExpression) exp);

                case ((ExpressionType) 0x3ed):
                    return this.VisitEntity((EntityExpression) exp);

                case ((ExpressionType) 0x3f9):
                    return this.VisitOuterJoined((OuterJoinedExpression) exp);

                case ((ExpressionType) 0x3fa):
                case ((ExpressionType) 0x3fb):
                case ((ExpressionType) 0x3fc):
                case ((ExpressionType) 0x3ff):
                case ((ExpressionType) 0x400):
                case ((ExpressionType) 0x401):
                    return this.VisitCommand((CommandExpression) exp);

                case ((ExpressionType) 0x3fd):
                    return this.VisitBatch((BatchExpression) exp);

                case ((ExpressionType) 0x3fe):
                    return this.VisitFunction((FunctionExpression) exp);
            }
            if (exp is DbExpression)
            {
                base.Write(this.FormatQuery(exp));
                return exp;
            }
            return base.Visit(exp);
        }

        protected virtual Expression VisitBatch(BatchExpression batch)
        {
            base.Write("Batch(");
            base.WriteLine(ExpressionWriter.Indentation.Inner);
            this.Visit(batch.Input);
            base.Write(",");
            base.WriteLine(ExpressionWriter.Indentation.Same);
            this.Visit(batch.Operation);
            base.Write(",");
            base.WriteLine(ExpressionWriter.Indentation.Same);
            this.Visit(batch.BatchSize);
            base.Write(", ");
            this.Visit(batch.Stream);
            base.WriteLine(ExpressionWriter.Indentation.Outer);
            base.Write(")");
            return batch;
        }

        protected virtual Expression VisitClientJoin(ClientJoinExpression join)
        {
            this.AddAlias(join.Projection.Select.Alias);
            base.Write("ClientJoin(");
            base.WriteLine(ExpressionWriter.Indentation.Inner);
            base.Write("OuterKey(");
            this.VisitExpressionList(join.OuterKey);
            base.Write("),");
            base.WriteLine(ExpressionWriter.Indentation.Same);
            base.Write("InnerKey(");
            this.VisitExpressionList(join.InnerKey);
            base.Write("),");
            base.WriteLine(ExpressionWriter.Indentation.Same);
            this.Visit(join.Projection);
            base.WriteLine(ExpressionWriter.Indentation.Outer);
            base.Write(")");
            return join;
        }

        protected virtual Expression VisitColumn(ColumnExpression column)
        {
            int num;
            string text = this.aliasMap.TryGetValue(column.Alias, out num) ? ("A" + num) : ("A" + ((column.Alias != null) ? column.Alias.GetHashCode().ToString() : "") + "?");
            base.Write(text);
            base.Write(".");
            base.Write("Column(\"");
            base.Write(column.Name);
            base.Write("\")");
            return column;
        }

        protected virtual Expression VisitCommand(CommandExpression command)
        {
            base.Write(this.FormatQuery(command));
            return command;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (c.Type == typeof(QueryCommand))
            {
                QueryCommand command = (QueryCommand) c.Value;
                base.Write("new QueryCommand {");
                base.WriteLine(ExpressionWriter.Indentation.Inner);
                base.Write("\"" + command.CommandText + "\"");
                base.Write(",");
                base.WriteLine(ExpressionWriter.Indentation.Same);
                this.Visit(Expression.Constant(command.Parameters));
                base.Write(")");
                base.WriteLine(ExpressionWriter.Indentation.Outer);
                return c;
            }
            return base.VisitConstant(c);
        }

        protected virtual Expression VisitEntity(EntityExpression entity)
        {
            this.Visit(entity.Expression);
            return entity;
        }

        protected virtual Expression VisitFunction(FunctionExpression function)
        {
            base.Write("FUNCTION ");
            base.Write(function.Name);
            if (function.Arguments.Count > 0)
            {
                base.Write("(");
                this.VisitExpressionList(function.Arguments);
                base.Write(")");
            }
            return function;
        }

        protected virtual Expression VisitOuterJoined(OuterJoinedExpression outer)
        {
            base.Write("Outer(");
            base.WriteLine(ExpressionWriter.Indentation.Inner);
            this.Visit(outer.Test);
            base.Write(", ");
            base.WriteLine(ExpressionWriter.Indentation.Same);
            this.Visit(outer.Expression);
            base.WriteLine(ExpressionWriter.Indentation.Outer);
            base.Write(")");
            return outer;
        }

        protected virtual Expression VisitProjection(ProjectionExpression projection)
        {
            this.AddAlias(projection.Select.Alias);
            base.Write("Project(");
            base.WriteLine(ExpressionWriter.Indentation.Inner);
            base.Write("@\"");
            this.Visit(projection.Select);
            base.Write("\",");
            base.WriteLine(ExpressionWriter.Indentation.Same);
            this.Visit(projection.Projector);
            base.Write(",");
            base.WriteLine(ExpressionWriter.Indentation.Same);
            this.Visit(projection.Aggregator);
            base.WriteLine(ExpressionWriter.Indentation.Outer);
            base.Write(")");
            return projection;
        }

        protected virtual Expression VisitSelect(SelectExpression select)
        {
            base.Write(select.QueryText);
            return select;
        }

        protected virtual Expression VisitVariable(VariableExpression vex)
        {
            base.Write(this.FormatQuery(vex));
            return vex;
        }

        public static void Write(TextWriter writer, Expression expression)
        {
            Write(writer, null, expression);
        }

        public static void Write(TextWriter writer, QueryLanguage language, Expression expression)
        {
            new DbExpressionWriter(writer, language).Visit(expression);
        }

        public static string WriteToString(Expression expression)
        {
            return WriteToString(null, expression);
        }

        public static string WriteToString(QueryLanguage language, Expression expression)
        {
            StringWriter writer = new StringWriter();
            Write(writer, language, expression);
            return writer.ToString();
        }
    }
}
