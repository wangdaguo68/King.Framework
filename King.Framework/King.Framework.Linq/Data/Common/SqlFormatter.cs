namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;

    public class SqlFormatter : DbExpressionVisitor
    {
        private Dictionary<TableAlias, string> aliases;
        private int depth;
        private bool forDebug;
        private bool hideColumnAliases;
        private bool hideTableAliases;
        private int indent;
        private bool isNested;
        private QueryLanguage language;
        private StringBuilder sb;

        protected SqlFormatter(QueryLanguage language) : this(language, false)
        {
        }

        private SqlFormatter(QueryLanguage language, bool forDebug)
        {
            this.indent = 2;
            this.language = language;
            this.sb = new StringBuilder();
            this.aliases = new Dictionary<TableAlias, string>();
            this.forDebug = forDebug;
        }

        protected void AddAlias(TableAlias alias)
        {
            string str;
            if (!this.aliases.TryGetValue(alias, out str))
            {
                str = "t" + this.aliases.Count;
                this.aliases.Add(alias, str);
            }
        }

        protected virtual void AddAliases(Expression expr)
        {
            AliasedExpression expression = expr as AliasedExpression;
            if (expression != null)
            {
                this.AddAlias(expression.Alias);
            }
            else
            {
                JoinExpression expression2 = expr as JoinExpression;
                if (expression2 != null)
                {
                    this.AddAliases(expression2.Left);
                    this.AddAliases(expression2.Right);
                }
            }
        }

        public static string Format(Expression expression)
        {
            SqlFormatter formatter = new SqlFormatter(null, false);
            formatter.Visit(expression);
            return formatter.ToString();
        }

        public static string Format(Expression expression, bool forDebug)
        {
            SqlFormatter formatter = new SqlFormatter(null, forDebug);
            formatter.Visit(expression);
            return formatter.ToString();
        }

        protected virtual string GetAliasName(TableAlias alias)
        {
            string str;
            if (!this.aliases.TryGetValue(alias, out str))
            {
                str = "A" + alias.GetHashCode() + "?";
                this.aliases.Add(alias, str);
            }
            return str;
        }

        protected virtual string GetOperator(BinaryExpression b)
        {
            switch (b.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";

                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return (this.IsBoolean(b.Left.Type) ? "AND" : "&");

                case ExpressionType.Divide:
                    return "/";

                case ExpressionType.Equal:
                    return "=";

                case ExpressionType.ExclusiveOr:
                    return "^";

                case ExpressionType.GreaterThan:
                    return ">";

                case ExpressionType.GreaterThanOrEqual:
                    return ">=";

                case ExpressionType.LeftShift:
                    return "<<";

                case ExpressionType.LessThan:
                    return "<";

                case ExpressionType.LessThanOrEqual:
                    return "<=";

                case ExpressionType.Modulo:
                    return "%";

                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";

                case ExpressionType.NotEqual:
                    return "<>";

                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return (this.IsBoolean(b.Left.Type) ? "OR" : "|");

                case ExpressionType.RightShift:
                    return ">>";

                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
            }
            return "";
        }

        protected virtual string GetOperator(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    return "-";

                case ExpressionType.UnaryPlus:
                    return "+";

                case ExpressionType.Not:
                    return (this.IsBoolean(u.Operand.Type) ? "NOT" : "~");
            }
            return "";
        }

        protected virtual string GetOperator(string methodName)
        {
            switch (methodName)
            {
                case "Add":
                    return "+";

                case "Subtract":
                    return "-";

                case "Multiply":
                    return "*";

                case "Divide":
                    return "/";

                case "Negate":
                    return "-";

                case "Remainder":
                    return "%";
            }
            return null;
        }

        protected void Indent(Indentation style)
        {
            if (style == Indentation.Inner)
            {
                this.depth++;
            }
            else if (style == Indentation.Outer)
            {
                this.depth--;
                Debug.Assert(this.depth >= 0);
            }
        }

        protected virtual bool IsBoolean(Type type)
        {
            return ((type == typeof(bool)) || (type == typeof(bool?)));
        }

        protected virtual bool IsInteger(Type type)
        {
            return King.Framework.Linq.TypeHelper.IsInteger(type);
        }

        protected virtual bool IsPredicate(Expression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return this.IsBoolean(((BinaryExpression) expr).Type);

                case ExpressionType.Call:
                    return this.IsBoolean(((MethodCallExpression) expr).Type);

                case ExpressionType.Equal:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.NotEqual:
                case ((ExpressionType) 0x3f1):
                case ((ExpressionType) 0x3f2):
                case ((ExpressionType) 0x3f5):
                case ((ExpressionType) 0x3f6):
                    return true;

                case ExpressionType.Not:
                    return this.IsBoolean(((UnaryExpression) expr).Type);
            }
            return false;
        }

        protected virtual bool RequiresAsteriskWhenNoArgument(string aggregateName)
        {
            return ((aggregateName == "Count") || (aggregateName == "LongCount"));
        }

        public override string ToString()
        {
            return this.sb.ToString();
        }

        protected override Expression Visit(Expression exp)
        {
            if (exp == null)
            {
                return null;
            }
            switch (exp.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Call:
                case ExpressionType.Coalesce:
                case ExpressionType.Conditional:
                case ExpressionType.Constant:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Divide:
                case ExpressionType.Equal:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LeftShift:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.MemberAccess:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Negate:
                case ExpressionType.UnaryPlus:
                case ExpressionType.NegateChecked:
                case ExpressionType.New:
                case ExpressionType.Not:
                case ExpressionType.NotEqual:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.Power:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ((ExpressionType) 0x3e8):
                case ((ExpressionType) 0x3ea):
                case ((ExpressionType) 0x3eb):
                case ((ExpressionType) 0x3ec):
                case ((ExpressionType) 0x3ee):
                case ((ExpressionType) 0x3ef):
                case ((ExpressionType) 0x3f0):
                case ((ExpressionType) 0x3f1):
                case ((ExpressionType) 0x3f2):
                case ((ExpressionType) 0x3f4):
                case ((ExpressionType) 0x3f5):
                case ((ExpressionType) 0x3f6):
                case ((ExpressionType) 0x3f7):
                case ((ExpressionType) 0x3f8):
                case ((ExpressionType) 0x3fa):
                case ((ExpressionType) 0x3fb):
                case ((ExpressionType) 0x3fc):
                case ((ExpressionType) 0x3fe):
                case ((ExpressionType) 0x3ff):
                case ((ExpressionType) 0x400):
                case ((ExpressionType) 0x401):
                case ((ExpressionType) 0x402):
                    return base.Visit(exp);
            }
            if (!this.forDebug)
            {
                throw new NotSupportedException(string.Format("The LINQ expression node of type {0} is not supported", exp.NodeType));
            }
            this.Write(string.Format("?{0}?(", exp.NodeType));
            base.Visit(exp);
            this.Write(")");
            return exp;
        }

        protected override Expression VisitAggregate(AggregateExpression aggregate)
        {
            this.WriteAggregateName(aggregate.AggregateName);
            this.Write("(");
            if (aggregate.IsDistinct)
            {
                this.Write("DISTINCT ");
            }
            if (aggregate.Argument != null)
            {
                this.VisitValue(aggregate.Argument);
            }
            else if (this.RequiresAsteriskWhenNoArgument(aggregate.AggregateName))
            {
                this.Write("*");
            }
            this.Write(")");
            return aggregate;
        }

        protected override Expression VisitBetween(BetweenExpression between)
        {
            this.VisitValue(between.Expression);
            this.Write(" BETWEEN ");
            this.VisitValue(between.Lower);
            this.Write(" AND ");
            this.VisitValue(between.Upper);
            return between;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            ConstantExpression expression3;
            string @operator = this.GetOperator(b);
            Expression left = b.Left;
            Expression right = b.Right;
            this.Write("(");
            switch (b.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Divide:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.LeftShift:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    goto Label_03F3;

                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    if (this.IsBoolean(left.Type))
                    {
                        this.VisitPredicate(left);
                        this.Write(" ");
                        this.Write(@operator);
                        this.Write(" ");
                        this.VisitPredicate(right);
                    }
                    else
                    {
                        this.VisitValue(left);
                        this.Write(" ");
                        this.Write(@operator);
                        this.Write(" ");
                        this.VisitValue(right);
                    }
                    goto Label_04B3;

                case ExpressionType.Equal:
                    if (right.NodeType != ExpressionType.Constant)
                    {
                        if (left.NodeType == ExpressionType.Constant)
                        {
                            expression3 = (ConstantExpression) left;
                            if (expression3.Value == null)
                            {
                                this.Visit(right);
                                this.Write(" IS NULL");
                                goto Label_04B3;
                            }
                        }
                        break;
                    }
                    expression3 = (ConstantExpression) right;
                    if (expression3.Value != null)
                    {
                        break;
                    }
                    this.Visit(left);
                    this.Write(" IS NULL");
                    goto Label_04B3;

                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    break;

                case ExpressionType.NotEqual:
                    if (right.NodeType != ExpressionType.Constant)
                    {
                        if (left.NodeType == ExpressionType.Constant)
                        {
                            expression3 = (ConstantExpression) left;
                            if (expression3.Value == null)
                            {
                                this.Visit(right);
                                this.Write(" IS NOT NULL");
                                goto Label_04B3;
                            }
                        }
                        break;
                    }
                    expression3 = (ConstantExpression) right;
                    if (expression3.Value != null)
                    {
                        break;
                    }
                    this.Visit(left);
                    this.Write(" IS NOT NULL");
                    goto Label_04B3;

                default:
                    if (!this.forDebug)
                    {
                        throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
                    }
                    this.Write(string.Format("?{0}?", b.NodeType));
                    this.Write("(");
                    this.Visit(b.Left);
                    this.Write(", ");
                    this.Visit(b.Right);
                    this.Write(")");
                    return b;
            }
            if ((left.NodeType == ExpressionType.Call) && (right.NodeType == ExpressionType.Constant))
            {
                MethodCallExpression expression4 = (MethodCallExpression) left;
                expression3 = (ConstantExpression) right;
                if (((expression3.Value != null) && (expression3.Value.GetType() == typeof(int))) && (((int) expression3.Value) == 0))
                {
                    if (((expression4.Method.Name == "CompareTo") && !expression4.Method.IsStatic) && (expression4.Arguments.Count == 1))
                    {
                        left = expression4.Object;
                        right = expression4.Arguments[0];
                    }
                    else if ((((expression4.Method.DeclaringType == typeof(string)) || (expression4.Method.DeclaringType == typeof(decimal))) && ((expression4.Method.Name == "Compare") && expression4.Method.IsStatic)) && (expression4.Arguments.Count == 2))
                    {
                        left = expression4.Arguments[0];
                        right = expression4.Arguments[1];
                    }
                }
            }
        Label_03F3:
            this.VisitValue(left);
            this.Write(" ");
            this.Write(@operator);
            this.Write(" ");
            this.VisitValue(right);
        Label_04B3:
            this.Write(")");
            return b;
        }

        protected override Expression VisitBlock(BlockCommand block)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitColumn(ColumnExpression column)
        {
            if (!((column.Alias == null) || this.HideColumnAliases))
            {
                this.WriteAliasName(this.GetAliasName(column.Alias));
                this.Write(".");
            }
            this.WriteColumnName(column.Name);
            return column;
        }

        protected override Expression VisitConditional(ConditionalExpression c)
        {
            if (!this.forDebug)
            {
                throw new NotSupportedException(string.Format("Conditional expressions not supported", new object[0]));
            }
            this.Write("?iff?(");
            this.Visit(c.Test);
            this.Write(", ");
            this.Visit(c.IfTrue);
            this.Write(", ");
            this.Visit(c.IfFalse);
            this.Write(")");
            return c;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            this.WriteValue(c.Value);
            return c;
        }

        protected override Expression VisitDeclaration(DeclarationCommand decl)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitDelete(DeleteCommand delete)
        {
            this.Write("DELETE FROM ");
            bool hideTableAliases = this.HideTableAliases;
            bool hideColumnAliases = this.HideColumnAliases;
            this.HideTableAliases = true;
            this.HideColumnAliases = true;
            this.VisitSource(delete.Table);
            if (delete.Where != null)
            {
                this.WriteLine(Indentation.Same);
                this.Write("WHERE ");
                this.VisitPredicate(delete.Where);
            }
            this.HideTableAliases = hideTableAliases;
            this.HideColumnAliases = hideColumnAliases;
            return delete;
        }

        protected override Expression VisitExists(ExistsExpression exists)
        {
            this.Write("EXISTS(");
            this.WriteLine(Indentation.Inner);
            this.Visit(exists.Select);
            this.WriteLine(Indentation.Same);
            this.Write(")");
            this.Indent(Indentation.Outer);
            return exists;
        }

        protected override Expression VisitFunction(FunctionExpression func)
        {
            this.Write(func.Name);
            if (func.Arguments.Count > 0)
            {
                this.Write("(");
                int num = 0;
                int count = func.Arguments.Count;
                while (num < count)
                {
                    if (num > 0)
                    {
                        this.Write(", ");
                    }
                    this.Visit(func.Arguments[num]);
                    num++;
                }
                this.Write(")");
            }
            return func;
        }

        protected override Expression VisitIf(IFCommand ifx)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitIn(InExpression @in)
        {
            if (@in.Values != null)
            {
                if (@in.Values.Count == 0)
                {
                    this.Write("0 <> 0");
                    return @in;
                }
                this.VisitValue(@in.Expression);
                this.Write(" IN (");
                int num = 0;
                int count = @in.Values.Count;
                while (num < count)
                {
                    if (num > 0)
                    {
                        this.Write(", ");
                    }
                    this.VisitValue(@in.Values[num]);
                    num++;
                }
                this.Write(")");
                return @in;
            }
            this.VisitValue(@in.Expression);
            this.Write(" IN (");
            this.WriteLine(Indentation.Inner);
            this.Visit(@in.Select);
            this.WriteLine(Indentation.Same);
            this.Write(")");
            this.Indent(Indentation.Outer);
            return @in;
        }

        protected override Expression VisitInsert(InsertCommand insert)
        {
            ColumnAssignment assignment;
            this.Write("INSERT INTO ");
            this.WriteTableName(insert.Table.Name);
            this.Write("(");
            int num = 0;
            int count = insert.Assignments.Count;
            while (num < count)
            {
                assignment = insert.Assignments[num];
                if (num > 0)
                {
                    this.Write(", ");
                }
                this.WriteColumnName(assignment.Column.Name);
                num++;
            }
            this.Write(")");
            this.WriteLine(Indentation.Same);
            this.Write("VALUES (");
            num = 0;
            count = insert.Assignments.Count;
            while (num < count)
            {
                assignment = insert.Assignments[num];
                if (num > 0)
                {
                    this.Write(", ");
                }
                this.Visit(assignment.Expression);
                num++;
            }
            this.Write(")");
            return insert;
        }

        protected override Expression VisitIsNull(IsNullExpression isnull)
        {
            this.VisitValue(isnull.Expression);
            this.Write(" IS NULL");
            return isnull;
        }

        protected override Expression VisitJoin(JoinExpression join)
        {
            this.VisitJoinLeft(join.Left);
            this.WriteLine(Indentation.Same);
            switch (join.Join)
            {
                case JoinType.CrossJoin:
                    this.Write("CROSS JOIN ");
                    break;

                case JoinType.InnerJoin:
                    this.Write("INNER JOIN ");
                    break;

                case JoinType.CrossApply:
                    this.Write("CROSS APPLY ");
                    break;

                case JoinType.OuterApply:
                    this.Write("OUTER APPLY ");
                    break;

                case JoinType.LeftOuter:
                case JoinType.SingletonLeftOuter:
                    this.Write("LEFT OUTER JOIN ");
                    break;
            }
            this.VisitJoinRight(join.Right);
            if (join.Condition != null)
            {
                this.WriteLine(Indentation.Inner);
                this.Write("ON ");
                this.VisitPredicate(join.Condition);
                this.Indent(Indentation.Outer);
            }
            return join;
        }

        protected virtual Expression VisitJoinLeft(Expression source)
        {
            return this.VisitSource(source);
        }

        protected virtual Expression VisitJoinRight(Expression source)
        {
            return this.VisitSource(source);
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (this.forDebug)
            {
                this.Visit(m.Expression);
                this.Write(".");
                this.Write(m.Member.Name);
                return m;
            }
            if ((m.Expression.NodeType == ((ExpressionType) 0x3ea)) && King.Framework.Linq.TypeHelper.IsNullableType(m.Expression.Type))
            {
                if (m.Member.Name == "Value")
                {
                    this.Visit(m.Expression);
                    return m;
                }
                if (m.Member.Name == "HasValue")
                {
                    this.Visit(m.Expression);
                    this.sb.Append(" IS NOT NULL ");
                    return m;
                }
            }
            throw new NotSupportedException(string.Format("The member access '{0}' is not supported", m.Member));
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(decimal))
            {
                switch (m.Method.Name)
                {
                    case "Add":
                    case "Subtract":
                    case "Multiply":
                    case "Divide":
                    case "Remainder":
                        this.Write("(");
                        this.VisitValue(m.Arguments[0]);
                        this.Write(" ");
                        this.Write(this.GetOperator(m.Method.Name));
                        this.Write(" ");
                        this.VisitValue(m.Arguments[1]);
                        this.Write(")");
                        return m;

                    case "Negate":
                        this.Write("-");
                        this.Visit(m.Arguments[0]);
                        this.Write("");
                        return m;

                    case "Compare":
                        this.Visit(Expression.Condition(Expression.Equal(m.Arguments[0], m.Arguments[1]), Expression.Constant(0), Expression.Condition(Expression.LessThan(m.Arguments[0], m.Arguments[1]), Expression.Constant(-1), Expression.Constant(1))));
                        return m;
                }
            }
            else
            {
                if ((m.Method.Name == "ToString") && (m.Object.Type == typeof(string)))
                {
                    return this.Visit(m.Object);
                }
                if (m.Method.Name == "Equals")
                {
                    if (m.Method.IsStatic && (m.Method.DeclaringType == typeof(object)))
                    {
                        this.Write("(");
                        this.Visit(m.Arguments[0]);
                        this.Write(" = ");
                        this.Visit(m.Arguments[1]);
                        this.Write(")");
                        return m;
                    }
                    if ((!m.Method.IsStatic && (m.Arguments.Count == 1)) && (m.Arguments[0].Type == m.Object.Type))
                    {
                        this.Write("(");
                        this.Visit(m.Object);
                        this.Write(" = ");
                        this.Visit(m.Arguments[0]);
                        this.Write(")");
                        return m;
                    }
                }
                else if ((((m.Method.Name == "Contains") && (m.Object != null)) && m.Object.Type.IsArray) && (m.Object.NodeType == ExpressionType.Constant))
                {
                    Array array = (m.Object as ConstantExpression).Value as Array;
                    if (array != null)
                    {
                        this.Visit(m.Arguments[0]);
                        this.sb.Append(" IN (");
                        int num = -1;
                        foreach (object obj2 in array)
                        {
                            num++;
                            if (num > 0)
                            {
                                this.sb.Append(",");
                            }
                            this.Visit(Expression.Constant(obj2));
                        }
                        this.sb.Append(") ");
                    }
                    return m;
                }
            }
            if (!this.forDebug)
            {
                throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
            }
            if (m.Object != null)
            {
                this.Visit(m.Object);
                this.Write(".");
            }
            this.Write(string.Format("?{0}?", m.Method.Name));
            this.Write("(");
            for (int i = 0; i < m.Arguments.Count; i++)
            {
                if (i > 0)
                {
                    this.Write(", ");
                }
                this.Visit(m.Arguments[i]);
            }
            this.Write(")");
            return m;
        }

        protected override Expression VisitNamedValue(NamedValueExpression value)
        {
            this.WriteParameterName(value.Name);
            return value;
        }

        protected override NewExpression VisitNew(NewExpression nex)
        {
            if (!this.forDebug)
            {
                throw new NotSupportedException(string.Format("The construtor for '{0}' is not supported", nex.Constructor.DeclaringType));
            }
            this.Write("?new?");
            this.Write(nex.Type.Name);
            this.Write("(");
            for (int i = 0; i < nex.Arguments.Count; i++)
            {
                if (i > 0)
                {
                    this.Write(", ");
                }
                this.Visit(nex.Arguments[i]);
            }
            this.Write(")");
            return nex;
        }

        protected virtual Expression VisitPredicate(Expression expr)
        {
            if (expr.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression expression = expr as MemberExpression;
                if (((expression != null) && (expression.Expression != null)) && ((expression.Expression.Type == typeof(bool?)) && (expression.Member.Name == "Value")))
                {
                    this.Visit(expression.Expression);
                    this.sb.Append("=");
                    this.Visit(Expression.Constant(true));
                    return expr;
                }
            }
            this.Visit(expr);
            if (!this.IsPredicate(expr))
            {
                if ((expr.NodeType == ExpressionType.MemberAccess) && King.Framework.Linq.TypeHelper.IsNullableType((expr as MemberExpression).Expression.Type))
                {
                    return expr;
                }
                this.Write(" <> 0");
            }
            return expr;
        }

        protected override Expression VisitProjection(ProjectionExpression proj)
        {
            if (!(proj.Projector is ColumnExpression) && !this.forDebug)
            {
                throw new NotSupportedException("Non-scalar projections cannot be translated to SQL.");
            }
            this.Write("(");
            this.WriteLine(Indentation.Inner);
            this.Visit(proj.Select);
            this.Write(")");
            this.Indent(Indentation.Outer);
            return proj;
        }

        protected override Expression VisitRowNumber(RowNumberExpression rowNumber)
        {
            throw new NotSupportedException();
        }

        protected override Expression VisitScalar(ScalarExpression subquery)
        {
            this.Write("(");
            this.WriteLine(Indentation.Inner);
            this.Visit(subquery.Select);
            this.WriteLine(Indentation.Same);
            this.Write(")");
            this.Indent(Indentation.Outer);
            return subquery;
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            int num;
            int count;
            this.AddAliases(select.From);
            this.Write("SELECT ");
            if (select.IsDistinct)
            {
                this.Write("DISTINCT ");
            }
            if (select.Take != null)
            {
                this.WriteTopClause(select.Take);
            }
            this.WriteColumns(select.Columns);
            if (select.From != null)
            {
                this.WriteLine(Indentation.Same);
                this.Write("FROM ");
                this.VisitSource(select.From);
            }
            if (select.Where != null)
            {
                this.WriteLine(Indentation.Same);
                this.Write("WHERE ");
                this.VisitPredicate(select.Where);
            }
            if ((select.GroupBy != null) && (select.GroupBy.Count > 0))
            {
                this.WriteLine(Indentation.Same);
                this.Write("GROUP BY ");
                num = 0;
                count = select.GroupBy.Count;
                while (num < count)
                {
                    if (num > 0)
                    {
                        this.Write(", ");
                    }
                    this.VisitValue(select.GroupBy[num]);
                    num++;
                }
            }
            if ((select.OrderBy != null) && (select.OrderBy.Count > 0))
            {
                this.WriteLine(Indentation.Same);
                this.Write("ORDER BY ");
                num = 0;
                count = select.OrderBy.Count;
                while (num < count)
                {
                    OrderExpression expression = select.OrderBy[num];
                    if (num > 0)
                    {
                        this.Write(", ");
                    }
                    this.VisitValue(expression.Expression);
                    if (expression.OrderType != OrderType.Ascending)
                    {
                        this.Write(" DESC");
                    }
                    num++;
                }
            }
            return select;
        }

        protected override Expression VisitSource(Expression source)
        {
            bool isNested = this.isNested;
            this.isNested = true;
            DbExpressionType nodeType = (DbExpressionType) source.NodeType;
            if (nodeType != DbExpressionType.Table)
            {
                if (nodeType != DbExpressionType.Select)
                {
                    if (nodeType != DbExpressionType.Join)
                    {
                        throw new InvalidOperationException("Select source is not valid type");
                    }
                    this.VisitJoin((JoinExpression) source);
                    goto Label_00EB;
                }
            }
            else
            {
                TableExpression expression = (TableExpression) source;
                this.WriteTableName(expression.Name);
                if (!this.HideTableAliases)
                {
                    this.Write(" ");
                    this.WriteAsAliasName(this.GetAliasName(expression.Alias));
                }
                goto Label_00EB;
            }
            SelectExpression exp = (SelectExpression) source;
            this.Write("(");
            this.WriteLine(Indentation.Inner);
            this.Visit(exp);
            this.WriteLine(Indentation.Same);
            this.Write(") ");
            this.WriteAsAliasName(this.GetAliasName(exp.Alias));
            this.Indent(Indentation.Outer);
        Label_00EB:
            this.isNested = isNested;
            return source;
        }

        protected virtual void VisitStatement(Expression expression)
        {
            ProjectionExpression expression2 = expression as ProjectionExpression;
            if (expression2 != null)
            {
                this.Visit(expression2.Select);
            }
            else
            {
                this.Visit(expression);
            }
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            string @operator = this.GetOperator(u);
            switch (u.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    this.Write(@operator);
                    this.VisitValue(u.Operand);
                    return u;

                case ExpressionType.UnaryPlus:
                    this.VisitValue(u.Operand);
                    return u;

                case ExpressionType.Not:
                    if (!this.IsBoolean(u.Operand.Type) && (@operator.Length <= 1))
                    {
                        this.Write(@operator);
                        this.VisitValue(u.Operand);
                        return u;
                    }
                    this.Write(@operator);
                    this.Write(" ");
                    this.VisitPredicate(u.Operand);
                    return u;

                case ExpressionType.Convert:
                    this.Visit(u.Operand);
                    return u;
            }
            if (!this.forDebug)
            {
                throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }
            this.Write(string.Format("?{0}?", u.NodeType));
            this.Write("(");
            this.Visit(u.Operand);
            this.Write(")");
            return u;
        }

        protected override Expression VisitUpdate(UpdateCommand update)
        {
            this.Write("UPDATE ");
            this.WriteTableName(update.Table.Name);
            this.WriteLine(Indentation.Same);
            bool hideColumnAliases = this.HideColumnAliases;
            this.HideColumnAliases = true;
            this.Write("SET ");
            int num = 0;
            int count = update.Assignments.Count;
            while (num < count)
            {
                ColumnAssignment assignment = update.Assignments[num];
                if (num > 0)
                {
                    this.Write(", ");
                }
                this.Visit(assignment.Column);
                this.Write(" = ");
                this.Visit(assignment.Expression);
                num++;
            }
            if (update.Where != null)
            {
                this.WriteLine(Indentation.Same);
                this.Write("WHERE ");
                this.VisitPredicate(update.Where);
            }
            this.HideColumnAliases = hideColumnAliases;
            return update;
        }

        protected virtual Expression VisitValue(Expression expr)
        {
            return this.Visit(expr);
        }

        protected override Expression VisitVariable(VariableExpression vex)
        {
            this.WriteVariableName(vex.Name);
            return vex;
        }

        protected void Write(object value)
        {
            this.sb.Append(value);
        }

        protected virtual void WriteAggregateName(string aggregateName)
        {
            switch (aggregateName)
            {
                case "Average":
                    this.Write("AVG");
                    return;

                case "LongCount":
                    this.Write("COUNT");
                    return;
            }
            this.Write(aggregateName.ToUpper());
        }

        protected virtual void WriteAliasName(string aliasName)
        {
            this.Write(aliasName);
        }

        protected virtual void WriteAsAliasName(string aliasName)
        {
            this.Write("AS ");
            this.WriteAliasName(aliasName);
        }

        protected virtual void WriteAsColumnName(string columnName)
        {
            this.Write("AS ");
            this.WriteColumnName(columnName);
        }

        protected virtual void WriteColumnName(string columnName)
        {
            string str = (this.Language != null) ? this.Language.Quote(columnName) : columnName;
            this.Write(str);
        }

        protected virtual void WriteColumns(ReadOnlyCollection<ColumnDeclaration> columns)
        {
            if (columns.Count > 0)
            {
                int num = 0;
                int count = columns.Count;
                while (num < count)
                {
                    ColumnDeclaration declaration = columns[num];
                    if (num > 0)
                    {
                        this.Write(", ");
                    }
                    ColumnExpression expression = this.VisitValue(declaration.Expression) as ColumnExpression;
                    if (!(string.IsNullOrEmpty(declaration.Name) || ((expression != null) && !(expression.Name != declaration.Name))))
                    {
                        this.Write(" ");
                        this.WriteAsColumnName(declaration.Name);
                    }
                    num++;
                }
            }
            else
            {
                this.Write("NULL ");
                if (this.isNested)
                {
                    this.WriteAsColumnName("tmp");
                    this.Write(" ");
                }
            }
        }

        protected virtual void WriteConstantDateTime(DateTime value)
        {
            this.Write(string.Format("convert(datetime, '{0:yyyy-MM-ddTHH:mm:ss.fff}', 126)", value));
        }

        protected void WriteLine(Indentation style)
        {
            this.sb.AppendLine();
            this.Indent(style);
            int num = 0;
            int num2 = this.depth * this.indent;
            while (num < num2)
            {
                this.Write(" ");
                num++;
            }
        }

        protected virtual void WriteParameterName(string name)
        {
            this.Write("@" + name);
        }

        protected virtual void WriteTableName(string tableName)
        {
            string str = (this.Language != null) ? this.Language.Quote(tableName) : tableName;
            this.Write(str);
        }

        protected virtual void WriteTopClause(Expression expression)
        {
            this.Write("TOP (");
            this.Visit(expression);
            this.Write(") ");
        }

        protected virtual void WriteValue(object value)
        {
            if (value == null)
            {
                this.Write("NULL");
            }
            else if (value.GetType().IsEnum)
            {
                this.Write(Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType())));
            }
            else
            {
                switch (Type.GetTypeCode(value.GetType()))
                {
                    case TypeCode.Object:
                        throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", value));

                    case TypeCode.Boolean:
                        this.Write(((bool) value) ? 1 : 0);
                        return;

                    case TypeCode.Single:
                    case TypeCode.Double:
                    {
                        string source = value.ToString();
                        if (!source.Contains<char>('.'))
                        {
                            source = source + ".0";
                        }
                        this.Write(source);
                        return;
                    }
                    case TypeCode.DateTime:
                        this.WriteConstantDateTime((DateTime) value);
                        return;

                    case TypeCode.String:
                        this.Write("'");
                        this.Write(value.ToString().Replace("'", "''"));
                        this.Write("'");
                        return;
                }
                this.Write(value);
            }
        }

        protected virtual void WriteVariableName(string name)
        {
            this.WriteParameterName(name);
        }

        protected bool ForDebug
        {
            get
            {
                return this.forDebug;
            }
        }

        protected bool HideColumnAliases
        {
            get
            {
                return this.hideColumnAliases;
            }
            set
            {
                this.hideColumnAliases = value;
            }
        }

        protected bool HideTableAliases
        {
            get
            {
                return this.hideTableAliases;
            }
            set
            {
                this.hideTableAliases = value;
            }
        }

        public int IndentationWidth
        {
            get
            {
                return this.indent;
            }
            set
            {
                this.indent = value;
            }
        }

        protected bool IsNested
        {
            get
            {
                return this.isNested;
            }
            set
            {
                this.isNested = value;
            }
        }

        protected virtual QueryLanguage Language
        {
            get
            {
                return this.language;
            }
        }

        protected enum Indentation
        {
            Same,
            Inner,
            Outer
        }
    }
}
