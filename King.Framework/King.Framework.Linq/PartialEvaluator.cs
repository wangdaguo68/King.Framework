namespace King.Framework.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public static class PartialEvaluator
    {
        private static bool CanBeEvaluatedLocally(Expression expression)
        {
            return (expression.NodeType != ExpressionType.Parameter);
        }

        public static Expression Eval(Expression expression)
        {
            return Eval(expression, null, null);
        }

        public static Expression Eval(Expression expression, Func<Expression, bool> fnCanBeEvaluated)
        {
            return Eval(expression, fnCanBeEvaluated, null);
        }

        public static Expression Eval(Expression expression, Func<Expression, bool> fnCanBeEvaluated, Func<ConstantExpression, Expression> fnPostEval)
        {
            if (fnCanBeEvaluated == null)
            {
                fnCanBeEvaluated = new Func<Expression, bool>(PartialEvaluator.CanBeEvaluatedLocally);
            }
            return SubtreeEvaluator.Eval(Nominator.Nominate(fnCanBeEvaluated, expression), fnPostEval, expression);
        }

        private class Nominator : King.Framework.Linq.ExpressionVisitor
        {
            private HashSet<Expression> candidates = new HashSet<Expression>();
            private bool cannotBeEvaluated;
            private Func<Expression, bool> fnCanBeEvaluated;

            private Nominator(Func<Expression, bool> fnCanBeEvaluated)
            {
                this.fnCanBeEvaluated = fnCanBeEvaluated;
            }

            internal static HashSet<Expression> Nominate(Func<Expression, bool> fnCanBeEvaluated, Expression expression)
            {
                PartialEvaluator.Nominator nominator = new PartialEvaluator.Nominator(fnCanBeEvaluated);
                nominator.Visit(expression);
                return nominator.candidates;
            }

            protected override Expression Visit(Expression expression)
            {
                if (expression != null)
                {
                    bool cannotBeEvaluated = this.cannotBeEvaluated;
                    this.cannotBeEvaluated = false;
                    base.Visit(expression);
                    if (!this.cannotBeEvaluated)
                    {
                        if (this.fnCanBeEvaluated(expression))
                        {
                            this.candidates.Add(expression);
                        }
                        else
                        {
                            this.cannotBeEvaluated = true;
                        }
                    }
                    this.cannotBeEvaluated |= cannotBeEvaluated;
                }
                return expression;
            }

            protected override Expression VisitConstant(ConstantExpression c)
            {
                return base.VisitConstant(c);
            }
        }

        private class SubtreeEvaluator : King.Framework.Linq.ExpressionVisitor
        {
            private HashSet<Expression> candidates;
            private Func<ConstantExpression, Expression> onEval;

            private SubtreeEvaluator(HashSet<Expression> candidates, Func<ConstantExpression, Expression> onEval)
            {
                this.candidates = candidates;
                this.onEval = onEval;
            }

            internal static Expression Eval(HashSet<Expression> candidates, Func<ConstantExpression, Expression> onEval, Expression exp)
            {
                return new PartialEvaluator.SubtreeEvaluator(candidates, onEval).Visit(exp);
            }

            private Expression Evaluate(Expression e)
            {
                Type type = e.Type;
                if (e.NodeType == ExpressionType.Convert)
                {
                    UnaryExpression expression = (UnaryExpression) e;
                    if (TypeHelper.GetNonNullableType(expression.Operand.Type) == TypeHelper.GetNonNullableType(type))
                    {
                        e = ((UnaryExpression) e).Operand;
                    }
                }
                if (e.NodeType == ExpressionType.Constant)
                {
                    if (e.Type == type)
                    {
                        return e;
                    }
                    if (TypeHelper.GetNonNullableType(e.Type) == TypeHelper.GetNonNullableType(type))
                    {
                        return Expression.Constant(((ConstantExpression) e).Value, type);
                    }
                }
                MemberExpression expression2 = e as MemberExpression;
                if (expression2 != null)
                {
                    ConstantExpression expression3 = expression2.Expression as ConstantExpression;
                    if (expression3 != null)
                    {
                        return this.PostEval(Expression.Constant(expression2.Member.GetValue(expression3.Value), type));
                    }
                }
                if (type.IsValueType)
                {
                    e = Expression.Convert(e, typeof(object));
                }
                Func<object> func = Expression.Lambda<Func<object>>(e, new ParameterExpression[0]).Compile();
                return this.PostEval(Expression.Constant(func(), type));
            }

            private Expression PostEval(ConstantExpression e)
            {
                if (this.onEval != null)
                {
                    return this.onEval(e);
                }
                return e;
            }

            protected override Expression Visit(Expression exp)
            {
                if (exp == null)
                {
                    return null;
                }
                if (this.candidates.Contains(exp))
                {
                    return this.Evaluate(exp);
                }
                return base.Visit(exp);
            }
        }
    }
}
