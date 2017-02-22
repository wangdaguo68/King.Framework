namespace King.Framework.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;

    public class ExpressionWriter : King.Framework.Linq.ExpressionVisitor
    {
        private int depth;
        private int indent = 2;
        private static readonly char[] special = new char[] { '\n', '\n', '\\' };
        private static readonly char[] splitters = new char[] { '\n', '\r' };
        private TextWriter writer;

        protected ExpressionWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        protected virtual string GetOperator(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";

                case ExpressionType.And:
                    return "&";

                case ExpressionType.AndAlso:
                    return "&&";

                case ExpressionType.Coalesce:
                    return "??";

                case ExpressionType.Divide:
                    return "/";

                case ExpressionType.Equal:
                    return "==";

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

                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";

                case ExpressionType.Not:
                    return "!";

                case ExpressionType.NotEqual:
                    return "!=";

                case ExpressionType.Or:
                    return "|";

                case ExpressionType.OrElse:
                    return "||";

                case ExpressionType.RightShift:
                    return ">>";
            }
            return null;
        }

        protected virtual string GetTypeName(Type type)
        {
            string str = type.Name.Replace('+', '.');
            int index = str.IndexOf('`');
            if (index > 0)
            {
                str = str.Substring(0, index);
            }
            if (!type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                return str;
            }
            StringBuilder builder = new StringBuilder();
            builder.Append(str);
            builder.Append("<");
            Type[] genericArguments = type.GetGenericArguments();
            int num2 = 0;
            int length = genericArguments.Length;
            while (num2 < length)
            {
                if (num2 > 0)
                {
                    builder.Append(",");
                }
                if (type.IsGenericType)
                {
                    builder.Append(this.GetTypeName(genericArguments[num2]));
                }
                num2++;
            }
            builder.Append(">");
            return builder.ToString();
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

        protected override Expression VisitBinary(BinaryExpression b)
        {
            ExpressionType nodeType = b.NodeType;
            if (nodeType != ExpressionType.ArrayIndex)
            {
                if (nodeType == ExpressionType.Power)
                {
                    this.Write("POW(");
                    this.Visit(b.Left);
                    this.Write(", ");
                    this.Visit(b.Right);
                    this.Write(")");
                    return b;
                }
                this.Visit(b.Left);
                this.Write(" ");
                this.Write(this.GetOperator(b.NodeType));
                this.Write(" ");
                this.Visit(b.Right);
                return b;
            }
            this.Visit(b.Left);
            this.Write("[");
            this.Visit(b.Right);
            this.Write("]");
            return b;
        }

        protected override IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            int num = 0;
            int count = original.Count;
            while (num < count)
            {
                this.VisitBinding(original[num]);
                if (num < (count - 1))
                {
                    this.Write(",");
                    this.WriteLine(Indentation.Same);
                }
                num++;
            }
            return original;
        }

        protected override Expression VisitConditional(ConditionalExpression c)
        {
            this.Visit(c.Test);
            this.WriteLine(Indentation.Inner);
            this.Write("? ");
            this.Visit(c.IfTrue);
            this.WriteLine(Indentation.Same);
            this.Write(": ");
            this.Visit(c.IfFalse);
            this.Indent(Indentation.Outer);
            return c;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (c.Value == null)
            {
                this.Write("null");
                return c;
            }
            if (c.Type == typeof(string))
            {
                if (c.Value.ToString().IndexOfAny(special) >= 0)
                {
                    this.Write("@");
                }
                this.Write("\"");
                this.Write(c.Value.ToString());
                this.Write("\"");
                return c;
            }
            if (c.Type == typeof(DateTime))
            {
                this.Write("new DateTime(\"");
                this.Write(c.Value.ToString());
                this.Write("\")");
                return c;
            }
            if (c.Type.IsArray)
            {
                Type elementType = c.Type.GetElementType();
                this.VisitNewArray(Expression.NewArrayInit(elementType, (IEnumerable<Expression>) (from v in ((IEnumerable) c.Value).OfType<object>() select Expression.Constant(v, elementType))));
                return c;
            }
            this.Write(c.Value.ToString());
            return c;
        }

        protected override ElementInit VisitElementInitializer(ElementInit initializer)
        {
            if (initializer.Arguments.Count > 1)
            {
                this.Write("{");
                int num = 0;
                int count = initializer.Arguments.Count;
                while (num < count)
                {
                    this.Visit(initializer.Arguments[num]);
                    if (num < (count - 1))
                    {
                        this.Write(", ");
                    }
                    num++;
                }
                this.Write("}");
                return initializer;
            }
            this.Visit(initializer.Arguments[0]);
            return initializer;
        }

        protected override IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            int num = 0;
            int count = original.Count;
            while (num < count)
            {
                this.VisitElementInitializer(original[num]);
                if (num < (count - 1))
                {
                    this.Write(",");
                    this.WriteLine(Indentation.Same);
                }
                num++;
            }
            return original;
        }

        protected override ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            int num = 0;
            int count = original.Count;
            while (num < count)
            {
                this.Visit(original[num]);
                if (num < (count - 1))
                {
                    this.Write(",");
                    this.WriteLine(Indentation.Same);
                }
                num++;
            }
            return original;
        }

        protected override Expression VisitInvocation(InvocationExpression iv)
        {
            this.Write("Invoke(");
            this.WriteLine(Indentation.Inner);
            this.VisitExpressionList(iv.Arguments);
            this.Write(", ");
            this.WriteLine(Indentation.Same);
            this.Visit(iv.Expression);
            this.WriteLine(Indentation.Same);
            this.Write(")");
            this.Indent(Indentation.Outer);
            return iv;
        }

        protected override Expression VisitLambda(LambdaExpression lambda)
        {
            if (lambda.Parameters.Count != 1)
            {
                this.Write("(");
                int num = 0;
                int count = lambda.Parameters.Count;
                while (num < count)
                {
                    this.Write(lambda.Parameters[num].Name);
                    if (num < (count - 1))
                    {
                        this.Write(", ");
                    }
                    num++;
                }
                this.Write(")");
            }
            else
            {
                this.Write(lambda.Parameters[0].Name);
            }
            this.Write(" => ");
            this.Visit(lambda.Body);
            return lambda;
        }

        protected override Expression VisitListInit(ListInitExpression init)
        {
            this.Visit(init.NewExpression);
            this.Write(" {");
            this.WriteLine(Indentation.Inner);
            this.VisitElementInitializerList(init.Initializers);
            this.WriteLine(Indentation.Outer);
            this.Write("}");
            return init;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            this.Visit(m.Expression);
            this.Write(".");
            this.Write(m.Member.Name);
            return m;
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            this.Write(assignment.Member.Name);
            this.Write(" = ");
            this.Visit(assignment.Expression);
            return assignment;
        }

        protected override Expression VisitMemberInit(MemberInitExpression init)
        {
            this.Visit(init.NewExpression);
            this.Write(" {");
            this.WriteLine(Indentation.Inner);
            this.VisitBindingList(init.Bindings);
            this.WriteLine(Indentation.Outer);
            this.Write("}");
            return init;
        }

        protected override MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            this.Write(binding.Member.Name);
            this.Write(" = {");
            this.WriteLine(Indentation.Inner);
            this.VisitElementInitializerList(binding.Initializers);
            this.WriteLine(Indentation.Outer);
            this.Write("}");
            return binding;
        }

        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            this.Write(binding.Member.Name);
            this.Write(" = {");
            this.WriteLine(Indentation.Inner);
            this.VisitBindingList(binding.Bindings);
            this.WriteLine(Indentation.Outer);
            this.Write("}");
            return binding;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Object != null)
            {
                this.Visit(m.Object);
            }
            else
            {
                this.Write(this.GetTypeName(m.Method.DeclaringType));
            }
            this.Write(".");
            this.Write(m.Method.Name);
            this.Write("(");
            if (m.Arguments.Count > 1)
            {
                this.WriteLine(Indentation.Inner);
            }
            this.VisitExpressionList(m.Arguments);
            if (m.Arguments.Count > 1)
            {
                this.WriteLine(Indentation.Outer);
            }
            this.Write(")");
            return m;
        }

        protected override NewExpression VisitNew(NewExpression nex)
        {
            this.Write("new ");
            this.Write(this.GetTypeName(nex.Constructor.DeclaringType));
            this.Write("(");
            if (nex.Arguments.Count > 1)
            {
                this.WriteLine(Indentation.Inner);
            }
            this.VisitExpressionList(nex.Arguments);
            if (nex.Arguments.Count > 1)
            {
                this.WriteLine(Indentation.Outer);
            }
            this.Write(")");
            return nex;
        }

        protected override Expression VisitNewArray(NewArrayExpression na)
        {
            this.Write("new ");
            this.Write(this.GetTypeName(King.Framework.Linq.TypeHelper.GetElementType(na.Type)));
            this.Write("[] {");
            if (na.Expressions.Count > 1)
            {
                this.WriteLine(Indentation.Inner);
            }
            this.VisitExpressionList(na.Expressions);
            if (na.Expressions.Count > 1)
            {
                this.WriteLine(Indentation.Outer);
            }
            this.Write("}");
            return na;
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            this.Write(p.Name);
            return p;
        }

        protected override Expression VisitTypeIs(TypeBinaryExpression b)
        {
            this.Visit(b.Expression);
            this.Write(" is ");
            this.Write(this.GetTypeName(b.TypeOperand));
            return b;
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.UnaryPlus:
                    this.Visit(u.Operand);
                    return u;

                case ExpressionType.Quote:
                    this.Visit(u.Operand);
                    return u;

                case ExpressionType.TypeAs:
                    this.Visit(u.Operand);
                    this.Write(" as ");
                    this.Write(this.GetTypeName(u.Type));
                    return u;

                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    this.Write("((");
                    this.Write(this.GetTypeName(u.Type));
                    this.Write(")");
                    this.Visit(u.Operand);
                    this.Write(")");
                    return u;

                case ExpressionType.ArrayLength:
                    this.Visit(u.Operand);
                    this.Write(".Length");
                    return u;
            }
            this.Write(this.GetOperator(u.NodeType));
            this.Visit(u.Operand);
            return u;
        }

        protected override Expression VisitUnknown(Expression expression)
        {
            this.Write(expression.ToString());
            return expression;
        }

        protected void Write(string text)
        {
            if (text.IndexOf('\n') >= 0)
            {
                string[] strArray = text.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
                int index = 0;
                int length = strArray.Length;
                while (index < length)
                {
                    this.Write(strArray[index]);
                    if (index < (length - 1))
                    {
                        this.WriteLine(Indentation.Same);
                    }
                    index++;
                }
            }
            else
            {
                this.writer.Write(text);
            }
        }

        public static void Write(TextWriter writer, Expression expression)
        {
            new ExpressionWriter(writer).Visit(expression);
        }

        protected void WriteLine(Indentation style)
        {
            this.writer.WriteLine();
            this.Indent(style);
            int num = 0;
            int num2 = this.depth * this.indent;
            while (num < num2)
            {
                this.writer.Write(" ");
                num++;
            }
        }

        public static string WriteToString(Expression expression)
        {
            StringWriter writer = new StringWriter();
            Write(writer, expression);
            return writer.ToString();
        }

        protected int IndentationWidth
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

        protected enum Indentation
        {
            Same,
            Inner,
            Outer
        }
    }
}
