namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Runtime.InteropServices;

    public class Parameterizer : DbExpressionVisitor
    {
        private int iParam = 0;
        private QueryLanguage language;
        private Dictionary<TypeAndValue, NamedValueExpression> map = new Dictionary<TypeAndValue, NamedValueExpression>();
        private Dictionary<HashedExpression, NamedValueExpression> pmap = new Dictionary<HashedExpression, NamedValueExpression>();

        private Parameterizer(QueryLanguage language)
        {
            this.language = language;
        }

        private Expression GetNamedValue(Expression e)
        {
            NamedValueExpression expression;
            HashedExpression key = new HashedExpression(e);
            if (!this.pmap.TryGetValue(key, out expression))
            {
                expression = new NamedValueExpression("p" + this.iParam++, this.language.TypeSystem.GetColumnType(e.Type), e);
                this.pmap.Add(key, expression);
            }
            return expression;
        }

        private static bool IsConstantOrParameter(Expression e)
        {
            return (((e != null) && (e.NodeType == ExpressionType.Constant)) || (e.NodeType == ExpressionType.Parameter));
        }

        private bool IsNumeric(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return true;
            }
            return false;
        }

        public static Expression Parameterize(QueryLanguage language, Expression expression)
        {
            return new Parameterizer(language).Visit(expression);
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            NamedValueExpression expression3;
            ColumnExpression expression4;
            Expression left = this.Visit(b.Left);
            Expression right = this.Visit(b.Right);
            if ((left.NodeType == ((ExpressionType) 0x3f8)) && (right.NodeType == ((ExpressionType) 0x3ea)))
            {
                expression3 = (NamedValueExpression) left;
                expression4 = (ColumnExpression) right;
                left = new NamedValueExpression(expression3.Name, expression4.QueryType, expression3.Value);
            }
            else if ((b.Right.NodeType == ((ExpressionType) 0x3f8)) && (b.Left.NodeType == ((ExpressionType) 0x3ea)))
            {
                expression3 = (NamedValueExpression) right;
                expression4 = (ColumnExpression) left;
                right = new NamedValueExpression(expression3.Name, expression4.QueryType, expression3.Value);
            }
            return base.UpdateBinary(b, left, right, b.Conversion, b.IsLiftedToNull, b.Method);
        }

        protected override ColumnAssignment VisitColumnAssignment(ColumnAssignment ca)
        {
            ca = base.VisitColumnAssignment(ca);
            Expression e = ca.Expression;
            NamedValueExpression expression2 = e as NamedValueExpression;
            if (expression2 != null)
            {
                e = new NamedValueExpression(expression2.Name, ca.Column.QueryType, expression2.Value);
            }
            return base.UpdateColumnAssignment(ca, ca.Column, e);
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (((c.Value == null) || !c.Type.IsArray) && ((c.Value != null) && !this.IsNumeric(c.Value.GetType())))
            {
                NamedValueExpression expression;
                TypeAndValue key = new TypeAndValue(c.Type, c.Value);
                if (!this.map.TryGetValue(key, out expression))
                {
                    expression = new NamedValueExpression("p" + this.iParam++, this.language.TypeSystem.GetColumnType(c.Type), c);
                    this.map.Add(key, expression);
                }
                return expression;
            }
            return c;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            m = (MemberExpression) base.VisitMemberAccess(m);
            NamedValueExpression expression = m.Expression as NamedValueExpression;
            if (expression != null)
            {
                Expression e = Expression.MakeMemberAccess(expression.Value, m.Member);
                return this.GetNamedValue(e);
            }
            return m;
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            return this.GetNamedValue(p);
        }

        protected override Expression VisitProjection(ProjectionExpression proj)
        {
            SelectExpression select = (SelectExpression) this.Visit(proj.Select);
            return base.UpdateProjection(proj, select, proj.Projector, proj.Aggregator);
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            if ((u.NodeType == ExpressionType.Convert) && (u.Operand.NodeType == ExpressionType.ArrayIndex))
            {
                BinaryExpression operand = (BinaryExpression) u.Operand;
                if (IsConstantOrParameter(operand.Left) && IsConstantOrParameter(operand.Right))
                {
                    return this.GetNamedValue(u);
                }
            }
            return base.VisitUnary(u);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HashedExpression : IEquatable<Parameterizer.HashedExpression>
        {
            private Expression expression;
            private int hashCode;
            public HashedExpression(Expression expression)
            {
                this.expression = expression;
                this.hashCode = Hasher.ComputeHash(expression);
            }

            public override bool Equals(object obj)
            {
                return ((obj is Parameterizer.HashedExpression) && this.Equals((Parameterizer.HashedExpression) obj));
            }

            public bool Equals(Parameterizer.HashedExpression other)
            {
                return ((this.hashCode == other.hashCode) && DbExpressionComparer.AreEqual(this.expression, other.expression));
            }

            public override int GetHashCode()
            {
                return this.hashCode;
            }
            private class Hasher : DbExpressionVisitor
            {
                private int hc;

                internal static int ComputeHash(Expression expression)
                {
                    Parameterizer.HashedExpression.Hasher hasher = new Parameterizer.HashedExpression.Hasher();
                    hasher.Visit(expression);
                    return hasher.hc;
                }

                protected override Expression VisitConstant(ConstantExpression c)
                {
                    this.hc += (c.Value != null) ? c.Value.GetHashCode() : 0;
                    return c;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TypeAndValue : IEquatable<Parameterizer.TypeAndValue>
        {
            private Type type;
            private object value;
            private int hash;
            public TypeAndValue(Type type, object value)
            {
                this.type = type;
                this.value = value;
                this.hash = type.GetHashCode() + ((value != null) ? value.GetHashCode() : 0);
            }

            public override bool Equals(object obj)
            {
                return ((obj is Parameterizer.TypeAndValue) && this.Equals((Parameterizer.TypeAndValue) obj));
            }

            public bool Equals(Parameterizer.TypeAndValue vt)
            {
                return ((vt.type == this.type) && object.Equals(vt.value, this.value));
            }

            public override int GetHashCode()
            {
                return this.hash;
            }
        }
    }
}
