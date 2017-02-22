namespace King.Framework.Linq
{
    using System;
    using System.Reflection;

    public class StrongDelegate
    {
        private static MethodInfo[] _meths = new MethodInfo[9];
        private Func<object[], object> fn;

        static StrongDelegate()
        {
            MethodInfo[] methods = typeof(StrongDelegate).GetMethods();
            int index = 0;
            int length = methods.Length;
            while (index < length)
            {
                MethodInfo info = methods[index];
                if (info.Name.StartsWith("M"))
                {
                    Type[] genericArguments = info.GetGenericArguments();
                    _meths[genericArguments.Length - 1] = info;
                }
                index++;
            }
        }

        private StrongDelegate(Func<object[], object> fn)
        {
            this.fn = fn;
        }

        public static Delegate CreateDelegate(Type delegateType, Func<object[], object> fn)
        {
            MethodInfo method = delegateType.GetMethod("Invoke");
            ParameterInfo[] parameters = method.GetParameters();
            Type[] typeArguments = new Type[1 + parameters.Length];
            int index = 0;
            int length = parameters.Length;
            while (index < length)
            {
                typeArguments[index] = parameters[index].ParameterType;
                index++;
            }
            typeArguments[typeArguments.Length - 1] = method.ReturnType;
            if (typeArguments.Length > _meths.Length)
            {
                throw new NotSupportedException("Delegate has too many arguments");
            }
            MethodInfo info3 = _meths[typeArguments.Length - 1].MakeGenericMethod(typeArguments);
            return Delegate.CreateDelegate(delegateType, new StrongDelegate(fn), info3);
        }

        public static Delegate CreateDelegate(Type delegateType, object target, MethodInfo method)
        {
            return CreateDelegate(delegateType, (Func<object[], object>) Delegate.CreateDelegate(typeof(Func<object[], object>), target, method));
        }

        public R M<R>()
        {
            return (R) this.fn(null);
        }

        public R M<A1, R>(A1 a1)
        {
            return (R)this.fn(new object[] { a1 });
        }

        public R M<A1, A2, R>(A1 a1, A2 a2)
        {
            return (R) this.fn(new object[] { a1, a2 });
        }

        public R M<A1, A2, A3, R>(A1 a1, A2 a2, A3 a3)
        {
            return (R) this.fn(new object[] { a1, a2, a3 });
        }

        public R M<A1, A2, A3, A4, R>(A1 a1, A2 a2, A3 a3, A4 a4)
        {
            return (R) this.fn(new object[] { a1, a2, a3, a4 });
        }

        public R M<A1, A2, A3, A4, A5, R>(A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
        {
            return (R) this.fn(new object[] { a1, a2, a3, a4, a5 });
        }

        public R M<A1, A2, A3, A4, A5, A6, R>(A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
        {
            return (R) this.fn(new object[] { a1, a2, a3, a4, a5, a6 });
        }

        public R M<A1, A2, A3, A4, A5, A6, A7, R>(A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7)
        {
            return (R) this.fn(new object[] { a1, a2, a3, a4, a5, a6, a7 });
        }

        public R M<A1, A2, A3, A4, A5, A6, A7, A8, R>(A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8)
        {
            return (R) this.fn(new object[] { a1, a2, a3, a4, a5, a6, a7, a8 });
        }
    }
}
