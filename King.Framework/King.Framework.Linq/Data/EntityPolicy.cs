using System.Collections;

namespace King.Framework.Linq.Data
{
    using King.Framework.Linq;
    using King.Framework.Linq.Data.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    public class EntityPolicy : QueryPolicy
    {
        private HashSet<MemberInfo> deferred = new HashSet<MemberInfo>();
        private HashSet<MemberInfo> included = new HashSet<MemberInfo>();
        private Dictionary<MemberInfo, List<LambdaExpression>> operations = new Dictionary<MemberInfo, List<LambdaExpression>>();

        private void AddOperation(MemberInfo member, LambdaExpression operation)
        {
            List<LambdaExpression> list;
            if (!this.operations.TryGetValue(member, out list))
            {
                list = new List<LambdaExpression>();
                this.operations.Add(member, list);
            }
            list.Add(operation);
        }

        public void Apply<TEntity>(Expression<Func<IEnumerable<TEntity>, IEnumerable<TEntity>>> fnApply)
        {
            this.Apply(fnApply);
        }

        public void Apply(LambdaExpression fnApply)
        {
            if (fnApply == null)
            {
                throw new ArgumentNullException("fnApply");
            }
            if (fnApply.Parameters.Count != 1)
            {
                throw new ArgumentException("Apply function has wrong number of arguments.");
            }
            this.AddOperation(TypeHelper.GetElementType(fnApply.Parameters[0].Type), fnApply);
        }

        public void AssociateWith<TEntity>(Expression<Func<TEntity, IEnumerable>> memberQuery)
        {
            this.AssociateWith(memberQuery);
        }

        public void AssociateWith(LambdaExpression memberQuery)
        {
            MemberExpression searchFor = RootMemberFinder.Find(memberQuery, memberQuery.Parameters[0]);
            if (searchFor == null)
            {
                throw new InvalidOperationException("Subquery does not originate with a member access");
            }
            if (searchFor != memberQuery.Body)
            {
                ParameterExpression replaceWith = Expression.Parameter(searchFor.Type, "root");
                Expression body = ExpressionReplacer.Replace(memberQuery.Body, searchFor, replaceWith);
                this.AddOperation(searchFor.Member, Expression.Lambda(body, new ParameterExpression[] { replaceWith }));
            }
        }

        public override QueryPolice CreatePolice(QueryTranslator translator)
        {
            return new Police(this, translator);
        }

        private void Defer(MemberInfo member)
        {
            Type memberType = TypeHelper.GetMemberType(member);
            if (memberType.IsGenericType)
            {
                Type genericTypeDefinition = memberType.GetGenericTypeDefinition();
                if (!((!(genericTypeDefinition != typeof(IEnumerable<>)) || !(genericTypeDefinition != typeof(IList<>))) || typeof(IDeferLoadable).IsAssignableFrom(memberType)))
                {
                    throw new InvalidOperationException(string.Format("The member '{0}' cannot be deferred due to its type.", member));
                }
            }
            this.deferred.Add(member);
        }

        public void Include(MemberInfo member)
        {
            this.Include(member, false);
        }

        public void Include(MemberInfo member, bool deferLoad)
        {
            this.included.Add(member);
            if (deferLoad)
            {
                this.Defer(member);
            }
        }

        public void IncludeWith<TEntity>(Expression<Func<TEntity, object>> fnMember)
        {
            this.IncludeWith(fnMember, false);
        }

        public void IncludeWith(LambdaExpression fnMember)
        {
            this.IncludeWith(fnMember, false);
        }

        public void IncludeWith<TEntity>(Expression<Func<TEntity, object>> fnMember, bool deferLoad)
        {
            this.IncludeWith(fnMember, deferLoad);
        }

        public void IncludeWith(LambdaExpression fnMember, bool deferLoad)
        {
            MemberExpression expression = RootMemberFinder.Find(fnMember, fnMember.Parameters[0]);
            if (expression == null)
            {
                throw new InvalidOperationException("Subquery does not originate with a member access");
            }
            this.Include(expression.Member, deferLoad);
            if (expression != fnMember.Body)
            {
                this.AssociateWith(fnMember);
            }
        }

        public override bool IsDeferLoaded(MemberInfo member)
        {
            return this.deferred.Contains(member);
        }

        public override bool IsIncluded(MemberInfo member)
        {
            return this.included.Contains(member);
        }

        private class Police : QueryPolice
        {
            private EntityPolicy policy;

            public Police(EntityPolicy policy, QueryTranslator translator) : base(policy, translator)
            {
                this.policy = policy;
            }

            public override Expression ApplyPolicy(Expression expression, MemberInfo member)
            {
                List<LambdaExpression> list;
                if (this.policy.operations.TryGetValue(member, out list))
                {
                    Expression expression2 = expression;
                    foreach (LambdaExpression expression3 in list)
                    {
                        QueryMapping mapping = base.Translator.Mapper.Mapping;
                        Expression expression4 = PartialEvaluator.Eval(expression3, new Func<Expression, bool>(mapping.CanBeEvaluatedLocally));
                        expression2 = base.Translator.Mapper.ApplyMapping(Expression.Invoke(expression4, new Expression[] { expression2 }));
                    }
                    ProjectionExpression expression5 = (ProjectionExpression)expression2;
                    if (expression5.Type != expression.Type)
                    {
                        LambdaExpression aggregator = Aggregator.GetAggregator(expression.Type, expression5.Type);
                        expression5 = new ProjectionExpression(expression5.Select, expression5.Projector, aggregator);
                    }
                    return expression5;
                }
                return expression;
            }
        }

        private class RootMemberFinder : King.Framework.Linq.ExpressionVisitor
        {
            private MemberExpression found;
            private ParameterExpression parameter;

            private RootMemberFinder(ParameterExpression parameter)
            {
                this.parameter = parameter;
            }

            public static MemberExpression Find(Expression query, ParameterExpression parameter)
            {
                EntityPolicy.RootMemberFinder finder = new EntityPolicy.RootMemberFinder(parameter);
                finder.Visit(query);
                return finder.found;
            }

            protected override Expression VisitMemberAccess(MemberExpression m)
            {
                if (m.Expression == this.parameter)
                {
                    this.found = m;
                    return m;
                }
                return base.VisitMemberAccess(m);
            }

            protected override Expression VisitMethodCall(MethodCallExpression m)
            {
                if (m.Object != null)
                {
                    this.Visit(m.Object);
                    return m;
                }
                if (m.Arguments.Count > 0)
                {
                    this.Visit(m.Arguments[0]);
                }
                return m;
            }
        }
    }
}
