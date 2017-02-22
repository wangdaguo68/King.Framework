namespace King.Framework.Linq.Data.OracleCore
{
    using King.Framework.Linq.Data.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class PLSqlFormatter : SqlFormatter
    {
        protected PLSqlFormatter(QueryLanguage language) : base(language)
        {
        }

        public static string Format(Expression expression)
        {
            return Format(expression, PLSqlLanguage.Default);
        }

        public static string Format(Expression expression, QueryLanguage language)
        {
            PLSqlFormatter formatter = new PLSqlFormatter(language);
            formatter.Visit(expression);
            return formatter.ToString();
        }

        protected override string GetOperator(BinaryExpression b)
        {
            switch (b.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    if (!b.Left.Type.Equals(typeof(string)) && !b.Right.Type.Equals(typeof(string)))
                    {
                        return base.GetOperator(b);
                    }
                    return "||";
            }
            return base.GetOperator(b);
        }

        protected override Expression VisitAggregate(AggregateExpression aggregate)
        {
            string aggregateName = aggregate.AggregateName;
            if ((aggregateName != null) && (aggregateName == "Average"))
            {
                this.WriteTruncMaxDecimalDigitsStart();
                base.VisitAggregate(aggregate);
                this.WriteTruncMaxDecimalDigitsEnd();
            }
            else
            {
                return base.VisitAggregate(aggregate);
            }
            return aggregate;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            switch (b.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    if (!this.IsBoolean(b.Left.Type))
                    {
                        base.Write("BITAND(");
                        this.VisitValue(b.Left);
                        base.Write(",");
                        this.VisitValue(b.Right);
                        base.Write(")");
                        return b;
                    }
                    return base.VisitBinary(b);

                case ExpressionType.Coalesce:
                {
                    base.Write("COALESCE(");
                    this.VisitValue(b.Left);
                    base.Write(", ");
                    Expression right = b.Right;
                    while (right.NodeType == ExpressionType.Coalesce)
                    {
                        BinaryExpression expression2 = (BinaryExpression) right;
                        this.VisitValue(expression2.Left);
                        base.Write(", ");
                        right = expression2.Right;
                    }
                    this.VisitValue(right);
                    base.Write(")");
                    return b;
                }
                case ExpressionType.Divide:
                    if (!this.IsInteger(b.Left.Type) || !this.IsInteger(b.Right.Type))
                    {
                        base.VisitBinary(b);
                        return b;
                    }
                    base.Write("TRUNC(");
                    base.VisitBinary(b);
                    base.Write(")");
                    return b;

                case ExpressionType.ExclusiveOr:
                    base.Write("(");
                    this.VisitValue(b.Left);
                    base.Write("-2*BITAND(");
                    this.VisitValue(b.Left);
                    base.Write(",");
                    this.VisitValue(b.Right);
                    base.Write(")+");
                    this.VisitValue(b.Right);
                    base.Write(")");
                    return b;

                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    if (!this.IsBoolean(b.Left.Type))
                    {
                        base.Write("(");
                        this.VisitValue(b.Left);
                        base.Write("-BITAND(");
                        this.VisitValue(b.Left);
                        base.Write(",");
                        this.VisitValue(b.Right);
                        base.Write(")+");
                        this.VisitValue(b.Right);
                        base.Write(")");
                        return b;
                    }
                    return base.VisitBinary(b);

                case ExpressionType.RightShift:
                    base.Write("(");
                    this.VisitValue(b.Left);
                    base.Write("/POWER(2,");
                    this.VisitValue(b.Right);
                    base.Write("))");
                    return b;

                case ExpressionType.LeftShift:
                    base.Write("(");
                    this.VisitValue(b.Left);
                    base.Write("*POWER(2,");
                    this.VisitValue(b.Right);
                    base.Write("))");
                    return b;

                case ExpressionType.Modulo:
                    base.Write("MOD(");
                    this.VisitValue(b.Left);
                    base.Write(",");
                    this.VisitValue(b.Right);
                    base.Write(")");
                    return b;
            }
            return base.VisitBinary(b);
        }

        protected override Expression VisitConditional(ConditionalExpression c)
        {
            if (this.IsPredicate(c.Test))
            {
                base.Write("(CASE WHEN ");
                this.VisitPredicate(c.Test);
                base.Write(" THEN ");
                this.VisitValue(c.IfTrue);
                Expression ifFalse = c.IfFalse;
                while ((ifFalse != null) && (ifFalse.NodeType == ExpressionType.Conditional))
                {
                    ConditionalExpression expression2 = (ConditionalExpression) ifFalse;
                    base.Write(" WHEN ");
                    this.VisitPredicate(expression2.Test);
                    base.Write(" THEN ");
                    this.VisitValue(expression2.IfTrue);
                    ifFalse = expression2.IfFalse;
                }
                if (ifFalse != null)
                {
                    base.Write(" ELSE ");
                    this.VisitValue(ifFalse);
                }
                base.Write(" END)");
                return c;
            }
            base.Write("(CASE ");
            this.VisitValue(c.Test);
            base.Write(" WHEN 0 THEN ");
            this.VisitValue(c.IfFalse);
            base.Write(" ELSE ");
            this.VisitValue(c.IfTrue);
            base.Write(" END)");
            return c;
        }

        protected override Expression VisitIf(IFCommand ifx)
        {
            if (!this.Language.AllowsMultipleCommands)
            {
                return base.VisitIf(ifx);
            }
            base.Write("IF ");
            this.Visit(ifx.Check);
            base.WriteLine(SqlFormatter.Indentation.Same);
            base.Write("THEN BEGIN");
            base.WriteLine(SqlFormatter.Indentation.Inner);
            this.VisitStatement(ifx.IfTrue);
            base.WriteLine(SqlFormatter.Indentation.Outer);
            if (ifx.IfFalse != null)
            {
                base.Write("ELSE BEGIN");
                base.WriteLine(SqlFormatter.Indentation.Inner);
                this.VisitStatement(ifx.IfFalse);
                base.WriteLine(SqlFormatter.Indentation.Outer);
            }
            base.Write("END IF;");
            return ifx;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Member.DeclaringType == typeof(string))
            {
                string name = m.Member.Name;
                if ((name != null) && (name == "Length"))
                {
                    base.Write("LENGTH(");
                    this.Visit(m.Expression);
                    base.Write(")");
                    return m;
                }
            }
            else if ((m.Member.DeclaringType == typeof(DateTime)) || (m.Member.DeclaringType == typeof(DateTimeOffset)))
            {
                switch (m.Member.Name)
                {
                    case "Day":
                        base.Write("TO_NUMBER(TO_CHAR(");
                        this.Visit(m.Expression);
                        base.Write(",'DD'))");
                        return m;

                    case "Month":
                        base.Write("TO_NUMBER(TO_CHAR(");
                        this.Visit(m.Expression);
                        base.Write(",'MM'))");
                        return m;

                    case "Year":
                        base.Write("TO_NUMBER(TO_CHAR(");
                        this.Visit(m.Expression);
                        base.Write(",'YYYY'))");
                        return m;

                    case "Hour":
                        base.Write("TO_NUMBER(TO_CHAR(");
                        this.Visit(m.Expression);
                        base.Write(",'HH24'))");
                        return m;

                    case "Minute":
                        base.Write("TO_NUMBER(TO_CHAR(");
                        this.Visit(m.Expression);
                        base.Write(",'MI'))");
                        return m;

                    case "Second":
                        base.Write("TO_NUMBER(TO_CHAR(");
                        this.Visit(m.Expression);
                        base.Write(",'SS'))");
                        return m;

                    case "Millisecond":
                        base.Write("TO_NUMBER(TO_CHAR(");
                        this.Visit(m.Expression);
                        base.Write(",'FF'))");
                        return m;

                    case "DayOfWeek":
                        base.Write("(TO_NUMBER(TO_CHAR(");
                        this.Visit(m.Expression);
                        base.Write(",'D'))-1)");
                        return m;

                    case "DayOfYear":
                        base.Write("TO_NUMBER(TO_CHAR(");
                        this.Visit(m.Expression);
                        base.Write(",'DDD'))");
                        return m;
                }
            }
            return base.VisitMemberAccess(m);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (!(m.Method.DeclaringType == typeof(string)))
            {
                if (m.Method.DeclaringType != typeof(DateTime))
                {
                    if (m.Method.DeclaringType == typeof(decimal))
                    {
                        switch (m.Method.Name)
                        {
                            case "Remainder":
                                base.Write("MOD(");
                                this.VisitValue(m.Arguments[0]);
                                base.Write(",");
                                this.VisitValue(m.Arguments[1]);
                                base.Write(")");
                                return m;

                            case "Ceiling":
                                base.Write("CEIL(");
                                this.Visit(m.Arguments[0]);
                                base.Write(")");
                                return m;

                            case "Floor":
                                base.Write("FLOOR(");
                                this.Visit(m.Arguments[0]);
                                base.Write(")");
                                return m;

                            case "Round":
                                if (m.Arguments.Count == 1)
                                {
                                    base.Write("ROUND(");
                                    this.Visit(m.Arguments[0]);
                                    base.Write(", 0)");
                                    return m;
                                }
                                if ((m.Arguments.Count == 2) && (m.Arguments[1].Type == typeof(int)))
                                {
                                    base.Write("ROUND(");
                                    this.Visit(m.Arguments[0]);
                                    base.Write(", ");
                                    this.Visit(m.Arguments[1]);
                                    base.Write(")");
                                    return m;
                                }
                                break;

                            case "Truncate":
                                base.Write("TRUNC(");
                                this.Visit(m.Arguments[0]);
                                base.Write(")");
                                return m;

                            case "Add":
                            case "Subtract":
                            case "Multiply":
                            case "Divide":
                                base.Write("(");
                                this.VisitValue(m.Arguments[0]);
                                base.Write(" ");
                                base.Write(this.GetOperator(m.Method.Name));
                                base.Write(" ");
                                this.VisitValue(m.Arguments[1]);
                                base.Write(")");
                                return m;

                            case "Negate":
                                base.Write("-");
                                this.Visit(m.Arguments[0]);
                                base.Write("");
                                return m;
                        }
                    }
                    else if (m.Method.DeclaringType == typeof(Math))
                    {
                        switch (m.Method.Name)
                        {
                            case "Abs":
                            case "Acos":
                            case "Asin":
                            case "Atan":
                            case "Cos":
                            case "Exp":
                            case "Sin":
                            case "Tan":
                            case "Sqrt":
                            case "Sign":
                                this.WriteTruncMaxDecimalDigitsStart();
                                base.Write(m.Method.Name.ToUpper());
                                base.Write("(");
                                this.Visit(m.Arguments[0]);
                                base.Write(")");
                                this.WriteTruncMaxDecimalDigitsEnd();
                                return m;

                            case "Floor":
                                base.Write("FLOOR(");
                                this.Visit(m.Arguments[0]);
                                base.Write(")");
                                return m;

                            case "Ceiling":
                                base.Write("CEIL(");
                                this.Visit(m.Arguments[0]);
                                base.Write(")");
                                return m;

                            case "Atan2":
                                this.WriteTruncMaxDecimalDigitsStart();
                                base.Write("ATAN2(");
                                this.Visit(m.Arguments[0]);
                                base.Write(", ");
                                this.Visit(m.Arguments[1]);
                                base.Write(")");
                                this.WriteTruncMaxDecimalDigitsEnd();
                                return m;

                            case "Log10":
                                this.WriteTruncMaxDecimalDigitsStart();
                                base.Write("LOG(");
                                base.Write("10,");
                                this.Visit(m.Arguments[0]);
                                base.Write(")");
                                this.WriteTruncMaxDecimalDigitsEnd();
                                return m;

                            case "Log":
                                this.WriteTruncMaxDecimalDigitsStart();
                                base.Write("LN(");
                                this.Visit(m.Arguments[0]);
                                base.Write(")");
                                this.WriteTruncMaxDecimalDigitsEnd();
                                return m;

                            case "Pow":
                                this.WriteTruncMaxDecimalDigitsStart();
                                base.Write("POWER(");
                                this.Visit(m.Arguments[0]);
                                base.Write(", ");
                                this.Visit(m.Arguments[1]);
                                base.Write(")");
                                this.WriteTruncMaxDecimalDigitsEnd();
                                return m;

                            case "Round":
                                if (m.Arguments.Count != 1)
                                {
                                    if ((m.Arguments.Count == 2) && (m.Arguments[1].Type == typeof(int)))
                                    {
                                        base.Write("ROUND(");
                                        this.Visit(m.Arguments[0]);
                                        base.Write(", ");
                                        this.Visit(m.Arguments[1]);
                                        base.Write(")");
                                        return m;
                                    }
                                    break;
                                }
                                base.Write("ROUND(");
                                this.Visit(m.Arguments[0]);
                                base.Write(", 0)");
                                return m;

                            case "Truncate":
                                base.Write("TRUNC(");
                                this.Visit(m.Arguments[0]);
                                base.Write(")");
                                return m;
                        }
                    }
                }
                else
                {
                    switch (m.Method.Name)
                    {
                        case "op_Subtract":
                            if (!(m.Arguments[1].Type == typeof(DateTime)))
                            {
                                break;
                            }
                            base.Write("(");
                            this.Visit(m.Arguments[0]);
                            base.Write("-");
                            this.Visit(m.Arguments[1]);
                            base.Write(")");
                            return m;

                        case "AddYears":
                            base.Write("(");
                            this.Visit(m.Object);
                            base.Write("+");
                            this.Visit(m.Arguments[0]);
                            base.Write("*NUMTOYMINTERVAL(1,'YEAR'))");
                            return m;

                        case "AddMonths":
                            base.Write("(");
                            this.Visit(m.Object);
                            base.Write("+");
                            this.Visit(m.Arguments[0]);
                            base.Write("*NUMTOYMINTERVAL(1,'MONTH'))");
                            return m;

                        case "AddDays":
                            base.Write("(");
                            this.Visit(m.Object);
                            base.Write("+");
                            this.Visit(m.Arguments[0]);
                            base.Write("*NUMTODSINTERVAL(1,'DAY'))");
                            return m;

                        case "AddHours":
                            base.Write("(");
                            this.Visit(m.Object);
                            base.Write("+");
                            this.Visit(m.Arguments[0]);
                            base.Write("*NUMTODSINTERVAL(1,'HOUR'))");
                            return m;

                        case "AddMinutes":
                            base.Write("(");
                            this.Visit(m.Object);
                            base.Write("+");
                            this.Visit(m.Arguments[0]);
                            base.Write("*NUMTODSINTERVAL(1,'MINUTE'))");
                            return m;

                        case "AddSeconds":
                            base.Write("(");
                            this.Visit(m.Object);
                            base.Write("+");
                            this.Visit(m.Arguments[0]);
                            base.Write("*NUMTODSINTERVAL(1,'SECOND'))");
                            return m;

                        case "AddMilliseconds":
                            base.Write("(");
                            this.Visit(m.Object);
                            base.Write("+");
                            this.Visit(m.Arguments[0]);
                            base.Write("*NUMTODSINTERVAL(0.001,'SECOND'))");
                            return m;
                    }
                }
            }
            else
            {
                switch (m.Method.Name)
                {
                    case "StartsWith":
                        base.Write("(");
                        this.Visit(m.Object);
                        base.Write(" LIKE ");
                        this.Visit(m.Arguments[0]);
                        base.Write(" || '%' ESCAPE '/' )");
                        return m;

                    case "EndsWith":
                        base.Write("(");
                        this.Visit(m.Object);
                        base.Write(" LIKE '%' || ");
                        this.Visit(m.Arguments[0]);
                        base.Write(" ESCAPE '/' )");
                        return m;

                    case "Contains":
                        base.Write("(");
                        this.Visit(m.Object);
                        base.Write(" LIKE '%' || ");
                        this.Visit(m.Arguments[0]);
                        base.Write(" || '%' ESCAPE '/' )");
                        return m;

                    case "Concat":
                    {
                        IList<Expression> arguments = m.Arguments;
                        if ((arguments.Count == 1) && (arguments[0].NodeType == ExpressionType.NewArrayInit))
                        {
                            arguments = ((NewArrayExpression) arguments[0]).Expressions;
                        }
                        int num = 0;
                        int count = arguments.Count;
                        while (num < count)
                        {
                            if (num > 0)
                            {
                                base.Write(" || ");
                            }
                            this.Visit(arguments[num]);
                            num++;
                        }
                        return m;
                    }
                    case "IsNullOrEmpty":
                        base.Write("(");
                        this.Visit(m.Arguments[0]);
                        base.Write(" IS NULL)");
                        return m;

                    case "ToUpper":
                        base.Write("UPPER(");
                        this.Visit(m.Object);
                        base.Write(")");
                        return m;

                    case "ToLower":
                        base.Write("LOWER(");
                        this.Visit(m.Object);
                        base.Write(")");
                        return m;

                    case "Replace":
                        base.Write("REPLACE(");
                        this.Visit(m.Object);
                        base.Write(", ");
                        this.Visit(m.Arguments[0]);
                        base.Write(", ");
                        this.Visit(m.Arguments[1]);
                        base.Write(")");
                        return m;

                    case "Substring":
                        base.Write("SUBSTR(");
                        this.Visit(m.Object);
                        base.Write(", ");
                        this.Visit(m.Arguments[0]);
                        base.Write(" + 1, ");
                        if (m.Arguments.Count != 2)
                        {
                            base.Write("8000");
                            break;
                        }
                        this.Visit(m.Arguments[1]);
                        break;

                    case "Remove":
                        base.Write("(SUBSTR(");
                        this.Visit(m.Object);
                        base.Write(",1,");
                        this.Visit(m.Arguments[0]);
                        base.Write(") || SUBSTR(");
                        this.Visit(m.Object);
                        base.Write(",");
                        this.Visit(m.Arguments[0]);
                        base.Write("+1+");
                        if (m.Arguments.Count != 2)
                        {
                            base.Write("8000");
                        }
                        else
                        {
                            this.Visit(m.Arguments[1]);
                        }
                        base.Write("))");
                        return m;

                    case "IndexOf":
                        base.Write("(INSTR(");
                        this.Visit(m.Object);
                        base.Write(", ");
                        this.Visit(m.Arguments[0]);
                        if ((m.Arguments.Count == 2) && (m.Arguments[1].Type == typeof(int)))
                        {
                            base.Write(", ");
                            this.Visit(m.Arguments[1]);
                        }
                        base.Write(") - 1)");
                        return m;

                    case "Trim":
                        base.Write("TRIM(");
                        this.Visit(m.Object);
                        base.Write(")");
                        return m;

                    default:
                        goto Label_1163;
                }
                base.Write(")");
                return m;
            }
        Label_1163:
            if (m.Method.Name == "ToString")
            {
                if (m.Object.Type != typeof(string))
                {
                    base.Write("TO_CHAR(");
                    this.Visit(m.Object);
                    base.Write(")");
                    return m;
                }
                this.Visit(m.Object);
                return m;
            }
            if (((!m.Method.IsStatic && (m.Method.Name == "CompareTo")) && (m.Method.ReturnType == typeof(int))) && (m.Arguments.Count == 1))
            {
                base.Write("(CASE WHEN ");
                this.Visit(m.Object);
                base.Write(" = ");
                this.Visit(m.Arguments[0]);
                base.Write(" THEN 0 WHEN ");
                this.Visit(m.Object);
                base.Write(" < ");
                this.Visit(m.Arguments[0]);
                base.Write(" THEN -1 ELSE 1 END)");
                return m;
            }
            if (((m.Method.IsStatic && (m.Method.Name == "Compare")) && (m.Method.ReturnType == typeof(int))) && (m.Arguments.Count == 2))
            {
                base.Write("(CASE WHEN ");
                this.Visit(m.Arguments[0]);
                base.Write(" = ");
                this.Visit(m.Arguments[1]);
                base.Write(" THEN 0 WHEN ");
                this.Visit(m.Arguments[0]);
                base.Write(" < ");
                this.Visit(m.Arguments[1]);
                base.Write(" THEN -1 ELSE 1 END)");
                return m;
            }
            return base.VisitMethodCall(m);
        }

        protected override NewExpression VisitNew(NewExpression nex)
        {
            if (nex.Constructor.DeclaringType == typeof(DateTime))
            {
                if (nex.Arguments.Count == 3)
                {
                    base.Write("TO_DATE((");
                    this.Visit(nex.Arguments[0]);
                    base.Write(") || '-' || (");
                    this.Visit(nex.Arguments[1]);
                    base.Write(") || '-' || (");
                    this.Visit(nex.Arguments[2]);
                    base.Write("),'YYYY-MM-DD')");
                    return nex;
                }
                if (nex.Arguments.Count == 6)
                {
                    base.Write("TO_TIMESTAMP((");
                    this.Visit(nex.Arguments[0]);
                    base.Write(") || '-' || (");
                    this.Visit(nex.Arguments[1]);
                    base.Write(") || '-' || (");
                    this.Visit(nex.Arguments[2]);
                    base.Write(") || ' ' || (");
                    this.Visit(nex.Arguments[3]);
                    base.Write(") || ':' || (");
                    this.Visit(nex.Arguments[4]);
                    base.Write(") || ':' || (");
                    this.Visit(nex.Arguments[5]);
                    base.Write("),'YYYY-MM-DD HH24:MI:SS')");
                    return nex;
                }
            }
            return base.VisitNew(nex);
        }

        protected override Expression VisitRowNumber(RowNumberExpression rowNumber)
        {
            base.Write("ROW_NUMBER() OVER(");
            if ((rowNumber.OrderBy != null) && (rowNumber.OrderBy.Count > 0))
            {
                base.Write("ORDER BY ");
                int num = 0;
                int count = rowNumber.OrderBy.Count;
                while (num < count)
                {
                    OrderExpression expression = rowNumber.OrderBy[num];
                    if (num > 0)
                    {
                        base.Write(", ");
                    }
                    this.VisitValue(expression.Expression);
                    if (expression.OrderType != OrderType.Ascending)
                    {
                        base.Write(" DESC");
                    }
                    num++;
                }
            }
            base.Write(")");
            return rowNumber;
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            int num;
            int count;
            if (select.Take != null)
            {
                base.Write("SELECT * FROM (");
            }
            this.AddAliases(select.From);
            base.Write("SELECT ");
            if (select.IsDistinct)
            {
                base.Write("DISTINCT ");
            }
            if (select.Take != null)
            {
                this.WriteTopClause(select.Take);
            }
            this.WriteColumns(select.Columns);
            if (select.From != null)
            {
                base.WriteLine(SqlFormatter.Indentation.Same);
                base.Write("FROM ");
                this.VisitSource(select.From);
            }
            else
            {
                base.WriteLine(SqlFormatter.Indentation.Same);
                base.Write("FROM DUAL");
            }
            if (select.Where != null)
            {
                base.WriteLine(SqlFormatter.Indentation.Same);
                base.Write("WHERE ");
                this.VisitPredicate(select.Where);
            }
            if ((select.GroupBy != null) && (select.GroupBy.Count > 0))
            {
                base.WriteLine(SqlFormatter.Indentation.Same);
                base.Write("GROUP BY ");
                num = 0;
                count = select.GroupBy.Count;
                while (num < count)
                {
                    if (num > 0)
                    {
                        base.Write(", ");
                    }
                    this.VisitValue(select.GroupBy[num]);
                    num++;
                }
            }
            if ((select.OrderBy != null) && (select.OrderBy.Count > 0))
            {
                base.WriteLine(SqlFormatter.Indentation.Same);
                base.Write("ORDER BY ");
                num = 0;
                count = select.OrderBy.Count;
                while (num < count)
                {
                    OrderExpression expression = select.OrderBy[num];
                    if (num > 0)
                    {
                        base.Write(", ");
                    }
                    this.VisitValue(expression.Expression);
                    if (expression.OrderType != OrderType.Ascending)
                    {
                        base.Write(" DESC");
                    }
                    num++;
                }
            }
            if (select.Take != null)
            {
                base.Write(") WHERE ROWNUM<=");
                base.Write(select.Take);
            }
            return select;
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            if (u.NodeType == ExpressionType.Not)
            {
                string @operator = this.GetOperator(u);
                if (this.IsBoolean(u.Operand.Type) || (@operator.Length > 1))
                {
                    base.Write(@operator);
                    base.Write(" ");
                    this.VisitPredicate(u.Operand);
                }
                else
                {
                    base.Write("(-1-");
                    this.VisitValue(u.Operand);
                    base.Write(")");
                }
                return u;
            }
            return base.VisitUnary(u);
        }

        protected override Expression VisitValue(Expression expr)
        {
            if (this.IsPredicate(expr))
            {
                base.Write("CASE WHEN (");
                this.Visit(expr);
                base.Write(") THEN 1 ELSE 0 END");
                return expr;
            }
            return base.VisitValue(expr);
        }

        protected override void WriteAsAliasName(string aliasName)
        {
            base.Write(aliasName);
        }

        protected override void WriteAsColumnName(string columnName)
        {
            base.Write(columnName);
        }

        protected override void WriteParameterName(string name)
        {
            base.Write(":");
            base.Write(name);
        }

        protected override void WriteTopClause(Expression expression)
        {
        }

        private void WriteTruncMaxDecimalDigitsEnd()
        {
            base.Write(",");
            base.Write(20);
            base.Write(")");
        }

        private void WriteTruncMaxDecimalDigitsStart()
        {
            base.Write("TRUNC(");
        }
    }
}
