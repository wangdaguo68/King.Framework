namespace King.Framework.Common.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class PropertyAccessor
    {
        private static Dictionary<PropertyAccessorKey, PropertyAccessor> propertyAccessors = new Dictionary<PropertyAccessorKey, PropertyAccessor>();
        private static object synchHelper = new object();

        public PropertyAccessor(Type targetType, string propertyName)
        {
            Guard.ArgumentNotNullOrEmpty(targetType, "targetType");
            Guard.ArgumentNotNullOrEmpty(propertyName, "propertyName");
            PropertyInfo property = targetType.GetProperty(propertyName);
            if (null == property)
            {
                throw new ArgumentException(string.Format("Cannot find the property \"{0}\" from the type \"{1}\"", propertyName, targetType.FullName));
            }
            this.Property = property;
            this.TargetType = targetType;
            this.PropertyName = propertyName;
            this.PropertyType = property.PropertyType;
            if (this.Property.CanRead)
            {
                this.GetFunction = this.CreateGetFunction();
            }
            if (this.Property.CanWrite)
            {
                this.SetAction = this.CreateSetAction();
            }
        }

        private Func<object, object> CreateGetFunction()
        {
            ParameterExpression expression = null;
            MethodInfo getMethod = this.Property.GetGetMethod();
            UnaryExpression expression2 = getMethod.IsStatic ? null : Expression.Convert(expression = Expression.Parameter(typeof(object), "target"), this.TargetType);
            return Expression.Lambda<Func<object, object>>(Expression.Convert(Expression.Property(expression2, this.Property), typeof(object)), new ParameterExpression[] { expression }).Compile();
        }

        private Action<object, object> CreateSetAction()
        {
            ParameterExpression expression = null;
            ParameterExpression expression2;
            MethodInfo setMethod = this.Property.GetSetMethod();
            UnaryExpression instance = setMethod.IsStatic ? null : Expression.Convert(expression = Expression.Parameter(typeof(object), "target"), this.TargetType);
            UnaryExpression expression4 = Expression.Convert(expression2 = Expression.Parameter(typeof(object), "value"), this.PropertyType);
            return Expression.Lambda<Action<object, object>>(Expression.Call(instance, setMethod, new Expression[] { expression4 }), new ParameterExpression[] { expression, expression2 }).Compile();
        }

        private void EnsureValidType(object value, string parameterName)
        {
            if (!this.TargetType.IsAssignableFrom(value.GetType()))
            {
                throw new ArgumentException("The target type cannot be assignable from the type of given object.", parameterName);
            }
        }

        public object Get(object obj)
        {
            Guard.ArgumentNotNullOrEmpty(obj, "obj");
            this.EnsureValidType(obj, "obj");
            if (null == this.GetFunction)
            {
                throw new InvalidOperationException(string.Format("The property \"{0}\" of type \"{1}\" is not readable.", this.PropertyName, this.TargetType.FullName));
            }
            return this.GetFunction(obj);
        }

        public static object Get(object obj, string propertyName)
        {
            PropertyAccessor accessor;
            Guard.ArgumentNotNullOrEmpty(obj, "obj");
            Guard.ArgumentNotNullOrEmpty(propertyName, "propertyName");
            PropertyAccessorKey key = new PropertyAccessorKey(obj.GetType(), propertyName);
            if (propertyAccessors.ContainsKey(key))
            {
                accessor = propertyAccessors[key];
                return accessor.Get(obj);
            }
            lock (synchHelper)
            {
                if (propertyAccessors.ContainsKey(key))
                {
                    accessor = propertyAccessors[key];
                    return accessor.Get(obj);
                }
                accessor = new PropertyAccessor(obj.GetType(), propertyName);
                propertyAccessors[key] = accessor;
            }
            return accessor.Get(obj);
        }

        public static Type GetPropertyType(Type targetType, string propertyName)
        {
            Guard.ArgumentNotNullOrEmpty(targetType, "targetType");
            Guard.ArgumentNotNullOrEmpty(propertyName, "propertyName");
            PropertyAccessorKey key = new PropertyAccessorKey(targetType, propertyName);
            if (propertyAccessors.ContainsKey(key))
            {
                return propertyAccessors[key].PropertyType;
            }
            lock (synchHelper)
            {
                if (propertyAccessors.ContainsKey(key))
                {
                    return propertyAccessors[key].PropertyType;
                }
                PropertyAccessor accessor = new PropertyAccessor(targetType, propertyName);
                propertyAccessors[key] = accessor;
                return accessor.PropertyType;
            }
        }

        public void Set(object obj, object value)
        {
            Guard.ArgumentNotNullOrEmpty(obj, "obj");
            this.EnsureValidType(obj, "obj");
            if (null == this.SetAction)
            {
                throw new InvalidOperationException(string.Format("The property \"{0}\" of type \"{1}\" is not writable.", this.PropertyName, this.TargetType.FullName));
            }
            this.SetAction(obj, value);
        }

        public static void Set(object obj, string propertyName, object value)
        {
            PropertyAccessor accessor;
            Guard.ArgumentNotNullOrEmpty(obj, "obj");
            Guard.ArgumentNotNullOrEmpty(propertyName, "propertyName");
            PropertyAccessorKey key = new PropertyAccessorKey(obj.GetType(), propertyName);
            if (propertyAccessors.ContainsKey(key))
            {
                accessor = propertyAccessors[key];
                accessor.Set(obj, value);
            }
            else
            {
                lock (synchHelper)
                {
                    if (propertyAccessors.ContainsKey(key))
                    {
                        accessor = propertyAccessors[key];
                        accessor.Set(obj, value);
                        return;
                    }
                    accessor = new PropertyAccessor(obj.GetType(), propertyName);
                    propertyAccessors[key] = accessor;
                }
                accessor.Set(obj, value);
            }
        }

        public Func<object, object> GetFunction { get; private set; }

        public PropertyInfo Property { get; private set; }

        public string PropertyName { get; private set; }

        public Type PropertyType { get; private set; }

        public Action<object, object> SetAction { get; private set; }

        public Type TargetType { get; private set; }
    }
}
