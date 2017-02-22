using System;

namespace King.Framework.Common
{
    using System.Linq.Expressions;
    using System.Reflection;

    public sealed class MemberVisitor : ExpressionVisitor
    {
        private MemberInfo member;

        public static MemberInfo GetMember<T>(Expression<Func<T, object>> expr)
        {
            return InternalGetMember(expr);
        }

        public static MemberInfo GetMember<T1, T2>(Expression<Func<T1, T2>> expr)
        {
            return InternalGetMember(expr);
        }

        public static PropertyInfo GetProperty<T>(Expression<Func<T, object>> expr)
        {
            return (InternalGetMember(expr) as PropertyInfo);
        }

        private static MemberInfo InternalGetMember(Expression expr)
        {
            MemberVisitor visitor = new MemberVisitor();
            visitor.Visit(expr);
            return visitor.member;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            this.member = node.Member;
            return node;
        }
    }
}
