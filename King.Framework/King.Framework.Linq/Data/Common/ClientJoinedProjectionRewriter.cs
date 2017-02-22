namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public class ClientJoinedProjectionRewriter : DbExpressionVisitor
    {
        private bool canJoinOnClient = true;
        private MemberInfo currentMember;
        private SelectExpression currentSelect;
        private bool isTopLevel = true;
        private QueryLanguage language;
        private QueryPolicy policy;

        private ClientJoinedProjectionRewriter(QueryPolicy policy, QueryLanguage language)
        {
            this.policy = policy;
            this.language = language;
        }

        private bool CanJoinOnClient(SelectExpression select)
        {
            return ((((this.canJoinOnClient && (this.currentMember != null)) && (!this.policy.IsDeferLoaded(this.currentMember) && !select.IsDistinct)) && ((select.GroupBy == null) || (select.GroupBy.Count == 0))) && !AggregateChecker.HasAggregates(select));
        }

        private ColumnExpression GetColumnExpression(Expression expression)
        {
            while ((expression.NodeType == ExpressionType.Convert) || (expression.NodeType == ExpressionType.ConvertChecked))
            {
                expression = ((UnaryExpression)expression).Operand;
            }
            return (expression as ColumnExpression);
        }

        private bool GetEquiJoinKeyExpressions(Expression predicate, TableAlias outerAlias, List<Expression> outerExpressions, List<Expression> innerExpressions)
        {
            if (predicate.NodeType == ExpressionType.Equal)
            {
                BinaryExpression expression = (BinaryExpression)predicate;
                ColumnExpression columnExpression = this.GetColumnExpression(expression.Left);
                ColumnExpression expression3 = this.GetColumnExpression(expression.Right);
                if ((columnExpression != null) && (expression3 != null))
                {
                    if (columnExpression.Alias == outerAlias)
                    {
                        outerExpressions.Add(expression.Left);
                        innerExpressions.Add(expression.Right);
                        return true;
                    }
                    if (expression3.Alias == outerAlias)
                    {
                        innerExpressions.Add(expression.Left);
                        outerExpressions.Add(expression.Right);
                        return true;
                    }
                }
            }
            bool flag = false;
            Expression[] expressionArray = predicate.Split(new ExpressionType[] { ExpressionType.And, ExpressionType.AndAlso });
            if (expressionArray.Length > 1)
            {
                foreach (Expression expression4 in expressionArray)
                {
                    if (ReferencedAliasGatherer.Gather(expression4).Contains(outerAlias))
                    {
                        if (!this.GetEquiJoinKeyExpressions(expression4, outerAlias, outerExpressions, innerExpressions))
                        {
                            return false;
                        }
                        flag = true;
                    }
                }
            }
            return flag;
        }

        public static Expression Rewrite(QueryPolicy policy, QueryLanguage language, Expression expression)
        {
            return new ClientJoinedProjectionRewriter(policy, language).Visit(expression);
        }

        protected override Expression VisitCommand(CommandExpression command)
        {
            this.isTopLevel = true;
            return base.VisitCommand(command);
        }

        protected override Expression VisitMemberAndExpression(MemberInfo member, Expression expression)
        {
            MemberInfo currentMember = this.currentMember;
            this.currentMember = member;
            Expression expression2 = this.Visit(expression);
            this.currentMember = currentMember;
            return expression2;
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            MemberInfo currentMember = this.currentMember;
            this.currentMember = assignment.Member;
            Expression expression = this.Visit(assignment.Expression);
            this.currentMember = currentMember;
            return base.UpdateMemberAssignment(assignment, assignment.Member, expression);
        }

        protected override Expression VisitProjection(ProjectionExpression proj)
        {
            Expression expression7;
            SelectExpression save = this.currentSelect;
            this.currentSelect = proj.Select;
            try
            {
                if (!this.isTopLevel)
                {
                    if (!this.CanJoinOnClient(this.currentSelect))
                    {
                        bool canJoinOnClient = this.canJoinOnClient;
                        this.canJoinOnClient = false;
                        Expression expression6 = base.VisitProjection(proj);
                        this.canJoinOnClient = canJoinOnClient;
                        return expression6;
                    }
                    Func<Expression, Expression> selector = null;
                    Func<Expression, Expression> func2 = null;
                    SelectExpression newOuterSelect = (SelectExpression)QueryDuplicator.Duplicate(save);
                    SelectExpression source = (SelectExpression)ColumnMapper.Map(proj.Select, newOuterSelect.Alias, new TableAlias[] { save.Alias });
                    ProjectionExpression expression2 = this.language.AddOuterJoinTest(new ProjectionExpression(source, proj.Projector));
                    source = expression2.Select;
                    Expression projector = expression2.Projector;
                    TableAlias newAlias = new TableAlias();
                    ProjectedColumns columns = ColumnProjector.ProjectColumns(this.language, projector, null, newAlias, new TableAlias[] { newOuterSelect.Alias, source.Alias });
                    JoinExpression from = new JoinExpression(JoinType.OuterApply, newOuterSelect, source, null);
                    SelectExpression joinedSelect = new SelectExpression(newAlias, columns.Columns, from, null, null, null, proj.IsSingleton, null, null, false);
                    this.currentSelect = joinedSelect;
                    projector = this.Visit(columns.Projector);
                    List<Expression> outerExpressions = new List<Expression>();
                    List<Expression> innerExpressions = new List<Expression>();
                    if (this.GetEquiJoinKeyExpressions(source.Where, newOuterSelect.Alias, outerExpressions, innerExpressions))
                    {
                        if (selector == null)
                        {
                            selector = k => ColumnMapper.Map(k, save.Alias, new TableAlias[] { newOuterSelect.Alias });
                        }
                        IEnumerable<Expression> outerKey = outerExpressions.Select<Expression, Expression>(selector);
                        if (func2 == null)
                        {
                            func2 = k => ColumnMapper.Map(k, joinedSelect.Alias, new TableAlias[] { ((ColumnExpression)k).Alias });
                        }
                        IEnumerable<Expression> innerKey = innerExpressions.Select<Expression, Expression>(func2);
                        return new ClientJoinExpression(new ProjectionExpression(joinedSelect, projector, proj.Aggregator), outerKey, innerKey);
                    }
                }
                else
                {
                    this.isTopLevel = false;
                }
                expression7 = base.VisitProjection(proj);
            }
            finally
            {
                this.currentSelect = save;
            }
            return expression7;
        }

        protected override Expression VisitSubquery(SubqueryExpression subquery)
        {
            return subquery;
        }
    }
}
