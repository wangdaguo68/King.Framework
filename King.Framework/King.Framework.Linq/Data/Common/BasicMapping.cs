namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    public abstract class BasicMapping : QueryMapping
    {
        protected BasicMapping()
        {
        }

        public override object CloneEntity(MappingEntity entity, object instance)
        {
            object uninitializedObject = FormatterServices.GetUninitializedObject(entity.EntityType);
            foreach (MemberInfo info in this.GetMappedMembers(entity))
            {
                if (this.IsColumn(entity, info))
                {
                    info.SetValue(uninitializedObject, info.GetValue(instance));
                }
            }
            return uninitializedObject;
        }

        public override QueryMapper CreateMapper(QueryTranslator translator)
        {
            return new BasicMapper(this, translator);
        }

        public virtual IEnumerable<MemberInfo> GetAssociationKeyMembers(MappingEntity entity, MemberInfo member)
        {
            return new MemberInfo[0];
        }

        public virtual IEnumerable<MemberInfo> GetAssociationRelatedKeyMembers(MappingEntity entity, MemberInfo member)
        {
            return new MemberInfo[0];
        }

        public virtual string GetColumnDbType(MappingEntity entity, MemberInfo member)
        {
            return null;
        }

        public virtual string GetColumnName(MappingEntity entity, MemberInfo member)
        {
            return member.Name;
        }

        public override IEnumerable<EntityInfo> GetDependentEntities(MappingEntity entity, object instance)
        {
            foreach (MemberInfo iteratorVariable0 in this.GetMappedMembers(entity))
            {
                if (!this.IsRelationship(entity, iteratorVariable0) || !this.IsRelationshipSource(entity, iteratorVariable0))
                {
                    continue;
                }
                MappingEntity relatedEntity = this.GetRelatedEntity(entity, iteratorVariable0);
                object iteratorVariable2 = iteratorVariable0.GetValue(instance);
                if (iteratorVariable2 != null)
                {
                    IList iteratorVariable3 = iteratorVariable2 as IList;
                    if (iteratorVariable3 != null)
                    {
                        IEnumerator enumerator = iteratorVariable3.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            object current = enumerator.Current;
                            if (current != null)
                            {
                                yield return new EntityInfo(current, relatedEntity);
                            }
                        }
                        continue;
                    }
                    yield return new EntityInfo(iteratorVariable2, relatedEntity);
                }
            }
        }

        public override IEnumerable<EntityInfo> GetDependingEntities(MappingEntity entity, object instance)
        {
            foreach (MemberInfo iteratorVariable0 in this.GetMappedMembers(entity))
            {
                if (!this.IsRelationship(entity, iteratorVariable0) || !this.IsRelationshipTarget(entity, iteratorVariable0))
                {
                    continue;
                }
                MappingEntity relatedEntity = this.GetRelatedEntity(entity, iteratorVariable0);
                object iteratorVariable2 = iteratorVariable0.GetValue(instance);
                if (iteratorVariable2 != null)
                {
                    IList iteratorVariable3 = iteratorVariable2 as IList;
                    if (iteratorVariable3 != null)
                    {
                        IEnumerator enumerator = iteratorVariable3.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            object current = enumerator.Current;
                            if (current != null)
                            {
                                yield return new EntityInfo(current, relatedEntity);
                            }
                        }
                        continue;
                    }
                    yield return new EntityInfo(iteratorVariable2, relatedEntity);
                }
            }
        }

        public override MappingEntity GetEntity(MemberInfo contextMember)
        {
            Type elementType = King.Framework.Linq.TypeHelper.GetElementType(King.Framework.Linq.TypeHelper.GetMemberType(contextMember));
            return this.GetEntity(elementType);
        }

        public override MappingEntity GetEntity(Type elementType, string tableId)
        {
            if (tableId == null)
            {
                tableId = this.GetTableId(elementType);
            }
            return new BasicMappingEntity(elementType, tableId);
        }

        public override IEnumerable<MemberInfo> GetMappedMembers(MappingEntity entity)
        {
            Type entityType = entity.EntityType;
            HashSet<MemberInfo> set = new HashSet<MemberInfo>(from m in entityType.GetFields().Cast<MemberInfo>()
                                                              where this.IsMapped(entity, m)
                                                              select m);
            set.UnionWith(from m in entityType.GetProperties().Cast<MemberInfo>()
                          where this.IsMapped(entity, m)
                          select m);
            return (from m in set
                    orderby m.Name
                    select m);
        }

        public override object GetPrimaryKey(MappingEntity entity, object instance)
        {
            object obj2 = null;
            List<object> list = null;
            foreach (MemberInfo info in this.GetPrimaryKeyMembers(entity))
            {
                if (obj2 == null)
                {
                    obj2 = info.GetValue(instance);
                }
                else
                {
                    if (list == null)
                    {
                        list = new List<object> {
                            obj2
                        };
                    }
                    list.Add(info.GetValue(instance));
                }
            }
            if (list != null)
            {
                return new CompoundKey(list.ToArray());
            }
            return obj2;
        }

        public override Expression GetPrimaryKeyQuery(MappingEntity entity, Expression source, Expression[] keys)
        {
            ParameterExpression expression = Expression.Parameter(entity.ElementType, "p");
            Expression expression2 = null;
            List<MemberInfo> list = this.GetPrimaryKeyMembers(entity).ToList<MemberInfo>();
            if (list.Count != keys.Length)
            {
                throw new InvalidOperationException("Incorrect number of primary key values");
            }
            int index = 0;
            int length = keys.Length;
            while (index < length)
            {
                MemberInfo mi = list[index];
                Type memberType = King.Framework.Linq.TypeHelper.GetMemberType(mi);
                if ((keys[index] != null) && (King.Framework.Linq.TypeHelper.GetNonNullableType(keys[index].Type) != King.Framework.Linq.TypeHelper.GetNonNullableType(memberType)))
                {
                    throw new InvalidOperationException("Primary key value is wrong type");
                }
                Expression expression3 = Expression.MakeMemberAccess(expression, mi).Equal(keys[index]);
                expression2 = (expression2 == null) ? expression3 : expression2.And(expression3);
                index++;
            }
            LambdaExpression expression4 = Expression.Lambda(expression2, new ParameterExpression[] { expression });
            return Expression.Call(typeof(Queryable), "SingleOrDefault", new Type[] { entity.ElementType }, new Expression[] { source, expression4 });
        }

        public virtual MappingEntity GetRelatedEntity(MappingEntity entity, MemberInfo member)
        {
            Type elementType = King.Framework.Linq.TypeHelper.GetElementType(King.Framework.Linq.TypeHelper.GetMemberType(member));
            return this.GetEntity(elementType);
        }

        public virtual string GetTableName(MappingEntity entity)
        {
            return entity.EntityType.Name;
        }

        public virtual bool IsAssociationRelationship(MappingEntity entity, MemberInfo member)
        {
            return false;
        }

        public virtual bool IsColumn(MappingEntity entity, MemberInfo member)
        {
            return this.IsMapped(entity, member);
        }

        public virtual bool IsComputed(MappingEntity entity, MemberInfo member)
        {
            return false;
        }

        public virtual bool IsGenerated(MappingEntity entity, MemberInfo member)
        {
            return false;
        }

        public virtual bool IsMapped(MappingEntity entity, MemberInfo member)
        {
            return true;
        }

        public override bool IsModified(MappingEntity entity, object instance, object original)
        {
            foreach (MemberInfo info in this.GetMappedMembers(entity))
            {
                if (this.IsColumn(entity, info) && !object.Equals(info.GetValue(instance), info.GetValue(original)))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool IsPrimaryKey(MappingEntity entity, MemberInfo member)
        {
            return false;
        }

        public override bool IsRelationship(MappingEntity entity, MemberInfo member)
        {
            return this.IsAssociationRelationship(entity, member);
        }

        public abstract bool IsRelationshipSource(MappingEntity entity, MemberInfo member);
        public abstract bool IsRelationshipTarget(MappingEntity entity, MemberInfo member);
        public virtual bool IsUpdatable(MappingEntity entity, MemberInfo member)
        {
            return !this.IsPrimaryKey(entity, member);
        }

        
}

public class BasicMappingEntity : MappingEntity
{
    private string entityID;
    private Type type;

    public BasicMappingEntity(Type type, string entityID)
    {
        this.entityID = entityID;
        this.type = type;
    }

    public override Type ElementType
    {
        get
        {
            return this.type;
        }
    }

    public override Type EntityType
    {
        get
        {
            return this.type;
        }
    }

    public override string TableId
    {
        get
        {
            return this.entityID;
        }
    }
}
    }

