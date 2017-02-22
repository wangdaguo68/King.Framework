using System.Reflection;

namespace King.Framework.Linq
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;

    public class ExpressionComparer
    {
        private Func<object, object, bool> fnCompare;
        private ScopedDictionary<ParameterExpression, ParameterExpression> parameterScope;

        protected ExpressionComparer(ScopedDictionary<ParameterExpression, ParameterExpression> parameterScope, Func<object, object, bool> fnCompare)
        {
            this.parameterScope = parameterScope;
            this.fnCompare = fnCompare;
        }

        public static bool AreEqual(Expression a, Expression b)
        {
            return AreEqual(null, a, b);
        }

        public static bool AreEqual(ScopedDictionary<ParameterExpression, ParameterExpression> parameterScope, Expression a, Expression b)
        {
            return new ExpressionComparer(parameterScope, null).Compare(a, b);
        }

        public static bool AreEqual(Expression a, Expression b, Func<object, object, bool> fnCompare)
        {
            return AreEqual(null, a, b, fnCompare);
        }

        public static bool AreEqual(ScopedDictionary<ParameterExpression, ParameterExpression> parameterScope, Expression a, Expression b, Func<object, object, bool> fnCompare)
        {
            return new ExpressionComparer(parameterScope, fnCompare).Compare(a, b);
        }

        protected virtual bool Compare(Expression a, Expression b)
        {
            if (a == b)
            {
                return true;
            }
            if ((a == null) || (b == null))
            {
                return false;
            }
            if (a.NodeType != b.NodeType)
            {
                return false;
            }
            if (a.Type != b.Type)
            {
                return false;
            }
            switch (a.NodeType)
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
                    return this.CompareBinary((BinaryExpression) a, (BinaryExpression) b);

                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.UnaryPlus:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return this.CompareUnary((UnaryExpression) a, (UnaryExpression) b);

                case ExpressionType.Call:
                    return this.CompareMethodCall((MethodCallExpression) a, (MethodCallExpression) b);

                case ExpressionType.Conditional:
                    return this.CompareConditional((ConditionalExpression) a, (ConditionalExpression) b);

                case ExpressionType.Constant:
                    return this.CompareConstant((ConstantExpression) a, (ConstantExpression) b);

                case ExpressionType.Invoke:
                    return this.CompareInvocation((InvocationExpression) a, (InvocationExpression) b);

                case ExpressionType.Lambda:
                    return this.CompareLambda((LambdaExpression) a, (LambdaExpression) b);

                case ExpressionType.ListInit:
                    return this.CompareListInit((ListInitExpression) a, (ListInitExpression) b);

                case ExpressionType.MemberAccess:
                    return this.CompareMemberAccess((MemberExpression) a, (MemberExpression) b);

                case ExpressionType.MemberInit:
                    return this.CompareMemberInit((MemberInitExpression) a, (MemberInitExpression) b);

                case ExpressionType.New:
                    return this.CompareNew((NewExpression) a, (NewExpression) b);

                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return this.CompareNewArray((NewArrayExpression) a, (NewArrayExpression) b);

                case ExpressionType.Parameter:
                    return this.CompareParameter((ParameterExpression) a, (ParameterExpression) b);

                case ExpressionType.TypeIs:
                    return this.CompareTypeIs((TypeBinaryExpression) a, (TypeBinaryExpression) b);
            }
            throw new Exception(string.Format("Unhandled expression type: '{0}'", a.NodeType));
        }

        protected virtual bool CompareBinary(BinaryExpression a, BinaryExpression b)
        {
            return (((((a.NodeType == b.NodeType) && (a.Method == b.Method)) && ((a.IsLifted == b.IsLifted) && (a.IsLiftedToNull == b.IsLiftedToNull))) && this.Compare(a.Left, b.Left)) && this.Compare(a.Right, b.Right));
        }

        protected virtual bool CompareBinding(MemberBinding a, MemberBinding b)
        {
            if (a == b)
            {
                return true;
            }
            if ((a == null) || (b == null))
            {
                return false;
            }
            if (a.BindingType != b.BindingType)
            {
                return false;
            }
            if (a.Member != b.Member)
            {
                return false;
            }
            switch (a.BindingType)
            {
                case MemberBindingType.Assignment:
                    return this.CompareMemberAssignment((MemberAssignment) a, (MemberAssignment) b);

                case MemberBindingType.MemberBinding:
                    return this.CompareMemberMemberBinding((MemberMemberBinding) a, (MemberMemberBinding) b);

                case MemberBindingType.ListBinding:
                    return this.CompareMemberListBinding((MemberListBinding) a, (MemberListBinding) b);
            }
            throw new Exception(string.Format("Unhandled binding type: '{0}'", a.BindingType));
        }

        protected virtual bool CompareBindingList(ReadOnlyCollection<MemberBinding> a, ReadOnlyCollection<MemberBinding> b)
        {
            if (a != b)
            {
                if ((a == null) || (b == null))
                {
                    return false;
                }
                if (a.Count != b.Count)
                {
                    return false;
                }
                int num = 0;
                int count = a.Count;
                while (num < count)
                {
                    if (!this.CompareBinding(a[num], b[num]))
                    {
                        return false;
                    }
                    num++;
                }
            }
            return true;
        }

        protected virtual bool CompareConditional(ConditionalExpression a, ConditionalExpression b)
        {
            return ((this.Compare(a.Test, b.Test) && this.Compare(a.IfTrue, b.IfTrue)) && this.Compare(a.IfFalse, b.IfFalse));
        }

        protected virtual bool CompareConstant(ConstantExpression a, ConstantExpression b)
        {
            if (this.fnCompare != null)
            {
                return this.fnCompare(a.Value, b.Value);
            }
            return object.Equals(a.Value, b.Value);
        }

        protected virtual bool CompareElementInit(ElementInit a, ElementInit b)
        {
            return ((a.AddMethod == b.AddMethod) && this.CompareExpressionList(a.Arguments, b.Arguments));
        }

        protected virtual bool CompareElementInitList(ReadOnlyCollection<ElementInit> a, ReadOnlyCollection<ElementInit> b)
        {
            if (a != b)
            {
                if ((a == null) || (b == null))
                {
                    return false;
                }
                if (a.Count != b.Count)
                {
                    return false;
                }
                int num = 0;
                int count = a.Count;
                while (num < count)
                {
                    if (!this.CompareElementInit(a[num], b[num]))
                    {
                        return false;
                    }
                    num++;
                }
            }
            return true;
        }

        protected virtual bool CompareExpressionList(ReadOnlyCollection<Expression> a, ReadOnlyCollection<Expression> b)
        {
            if (a != b)
            {
                if ((a == null) || (b == null))
                {
                    return false;
                }
                if (a.Count != b.Count)
                {
                    return false;
                }
                int num = 0;
                int count = a.Count;
                while (num < count)
                {
                    if (!this.Compare(a[num], b[num]))
                    {
                        return false;
                    }
                    num++;
                }
            }
            return true;
        }

        protected virtual bool CompareInvocation(InvocationExpression a, InvocationExpression b)
        {
            return (this.Compare(a.Expression, b.Expression) && this.CompareExpressionList(a.Arguments, b.Arguments));
        }

        protected virtual bool CompareLambda(LambdaExpression a, LambdaExpression b)
        {
            int num2;
            bool flag;
            int count = a.Parameters.Count;
            if (b.Parameters.Count != count)
            {
                return false;
            }
            for (num2 = 0; num2 < count; num2++)
            {
                if (a.Parameters[num2].Type != b.Parameters[num2].Type)
                {
                    return false;
                }
            }
            ScopedDictionary<ParameterExpression, ParameterExpression> parameterScope = this.parameterScope;
            this.parameterScope = new ScopedDictionary<ParameterExpression, ParameterExpression>(this.parameterScope);
            try
            {
                for (num2 = 0; num2 < count; num2++)
                {
                    this.parameterScope.Add(a.Parameters[num2], b.Parameters[num2]);
                }
                flag = this.Compare(a.Body, b.Body);
            }
            finally
            {
                this.parameterScope = parameterScope;
            }
            return flag;
        }

        protected virtual bool CompareListInit(ListInitExpression a, ListInitExpression b)
        {
            return (this.Compare(a.NewExpression, b.NewExpression) && this.CompareElementInitList(a.Initializers, b.Initializers));
        }

        protected virtual bool CompareMemberAccess(MemberExpression a, MemberExpression b)
        {
            return ((a.Member == b.Member) && this.Compare(a.Expression, b.Expression));
        }

        protected virtual bool CompareMemberAssignment(MemberAssignment a, MemberAssignment b)
        {
            return ((a.Member == b.Member) && this.Compare(a.Expression, b.Expression));
        }

        protected virtual bool CompareMemberInit(MemberInitExpression a, MemberInitExpression b)
        {
            return (this.Compare(a.NewExpression, b.NewExpression) && this.CompareBindingList(a.Bindings, b.Bindings));
        }

        protected virtual bool CompareMemberList(ReadOnlyCollection<MemberInfo> a, ReadOnlyCollection<MemberInfo> b)
        {
            if (a != b)
            {
                if ((a == null) || (b == null))
                {
                    return false;
                }
                if (a.Count != b.Count)
                {
                    return false;
                }
                int num = 0;
                int count = a.Count;
                while (num < count)
                {
                    if (a[num] != b[num])
                    {
                        return false;
                    }
                    num++;
                }
            }
            return true;
        }

        protected virtual bool CompareMemberListBinding(MemberListBinding a, MemberListBinding b)
        {
            return ((a.Member == b.Member) && this.CompareElementInitList(a.Initializers, b.Initializers));
        }

        protected virtual bool CompareMemberMemberBinding(MemberMemberBinding a, MemberMemberBinding b)
        {
            return ((a.Member == b.Member) && this.CompareBindingList(a.Bindings, b.Bindings));
        }

        protected virtual bool CompareMethodCall(MethodCallExpression a, MethodCallExpression b)
        {
            return (((a.Method == b.Method) && this.Compare(a.Object, b.Object)) && this.CompareExpressionList(a.Arguments, b.Arguments));
        }

        protected virtual bool CompareNew(NewExpression a, NewExpression b)
        {
            return (((a.Constructor == b.Constructor) && this.CompareExpressionList(a.Arguments, b.Arguments)) && this.CompareMemberList(a.Members, b.Members));
        }

        protected virtual bool CompareNewArray(NewArrayExpression a, NewArrayExpression b)
        {
            return this.CompareExpressionList(a.Expressions, b.Expressions);
        }

        protected virtual bool CompareParameter(ParameterExpression a, ParameterExpression b)
        {
            ParameterExpression expression;
            if ((this.parameterScope != null) && this.parameterScope.TryGetValue(a, out expression))
            {
                return (expression == b);
            }
            return (a == b);
        }

        protected virtual bool CompareTypeIs(TypeBinaryExpression a, TypeBinaryExpression b)
        {
            return ((a.TypeOperand == b.TypeOperand) && this.Compare(a.Expression, b.Expression));
        }

        protected virtual bool CompareUnary(UnaryExpression a, UnaryExpression b)
        {
            return ((((a.NodeType == b.NodeType) && (a.Method == b.Method)) && ((a.IsLifted == b.IsLifted) && (a.IsLiftedToNull == b.IsLiftedToNull))) && this.Compare(a.Operand, b.Operand));
        }

        protected Func<object, object, bool> FnCompare
        {
            get
            {
                return this.fnCompare;
            }
        }
    }
}
