using System.Reflection;

namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Linq.Expressions;

    public class RelationshipIncluder : DbExpressionVisitor
    {
        private ScopedDictionary<MemberInfo, bool> includeScope = new ScopedDictionary<MemberInfo, bool>(null);
        private QueryMapper mapper;
        private QueryPolicy policy;

        private RelationshipIncluder(QueryMapper mapper)
        {
            this.mapper = mapper;
            this.policy = mapper.Translator.Police.Policy;
        }

        public static Expression Include(QueryMapper mapper, Expression expression)
        {
            return new RelationshipIncluder(mapper).Visit(expression);
        }

        protected override Expression VisitEntity(EntityExpression entity)
        {
            Func<MemberInfo, bool> fnIsIncluded = null;
            Expression expression;
            ScopedDictionary<MemberInfo, bool> includeScope = this.includeScope;
            this.includeScope = new ScopedDictionary<MemberInfo, bool>(this.includeScope);
            try
            {
                if (this.mapper.HasIncludedMembers(entity))
                {
                    if (fnIsIncluded == null)
                    {
                        fnIsIncluded = delegate (MemberInfo m) {
                            if (!this.includeScope.ContainsKey(m) && this.policy.IsIncluded(m))
                            {
                                this.includeScope.Add(m, true);
                                return true;
                            }
                            return false;
                        };
                    }
                    entity = this.mapper.IncludeMembers(entity, fnIsIncluded);
                }
                expression = base.VisitEntity(entity);
            }
            finally
            {
                this.includeScope = includeScope;
            }
            return expression;
        }

        protected override Expression VisitProjection(ProjectionExpression proj)
        {
            Expression projector = this.Visit(proj.Projector);
            return base.UpdateProjection(proj, proj.Select, projector, proj.Aggregator);
        }
    }
}
