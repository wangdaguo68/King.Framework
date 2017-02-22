namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public class ComparisonRewriter : DbExpressionVisitor
    {
        private QueryMapping mapping;

        private ComparisonRewriter(QueryMapping mapping)
        {
            this.mapping = mapping;
        }

        protected Expression Compare(BinaryExpression bop)
        {
            Expression expression = this.SkipConvert(bop.Left);
            Expression expression2 = this.SkipConvert(bop.Right);
            EntityExpression expression3 = expression as EntityExpression;
            EntityExpression expression4 = expression2 as EntityExpression;
            bool negate = bop.NodeType == ExpressionType.NotEqual;
            if (expression3 != null)
            {
                return this.MakePredicate(expression, expression2, this.mapping.GetPrimaryKeyMembers(expression3.Entity), negate);
            }
            if (expression4 != null)
            {
                return this.MakePredicate(expression, expression2, this.mapping.GetPrimaryKeyMembers(expression4.Entity), negate);
            }
            IEnumerable<MemberInfo> definedMembers = this.GetDefinedMembers(expression);
            IEnumerable<MemberInfo> members = this.GetDefinedMembers(expression2);
            if ((definedMembers == null) && (members == null))
            {
                return bop;
            }
            if ((definedMembers != null) && (members != null))
            {
                HashSet<string> other = new HashSet<string>(from m in definedMembers select m.Name);
                HashSet<string> set2 = new HashSet<string>(from m in members select m.Name);
                if (other.IsSubsetOf(set2) && set2.IsSubsetOf(other))
                {
                    return this.MakePredicate(expression, expression2, definedMembers, negate);
                }
            }
            else
            {
                if (definedMembers != null)
                {
                    return this.MakePredicate(expression, expression2, definedMembers, negate);
                }
                if (members != null)
                {
                    return this.MakePredicate(expression, expression2, members, negate);
                }
            }
            throw new InvalidOperationException("Cannot compare two constructed types with different sets of members assigned.");
        }

        private static MemberInfo FixMember(MemberInfo member)
        {
            if ((member.MemberType == MemberTypes.Method) && member.Name.StartsWith("get_"))
            {
                return member.DeclaringType.GetProperty(member.Name.Substring(4));
            }
            return member;
        }

        private IEnumerable<MemberInfo> GetDefinedMembers(Expression expr)
        {
            MemberInitExpression expression = expr as MemberInitExpression;
            if (expression != null)
            {
                IEnumerable<MemberInfo> first = from b in expression.Bindings select FixMember(b.Member);
                if (expression.NewExpression.Members != null)
                {
                    first.Concat<MemberInfo>(from m in expression.NewExpression.Members select FixMember(m));
                }
                return first;
            }
            NewExpression expression2 = expr as NewExpression;
            if ((expression2 != null) && (expression2.Members != null))
            {
                return (from m in expression2.Members select FixMember(m));
            }
            return null;
        }

        protected Expression MakePredicate(Expression e1, Expression e2, IEnumerable<MemberInfo> members, bool negate)
        {
            Expression expression = (from m in members select QueryBinder.BindMember(e1, m).Equal(QueryBinder.BindMember(e2, m))).Join(ExpressionType.And);
            if (negate)
            {
                expression = Expression.Not(expression);
            }
            return expression;
        }

        public static Expression Rewrite(QueryMapping mapping, Expression expression)
        {
            return new ComparisonRewriter(mapping).Visit(expression);
        }

        protected Expression SkipConvert(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Convert)
            {
                expression = ((UnaryExpression) expression).Operand;
            }
            return expression;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            switch (b.NodeType)
            {
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                {
                    Expression exp = this.Compare(b);
                    if (exp != b)
                    {
                        return this.Visit(exp);
                    }
                    break;
                }
            }
            return base.VisitBinary(b);
        }
    }
}
