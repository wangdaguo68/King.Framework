namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;

    public class RelationshipBinder : DbExpressionVisitor
    {
        private Expression currentFrom;
        private QueryLanguage language;
        private QueryMapper mapper;
        private QueryMapping mapping;

        private RelationshipBinder(QueryMapper mapper)
        {
            this.mapper = mapper;
            this.mapping = mapper.Mapping;
            this.language = mapper.Translator.Linguist.Language;
        }

        public static Expression Bind(QueryMapper mapper, Expression expression)
        {
            return new RelationshipBinder(mapper).Visit(expression);
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            Expression root = this.Visit(m.Expression);
            EntityExpression expression2 = root as EntityExpression;
            if ((expression2 != null) && this.mapping.IsRelationship(expression2.Entity, m.Member))
            {
                ProjectionExpression proj = (ProjectionExpression) this.Visit(this.mapper.GetMemberExpression(root, expression2.Entity, m.Member));
                if ((this.currentFrom != null) && this.mapping.IsSingletonRelationship(expression2.Entity, m.Member))
                {
                    proj = this.language.AddOuterJoinTest(proj);
                    Expression expression4 = new JoinExpression(JoinType.OuterApply, this.currentFrom, proj.Select, null);
                    this.currentFrom = expression4;
                    return proj.Projector;
                }
                return proj;
            }
            Expression expression5 = QueryBinder.BindMember(root, m.Member);
            MemberExpression expression6 = expression5 as MemberExpression;
            if (((expression6 != null) && (expression6.Member == m.Member)) && (expression6.Expression == m.Expression))
            {
                return m;
            }
            return expression5;
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            Expression expression5;
            Expression currentFrom = this.currentFrom;
            this.currentFrom = this.VisitSource(select.From);
            try
            {
                Expression where = this.Visit(select.Where);
                ReadOnlyCollection<OrderExpression> orderBy = this.VisitOrderBy(select.OrderBy);
                ReadOnlyCollection<Expression> groupBy = this.VisitExpressionList(select.GroupBy);
                Expression skip = this.Visit(select.Skip);
                Expression take = this.Visit(select.Take);
                ReadOnlyCollection<ColumnDeclaration> columns = this.VisitColumnDeclarations(select.Columns);
                if (((((this.currentFrom != select.From) || (where != select.Where)) || ((orderBy != select.OrderBy) || (groupBy != select.GroupBy))) || ((take != select.Take) || (skip != select.Skip))) || (columns != select.Columns))
                {
                    return new SelectExpression(select.Alias, columns, this.currentFrom, where, orderBy, groupBy, select.IsDistinct, skip, take, select.IsReverse);
                }
                expression5 = select;
            }
            finally
            {
                this.currentFrom = currentFrom;
            }
            return expression5;
        }
    }
}
