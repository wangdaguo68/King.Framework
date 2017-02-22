namespace King.Framework.Linq.Data.SqlClient
{
    using King.Framework.Linq.Data.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class TSqlFormatter : SqlFormatter
    {
        protected TSqlFormatter(QueryLanguage language) : base(language)
        {
        }

        public static string Format(Expression expression)
        {
            return Format(expression, new TSqlLanguage());
        }

        public static string Format(Expression expression, QueryLanguage language)
        {
            TSqlFormatter formatter = new TSqlFormatter(language);
            formatter.Visit(expression);
            return formatter.ToString();
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            if (b.NodeType == ExpressionType.Power)
            {
                base.Write("POWER(");
                this.VisitValue(b.Left);
                base.Write(", ");
                this.VisitValue(b.Right);
                base.Write(")");
                return b;
            }
            if (b.NodeType == ExpressionType.Coalesce)
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
            if (b.NodeType == ExpressionType.LeftShift)
            {
                base.Write("(");
                this.VisitValue(b.Left);
                base.Write(" * POWER(2, ");
                this.VisitValue(b.Right);
                base.Write("))");
                return b;
            }
            if (b.NodeType == ExpressionType.RightShift)
            {
                base.Write("(");
                this.VisitValue(b.Left);
                base.Write(" / POWER(2, ");
                this.VisitValue(b.Right);
                base.Write("))");
                return b;
            }
            return base.VisitBinary(b);
        }

        protected override Expression VisitBlock(BlockCommand block)
        {
            if (!this.Language.AllowsMultipleCommands)
            {
                return base.VisitBlock(block);
            }
            int num = 0;
            int count = block.Commands.Count;
            while (num < count)
            {
                if (num > 0)
                {
                    base.WriteLine(SqlFormatter.Indentation.Same);
                    base.WriteLine(SqlFormatter.Indentation.Same);
                }
                this.VisitStatement(block.Commands[num]);
                num++;
            }
            return block;
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

        protected override Expression VisitDeclaration(DeclarationCommand decl)
        {
            VariableDeclaration declaration;
            if (!this.Language.AllowsMultipleCommands)
            {
                return base.VisitDeclaration(decl);
            }
            int num = 0;
            int count = decl.Variables.Count;
            while (num < count)
            {
                declaration = decl.Variables[num];
                if (num > 0)
                {
                    base.WriteLine(SqlFormatter.Indentation.Same);
                }
                base.Write("DECLARE @");
                base.Write(declaration.Name);
                base.Write(" ");
                base.Write(this.Language.TypeSystem.GetVariableDeclaration(declaration.QueryType, false));
                num++;
            }
            if (decl.Source != null)
            {
                base.WriteLine(SqlFormatter.Indentation.Same);
                base.Write("SELECT ");
                num = 0;
                count = decl.Variables.Count;
                while (num < count)
                {
                    if (num > 0)
                    {
                        base.Write(", ");
                    }
                    base.Write("@");
                    base.Write(decl.Variables[num].Name);
                    base.Write(" = ");
                    this.Visit(decl.Source.Columns[num].Expression);
                    num++;
                }
                if (decl.Source.From != null)
                {
                    base.WriteLine(SqlFormatter.Indentation.Same);
                    base.Write("FROM ");
                    this.VisitSource(decl.Source.From);
                }
                if (decl.Source.Where != null)
                {
                    base.WriteLine(SqlFormatter.Indentation.Same);
                    base.Write("WHERE ");
                    this.Visit(decl.Source.Where);
                }
                return decl;
            }
            num = 0;
            count = decl.Variables.Count;
            while (num < count)
            {
                declaration = decl.Variables[num];
                if (declaration.Expression != null)
                {
                    base.WriteLine(SqlFormatter.Indentation.Same);
                    base.Write("SET @");
                    base.Write(declaration.Name);
                    base.Write(" = ");
                    this.Visit(declaration.Expression);
                }
                num++;
            }
            return decl;
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
            base.Write("BEGIN");
            base.WriteLine(SqlFormatter.Indentation.Inner);
            this.VisitStatement(ifx.IfTrue);
            base.WriteLine(SqlFormatter.Indentation.Outer);
            if (ifx.IfFalse != null)
            {
                base.Write("END ELSE BEGIN");
                base.WriteLine(SqlFormatter.Indentation.Inner);
                this.VisitStatement(ifx.IfFalse);
                base.WriteLine(SqlFormatter.Indentation.Outer);
            }
            base.Write("END");
            return ifx;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Member.DeclaringType == typeof(string))
            {
                string name = m.Member.Name;
                if ((name != null) && (name == "Length"))
                {
                    base.Write("LEN(");
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
                        base.Write("DAY(");
                        this.Visit(m.Expression);
                        base.Write(")");
                        return m;

                    case "Month":
                        base.Write("MONTH(");
                        this.Visit(m.Expression);
                        base.Write(")");
                        return m;

                    case "Year":
                        base.Write("YEAR(");
                        this.Visit(m.Expression);
                        base.Write(")");
                        return m;

                    case "Hour":
                        base.Write("DATEPART(hour, ");
                        this.Visit(m.Expression);
                        base.Write(")");
                        return m;

                    case "Minute":
                        base.Write("DATEPART(minute, ");
                        this.Visit(m.Expression);
                        base.Write(")");
                        return m;

                    case "Second":
                        base.Write("DATEPART(second, ");
                        this.Visit(m.Expression);
                        base.Write(")");
                        return m;

                    case "Millisecond":
                        base.Write("DATEPART(millisecond, ");
                        this.Visit(m.Expression);
                        base.Write(")");
                        return m;

                    case "DayOfWeek":
                        base.Write("(DATEPART(weekday, ");
                        this.Visit(m.Expression);
                        base.Write(") - 1)");
                        return m;

                    case "DayOfYear":
                        base.Write("(DATEPART(dayofyear, ");
                        this.Visit(m.Expression);
                        base.Write(") - 1)");
                        return m;
                }
            }
            return base.VisitMemberAccess(m);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (!(m.Method.DeclaringType == typeof(string)))
            {
                if (m.Method.DeclaringType == typeof(DateTime))
                {
                    switch (m.Method.Name)
                    {
                        case "op_Subtract":
                            if (!(m.Arguments[1].Type == typeof(DateTime)))
                            {
                                break;
                            }
                            base.Write("DATEDIFF(");
                            this.Visit(m.Arguments[0]);
                            base.Write(", ");
                            this.Visit(m.Arguments[1]);
                            base.Write(")");
                            return m;

                        case "AddYears":
                            base.Write("DATEADD(YYYY,");
                            this.Visit(m.Arguments[0]);
                            base.Write(",");
                            this.Visit(m.Object);
                            base.Write(")");
                            return m;

                        case "AddMonths":
                            base.Write("DATEADD(MM,");
                            this.Visit(m.Arguments[0]);
                            base.Write(",");
                            this.Visit(m.Object);
                            base.Write(")");
                            return m;

                        case "AddDays":
                            base.Write("DATEADD(DAY,");
                            this.Visit(m.Arguments[0]);
                            base.Write(",");
                            this.Visit(m.Object);
                            base.Write(")");
                            return m;

                        case "AddHours":
                            base.Write("DATEADD(HH,");
                            this.Visit(m.Arguments[0]);
                            base.Write(",");
                            this.Visit(m.Object);
                            base.Write(")");
                            return m;

                        case "AddMinutes":
                            base.Write("DATEADD(MI,");
                            this.Visit(m.Arguments[0]);
                            base.Write(",");
                            this.Visit(m.Object);
                            base.Write(")");
                            return m;

                        case "AddSeconds":
                            base.Write("DATEADD(SS,");
                            this.Visit(m.Arguments[0]);
                            base.Write(",");
                            this.Visit(m.Object);
                            base.Write(")");
                            return m;

                        case "AddMilliseconds":
                            base.Write("DATEADD(MS,");
                            this.Visit(m.Arguments[0]);
                            base.Write(",");
                            this.Visit(m.Object);
                            base.Write(")");
                            return m;
                    }
                }
                else if (!(m.Method.DeclaringType == typeof(decimal)))
                {
                    if (m.Method.DeclaringType == typeof(Math))
                    {
                        switch (m.Method.Name)
                        {
                            case "Abs":
                            case "Acos":
                            case "Asin":
                            case "Atan":
                            case "Cos":
                            case "Exp":
                            case "Log10":
                            case "Sin":
                            case "Tan":
                            case "Sqrt":
                            case "Sign":
                            case "Ceiling":
                            case "Floor":
                                goto Label_0DFF;

                            case "Atan2":
                                base.Write("ATN2(");
                                this.Visit(m.Arguments[0]);
                                base.Write(", ");
                                this.Visit(m.Arguments[1]);
                                base.Write(")");
                                return m;

                            case "Log":
                                if (m.Arguments.Count != 1)
                                {
                                    break;
                                }
                                goto Label_0DFF;

                            case "Pow":
                                base.Write("POWER(");
                                this.Visit(m.Arguments[0]);
                                base.Write(", ");
                                this.Visit(m.Arguments[1]);
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
                                base.Write("ROUND(");
                                this.Visit(m.Arguments[0]);
                                base.Write(", 0, 1)");
                                return m;
                        }
                    }
                }
                else
                {
                    switch (m.Method.Name)
                    {
                        case "Add":
                        case "Subtract":
                        case "Multiply":
                        case "Divide":
                        case "Remainder":
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

                        case "Ceiling":
                        case "Floor":
                            base.Write(m.Method.Name.ToUpper());
                            base.Write("(");
                            this.Visit(m.Arguments[0]);
                            base.Write(")");
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
                            base.Write("ROUND(");
                            this.Visit(m.Arguments[0]);
                            base.Write(", 0, 1)");
                            return m;
                    }
                }
                goto Label_1018;
            }
            switch (m.Method.Name)
            {
                case "StartsWith":
                    base.Write("(");
                    this.Visit(m.Object);
                    base.Write(" LIKE ");
                    this.Visit(m.Arguments[0]);
                    base.Write(" + '%' ESCAPE '/' )");
                    return m;

                case "EndsWith":
                    base.Write("(");
                    this.Visit(m.Object);
                    base.Write(" LIKE '%' + ");
                    this.Visit(m.Arguments[0]);
                    base.Write(" ESCAPE '/' )");
                    return m;

                case "Contains":
                    base.Write("(");
                    this.Visit(m.Object);
                    base.Write(" LIKE '%' + ");
                    this.Visit(m.Arguments[0]);
                    base.Write(" + '%' ESCAPE '/' )");
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
                            base.Write(" + ");
                        }
                        this.Visit(arguments[num]);
                        num++;
                    }
                    return m;
                }
                case "IsNullOrEmpty":
                    base.Write("(");
                    this.Visit(m.Arguments[0]);
                    base.Write(" IS NULL OR ");
                    this.Visit(m.Arguments[0]);
                    base.Write(" = '')");
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
                    base.Write("SUBSTRING(");
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
                    base.Write("STUFF(");
                    this.Visit(m.Object);
                    base.Write(", ");
                    this.Visit(m.Arguments[0]);
                    base.Write(" + 1, ");
                    if (m.Arguments.Count != 2)
                    {
                        base.Write("8000");
                    }
                    else
                    {
                        this.Visit(m.Arguments[1]);
                    }
                    base.Write(", '')");
                    return m;

                case "IndexOf":
                    base.Write("(CHARINDEX(");
                    this.Visit(m.Arguments[0]);
                    base.Write(", ");
                    this.Visit(m.Object);
                    if ((m.Arguments.Count == 2) && (m.Arguments[1].Type == typeof(int)))
                    {
                        base.Write(", ");
                        this.Visit(m.Arguments[1]);
                        base.Write(" + 1");
                    }
                    base.Write(") - 1)");
                    return m;

                case "Trim":
                    base.Write("RTRIM(LTRIM(");
                    this.Visit(m.Object);
                    base.Write("))");
                    return m;

                default:
                    goto Label_1018;
            }
            base.Write(")");
            return m;
        Label_0DFF:
            base.Write(m.Method.Name.ToUpper());
            base.Write("(");
            this.Visit(m.Arguments[0]);
            base.Write(")");
            return m;
        Label_1018:
            if (m.Method.Name == "ToString")
            {
                if (m.Object.Type != typeof(string))
                {
                    base.Write("CONVERT(NVARCHAR, ");
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
                    base.Write("Convert(DateTime, ");
                    base.Write("Convert(nvarchar, ");
                    this.Visit(nex.Arguments[0]);
                    base.Write(") + '/' + ");
                    base.Write("Convert(nvarchar, ");
                    this.Visit(nex.Arguments[1]);
                    base.Write(") + '/' + ");
                    base.Write("Convert(nvarchar, ");
                    this.Visit(nex.Arguments[2]);
                    base.Write("))");
                    return nex;
                }
                if (nex.Arguments.Count == 6)
                {
                    base.Write("Convert(DateTime, ");
                    base.Write("Convert(nvarchar, ");
                    this.Visit(nex.Arguments[0]);
                    base.Write(") + '/' + ");
                    base.Write("Convert(nvarchar, ");
                    this.Visit(nex.Arguments[1]);
                    base.Write(") + '/' + ");
                    base.Write("Convert(nvarchar, ");
                    this.Visit(nex.Arguments[2]);
                    base.Write(") + ' ' + ");
                    base.Write("Convert(nvarchar, ");
                    this.Visit(nex.Arguments[3]);
                    base.Write(") + ':' + ");
                    base.Write("Convert(nvarchar, ");
                    this.Visit(nex.Arguments[4]);
                    base.Write(") + ':' + ");
                    base.Write("Convert(nvarchar, ");
                    this.Visit(nex.Arguments[5]);
                    base.Write("))");
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

        protected override void WriteAggregateName(string aggregateName)
        {
            if (aggregateName == "LongCount")
            {
                base.Write("COUNT_BIG");
            }
            else
            {
                base.WriteAggregateName(aggregateName);
            }
        }
    }
}
