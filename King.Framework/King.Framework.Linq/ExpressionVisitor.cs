namespace King.Framework.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
    using System.Reflection;

    public abstract class ExpressionVisitor
    {
        protected ExpressionVisitor()
        {
        }

        protected BinaryExpression UpdateBinary(BinaryExpression b, Expression left, Expression right, Expression conversion, bool isLiftedToNull, MethodInfo method)
        {
            if ((((left != b.Left) || (right != b.Right)) || ((conversion != b.Conversion) || (method != b.Method))) || (isLiftedToNull != b.IsLiftedToNull))
            {
                if ((b.NodeType == ExpressionType.Coalesce) && (b.Conversion != null))
                {
                    return Expression.Coalesce(left, right, conversion as LambdaExpression);
                }
                return Expression.MakeBinary(b.NodeType, left, right, isLiftedToNull, method);
            }
            return b;
        }

        protected ConditionalExpression UpdateConditional(ConditionalExpression c, Expression test, Expression ifTrue, Expression ifFalse)
        {
            if (((test != c.Test) || (ifTrue != c.IfTrue)) || (ifFalse != c.IfFalse))
            {
                return Expression.Condition(test, ifTrue, ifFalse);
            }
            return c;
        }

        protected InvocationExpression UpdateInvocation(InvocationExpression iv, Expression expression, IEnumerable<Expression> args)
        {
            if ((args != iv.Arguments) || (expression != iv.Expression))
            {
                return Expression.Invoke(expression, args);
            }
            return iv;
        }

        protected LambdaExpression UpdateLambda(LambdaExpression lambda, Type delegateType, Expression body, IEnumerable<ParameterExpression> parameters)
        {
            if (((body != lambda.Body) || (parameters != lambda.Parameters)) || (delegateType != lambda.Type))
            {
                return Expression.Lambda(delegateType, body, parameters);
            }
            return lambda;
        }

        protected ListInitExpression UpdateListInit(ListInitExpression init, NewExpression nex, IEnumerable<ElementInit> initializers)
        {
            if ((nex != init.NewExpression) || (initializers != init.Initializers))
            {
                return Expression.ListInit(nex, initializers);
            }
            return init;
        }

        protected MemberExpression UpdateMemberAccess(MemberExpression m, Expression expression, MemberInfo member)
        {
            if ((expression != m.Expression) || (member != m.Member))
            {
                return Expression.MakeMemberAccess(expression, member);
            }
            return m;
        }

        protected MemberAssignment UpdateMemberAssignment(MemberAssignment assignment, MemberInfo member, Expression expression)
        {
            if ((expression != assignment.Expression) || (member != assignment.Member))
            {
                return Expression.Bind(member, expression);
            }
            return assignment;
        }

        protected MemberInitExpression UpdateMemberInit(MemberInitExpression init, NewExpression nex, IEnumerable<MemberBinding> bindings)
        {
            if ((nex != init.NewExpression) || (bindings != init.Bindings))
            {
                return Expression.MemberInit(nex, bindings);
            }
            return init;
        }

        protected MemberListBinding UpdateMemberListBinding(MemberListBinding binding, MemberInfo member, IEnumerable<ElementInit> initializers)
        {
            if ((initializers != binding.Initializers) || (member != binding.Member))
            {
                return Expression.ListBind(member, initializers);
            }
            return binding;
        }

        protected MemberMemberBinding UpdateMemberMemberBinding(MemberMemberBinding binding, MemberInfo member, IEnumerable<MemberBinding> bindings)
        {
            if ((bindings != binding.Bindings) || (member != binding.Member))
            {
                return Expression.MemberBind(member, bindings);
            }
            return binding;
        }

        protected MethodCallExpression UpdateMethodCall(MethodCallExpression m, Expression obj, MethodInfo method, IEnumerable<Expression> args)
        {
            if (((obj != m.Object) || (method != m.Method)) || (args != m.Arguments))
            {
                return Expression.Call(obj, method, args);
            }
            return m;
        }

        protected NewExpression UpdateNew(NewExpression nex, ConstructorInfo constructor, IEnumerable<Expression> args, IEnumerable<MemberInfo> members)
        {
            if (((args != nex.Arguments) || (constructor != nex.Constructor)) || (members != nex.Members))
            {
                if (nex.Members != null)
                {
                    return Expression.New(constructor, args, members);
                }
                return Expression.New(constructor, args);
            }
            return nex;
        }

        protected NewArrayExpression UpdateNewArray(NewArrayExpression na, Type arrayType, IEnumerable<Expression> expressions)
        {
            if ((expressions != na.Expressions) || (na.Type != arrayType))
            {
                if (na.NodeType == ExpressionType.NewArrayInit)
                {
                    return Expression.NewArrayInit(arrayType.GetElementType(), expressions);
                }
                return Expression.NewArrayBounds(arrayType.GetElementType(), expressions);
            }
            return na;
        }

        protected TypeBinaryExpression UpdateTypeIs(TypeBinaryExpression b, Expression expression, Type typeOperand)
        {
            if ((expression != b.Expression) || (typeOperand != b.TypeOperand))
            {
                return Expression.TypeIs(expression, typeOperand);
            }
            return b;
        }

        protected UnaryExpression UpdateUnary(UnaryExpression u, Expression operand, Type resultType, MethodInfo method)
        {
            if (((u.Operand != operand) || (u.Type != resultType)) || (u.Method != method))
            {
                return Expression.MakeUnary(u.NodeType, operand, resultType, method);
            }
            return u;
        }

        protected virtual Expression Visit(Expression exp)
        {
            if (exp == null)
            {
                return exp;
            }
            switch (exp.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.ArrayIndex:
                case ExpressionType.Coalesce:
                case ExpressionType.Divide:
                case ExpressionType.Equal:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LeftShift:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.NotEqual:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.Power:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return this.VisitBinary((BinaryExpression) exp);

                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.UnaryPlus:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return this.VisitUnary((UnaryExpression) exp);

                case ExpressionType.Call:
                    return this.VisitMethodCall((MethodCallExpression) exp);

                case ExpressionType.Conditional:
                    return this.VisitConditional((ConditionalExpression) exp);

                case ExpressionType.Constant:
                    return this.VisitConstant((ConstantExpression) exp);

                case ExpressionType.Invoke:
                    return this.VisitInvocation((InvocationExpression) exp);

                case ExpressionType.Lambda:
                    return this.VisitLambda((LambdaExpression) exp);

                case ExpressionType.ListInit:
                    return this.VisitListInit((ListInitExpression) exp);

                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess((MemberExpression) exp);

                case ExpressionType.MemberInit:
                    return this.VisitMemberInit((MemberInitExpression) exp);

                case ExpressionType.New:
                    return this.VisitNew((NewExpression) exp);

                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return this.VisitNewArray((NewArrayExpression) exp);

                case ExpressionType.Parameter:
                    return this.VisitParameter((ParameterExpression) exp);

                case ExpressionType.TypeIs:
                    return this.VisitTypeIs((TypeBinaryExpression) exp);
            }
            return this.VisitUnknown(exp);
        }

        protected virtual Expression VisitBinary(BinaryExpression b)
        {
            Expression left = this.Visit(b.Left);
            Expression right = this.Visit(b.Right);
            Expression conversion = this.Visit(b.Conversion);
            return this.UpdateBinary(b, left, right, conversion, b.IsLiftedToNull, b.Method);
        }

        protected virtual MemberBinding VisitBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return this.VisitMemberAssignment((MemberAssignment) binding);

                case MemberBindingType.MemberBinding:
                    return this.VisitMemberMemberBinding((MemberMemberBinding) binding);

                case MemberBindingType.ListBinding:
                    return this.VisitMemberListBinding((MemberListBinding) binding);
            }
            throw new Exception(string.Format("Unhandled binding type '{0}'", binding.BindingType));
        }

        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            List<MemberBinding> list = null;
            int num = 0;
            int count = original.Count;
            while (num < count)
            {
                MemberBinding item = this.VisitBinding(original[num]);
                if (list != null)
                {
                    list.Add(item);
                }
                else if (item != original[num])
                {
                    list = new List<MemberBinding>(count);
                    for (int i = 0; i < num; i++)
                    {
                        list.Add(original[i]);
                    }
                    list.Add(item);
                }
                num++;
            }
            if (list != null)
            {
                return list;
            }
            return original;
        }

        protected virtual Expression VisitConditional(ConditionalExpression c)
        {
            Expression test = this.Visit(c.Test);
            Expression ifTrue = this.Visit(c.IfTrue);
            Expression ifFalse = this.Visit(c.IfFalse);
            return this.UpdateConditional(c, test, ifTrue, ifFalse);
        }

        protected virtual Expression VisitConstant(ConstantExpression c)
        {
            return c;
        }

        protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            ReadOnlyCollection<Expression> arguments = this.VisitExpressionList(initializer.Arguments);
            if (arguments != initializer.Arguments)
            {
                return Expression.ElementInit(initializer.AddMethod, arguments);
            }
            return initializer;
        }

        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            List<ElementInit> list = null;
            int num = 0;
            int count = original.Count;
            while (num < count)
            {
                ElementInit item = this.VisitElementInitializer(original[num]);
                if (list != null)
                {
                    list.Add(item);
                }
                else if (item != original[num])
                {
                    list = new List<ElementInit>(count);
                    for (int i = 0; i < num; i++)
                    {
                        list.Add(original[i]);
                    }
                    list.Add(item);
                }
                num++;
            }
            if (list != null)
            {
                return list;
            }
            return original;
        }

        protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            if (original != null)
            {
                List<Expression> list = null;
                int num = 0;
                int count = original.Count;
                while (num < count)
                {
                    Expression item = this.Visit(original[num]);
                    if (list != null)
                    {
                        list.Add(item);
                    }
                    else if (item != original[num])
                    {
                        list = new List<Expression>(count);
                        for (int i = 0; i < num; i++)
                        {
                            list.Add(original[i]);
                        }
                        list.Add(item);
                    }
                    num++;
                }
                if (list != null)
                {
                    return list.AsReadOnly();
                }
            }
            return original;
        }

        protected virtual Expression VisitInvocation(InvocationExpression iv)
        {
            IEnumerable<Expression> args = this.VisitExpressionList(iv.Arguments);
            Expression expression = this.Visit(iv.Expression);
            return this.UpdateInvocation(iv, expression, args);
        }

        protected virtual Expression VisitLambda(LambdaExpression lambda)
        {
            Expression body = this.Visit(lambda.Body);
            return this.UpdateLambda(lambda, lambda.Type, body, lambda.Parameters);
        }

        protected virtual Expression VisitListInit(ListInitExpression init)
        {
            NewExpression nex = this.VisitNew(init.NewExpression);
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(init.Initializers);
            return this.UpdateListInit(init, nex, initializers);
        }

        protected virtual Expression VisitMemberAccess(MemberExpression m)
        {
            Expression expression = this.Visit(m.Expression);
            return this.UpdateMemberAccess(m, expression, m.Member);
        }

        protected virtual Expression VisitMemberAndExpression(MemberInfo member, Expression expression)
        {
            return this.Visit(expression);
        }

        protected virtual ReadOnlyCollection<Expression> VisitMemberAndExpressionList(ReadOnlyCollection<MemberInfo> members, ReadOnlyCollection<Expression> original)
        {
            if (original != null)
            {
                List<Expression> list = null;
                int num = 0;
                int count = original.Count;
                while (num < count)
                {
                    Expression item = this.VisitMemberAndExpression((members != null) ? members[num] : null, original[num]);
                    if (list != null)
                    {
                        list.Add(item);
                    }
                    else if (item != original[num])
                    {
                        list = new List<Expression>(count);
                        for (int i = 0; i < num; i++)
                        {
                            list.Add(original[i]);
                        }
                        list.Add(item);
                    }
                    num++;
                }
                if (list != null)
                {
                    return list.AsReadOnly();
                }
            }
            return original;
        }

        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            Expression expression = this.Visit(assignment.Expression);
            return this.UpdateMemberAssignment(assignment, assignment.Member, expression);
        }

        protected virtual Expression VisitMemberInit(MemberInitExpression init)
        {
            NewExpression nex = this.VisitNew(init.NewExpression);
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(init.Bindings);
            return this.UpdateMemberInit(init, nex, bindings);
        }

        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(binding.Initializers);
            return this.UpdateMemberListBinding(binding, binding.Member, initializers);
        }

        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(binding.Bindings);
            return this.UpdateMemberMemberBinding(binding, binding.Member, bindings);
        }

        protected virtual Expression VisitMethodCall(MethodCallExpression m)
        {
            Expression expression = this.Visit(m.Object);
            IEnumerable<Expression> args = this.VisitExpressionList(m.Arguments);
            return this.UpdateMethodCall(m, expression, m.Method, args);
        }

        protected virtual NewExpression VisitNew(NewExpression nex)
        {
            IEnumerable<Expression> args = this.VisitMemberAndExpressionList(nex.Members, nex.Arguments);
            return this.UpdateNew(nex, nex.Constructor, args, nex.Members);
        }

        protected virtual Expression VisitNewArray(NewArrayExpression na)
        {
            IEnumerable<Expression> expressions = this.VisitExpressionList(na.Expressions);
            return this.UpdateNewArray(na, na.Type, expressions);
        }

        protected virtual Expression VisitParameter(ParameterExpression p)
        {
            return p;
        }

        protected virtual Expression VisitTypeIs(TypeBinaryExpression b)
        {
            Expression expression = this.Visit(b.Expression);
            return this.UpdateTypeIs(b, expression, b.TypeOperand);
        }

        protected virtual Expression VisitUnary(UnaryExpression u)
        {
            Expression operand = this.Visit(u.Operand);
            return this.UpdateUnary(u, operand, u.Type, u.Method);
        }

        protected virtual Expression VisitUnknown(Expression expression)
        {
            throw new Exception(string.Format("Unhandled expression type: '{0}'", expression.NodeType));
        }
    }
}
