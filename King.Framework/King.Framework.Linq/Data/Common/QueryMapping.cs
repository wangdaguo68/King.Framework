namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public abstract class QueryMapping
    {
        protected QueryMapping()
        {
        }

        public virtual bool CanBeEvaluatedLocally(Expression expression)
        {
            ConstantExpression expression2 = expression as ConstantExpression;
            if (expression2 != null)
            {
                IQueryable queryable = expression2.Value as IQueryable;
                if ((queryable != null) && (queryable.Provider == this))
                {
                    return false;
                }
            }
            MethodCallExpression expression3 = expression as MethodCallExpression;
            if ((expression3 != null) && (((expression3.Method.DeclaringType == typeof(Enumerable)) || (expression3.Method.DeclaringType == typeof(Queryable))) || (expression3.Method.DeclaringType == typeof(Updatable))))
            {
                return false;
            }
            return (((expression.NodeType == ExpressionType.Convert) && (expression.Type == typeof(object))) || ((expression.NodeType != ExpressionType.Parameter) && (expression.NodeType != ExpressionType.Lambda)));
        }

        public abstract object CloneEntity(MappingEntity entity, object instance);
        public abstract QueryMapper CreateMapper(QueryTranslator translator);
        public abstract IEnumerable<EntityInfo> GetDependentEntities(MappingEntity entity, object instance);
        public abstract IEnumerable<EntityInfo> GetDependingEntities(MappingEntity entity, object instance);
        public abstract MappingEntity GetEntity(MemberInfo contextMember);
        public virtual MappingEntity GetEntity(Type type)
        {
            return this.GetEntity(type, this.GetTableId(type));
        }

        public abstract MappingEntity GetEntity(Type elementType, string entityID);
        public abstract IEnumerable<MemberInfo> GetMappedMembers(MappingEntity entity);
        public abstract object GetPrimaryKey(MappingEntity entity, object instance);
        public virtual IEnumerable<MemberInfo> GetPrimaryKeyMembers(MappingEntity entity)
        {
            return (from m in this.GetMappedMembers(entity)
                where this.IsPrimaryKey(entity, m)
                select m);
        }

        public abstract Expression GetPrimaryKeyQuery(MappingEntity entity, Expression source, Expression[] keys);
        public virtual string GetTableId(Type type)
        {
            return type.Name;
        }

        public abstract bool IsModified(MappingEntity entity, object instance, object original);
        public abstract bool IsPrimaryKey(MappingEntity entity, MemberInfo member);
        public abstract bool IsRelationship(MappingEntity entity, MemberInfo member);
        public virtual bool IsSingletonRelationship(MappingEntity entity, MemberInfo member)
        {
            if (!this.IsRelationship(entity, member))
            {
                return false;
            }
            return (King.Framework.Linq.TypeHelper.FindIEnumerable(King.Framework.Linq.TypeHelper.GetMemberType(member)) == null);
        }
    }
}
