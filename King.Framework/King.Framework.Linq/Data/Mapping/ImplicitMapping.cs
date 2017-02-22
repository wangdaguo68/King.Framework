namespace King.Framework.Linq.Data.Mapping
{
    using King.Framework.Linq;
    using King.Framework.Linq.Data.Common;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;

    public class ImplicitMapping : BasicMapping
    {
        public override IEnumerable<MemberInfo> GetAssociationKeyMembers(MappingEntity entity, MemberInfo member)
        {
            List<MemberInfo> list;
            List<MemberInfo> list2;
            this.GetAssociationKeys(entity, member, out list, out list2);
            return list;
        }

        private void GetAssociationKeys(MappingEntity entity, MemberInfo member, out List<MemberInfo> keyMembers, out List<MemberInfo> relatedKeyMembers)
        {
            MappingEntity entity2 = this.GetRelatedEntity(entity, member);
            Dictionary<string, MemberInfo> dictionary = (from m in this.GetMappedMembers(entity)
                where this.IsColumn(entity, m)
                select m).ToDictionary<MemberInfo, string>(m => m.Name);
            Dictionary<string, MemberInfo> dictionary2 = (from m in this.GetMappedMembers(entity2)
                where this.IsColumn(entity2, m)
                select m).ToDictionary<MemberInfo, string>(m => m.Name);
            IOrderedEnumerable<string> enumerable = from k in dictionary.Keys.Intersect<string>(dictionary2.Keys)
                orderby k
                select k;
            keyMembers = new List<MemberInfo>();
            relatedKeyMembers = new List<MemberInfo>();
            foreach (string str in enumerable)
            {
                keyMembers.Add(dictionary[str]);
                relatedKeyMembers.Add(dictionary2[str]);
            }
        }

        public override IEnumerable<MemberInfo> GetAssociationRelatedKeyMembers(MappingEntity entity, MemberInfo member)
        {
            List<MemberInfo> list;
            List<MemberInfo> list2;
            this.GetAssociationKeys(entity, member, out list, out list2);
            return list2;
        }

        public override string GetTableId(Type type)
        {
            return this.InferTableName(type);
        }

        public override string GetTableName(MappingEntity entity)
        {
            return (!string.IsNullOrEmpty(entity.TableId) ? entity.TableId : this.InferTableName(entity.EntityType));
        }

        private string InferTableName(Type rowType)
        {
            return SplitWords(Plural(rowType.Name));
        }

        public override bool IsAssociationRelationship(MappingEntity entity, MemberInfo member)
        {
            if (!(!this.IsMapped(entity, member) || this.IsColumn(entity, member)))
            {
                Type elementType = King.Framework.Linq.TypeHelper.GetElementType(King.Framework.Linq.TypeHelper.GetMemberType(member));
                return !this.IsScalar(elementType);
            }
            return false;
        }

        public override bool IsColumn(MappingEntity entity, MemberInfo member)
        {
            return this.IsScalar(King.Framework.Linq.TypeHelper.GetMemberType(member));
        }

        public override bool IsPrimaryKey(MappingEntity entity, MemberInfo member)
        {
            return (this.IsColumn(entity, member) && (member.Name.EndsWith("ID") && member.DeclaringType.Name.StartsWith(member.Name.Substring(0, member.Name.Length - 2))));
        }

        public override bool IsRelationshipSource(MappingEntity entity, MemberInfo member)
        {
            if (this.IsAssociationRelationship(entity, member))
            {
                if (typeof(IEnumerable).IsAssignableFrom(King.Framework.Linq.TypeHelper.GetMemberType(member)))
                {
                    return false;
                }
                MappingEntity relatedEntity = this.GetRelatedEntity(entity, member);
                HashSet<string> other = new HashSet<string>(from m in this.GetPrimaryKeyMembers(relatedEntity) select m.Name);
                HashSet<string> set2 = new HashSet<string>(from m in this.GetAssociationRelatedKeyMembers(entity, member) select m.Name);
                return (other.IsSubsetOf(set2) && set2.IsSubsetOf(other));
            }
            return false;
        }

        public override bool IsRelationshipTarget(MappingEntity entity, MemberInfo member)
        {
            if (this.IsAssociationRelationship(entity, member))
            {
                if (typeof(IEnumerable).IsAssignableFrom(King.Framework.Linq.TypeHelper.GetMemberType(member)))
                {
                    return true;
                }
                HashSet<string> other = new HashSet<string>(from m in this.GetPrimaryKeyMembers(entity) select m.Name);
                HashSet<string> set2 = new HashSet<string>(from m in this.GetAssociationKeyMembers(entity, member) select m.Name);
                return (set2.IsSubsetOf(other) && other.IsSubsetOf(set2));
            }
            return false;
        }

        private bool IsScalar(Type type)
        {
            type = King.Framework.Linq.TypeHelper.GetNonNullableType(type);
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Empty:
                case TypeCode.DBNull:
                    return false;

                case TypeCode.Object:
                    return ((((type == typeof(DateTimeOffset)) || (type == typeof(TimeSpan))) || (type == typeof(Guid))) || (type == typeof(byte[])));
            }
            return true;
        }

        private string NameWithoutTrailingDigits(string name)
        {
            int length = name.Length - 1;
            while ((length >= 0) && char.IsDigit(name[length]))
            {
                length--;
            }
            if (length < (name.Length - 1))
            {
                return name.Substring(0, length);
            }
            return name;
        }

        public static string Plural(string name)
        {
            if ((name.EndsWith("x", StringComparison.InvariantCultureIgnoreCase) || name.EndsWith("ch", StringComparison.InvariantCultureIgnoreCase)) || name.EndsWith("ss", StringComparison.InvariantCultureIgnoreCase))
            {
                return (name + "es");
            }
            if (name.EndsWith("y", StringComparison.InvariantCultureIgnoreCase))
            {
                return (name.Substring(0, name.Length - 1) + "ies");
            }
            if (!name.EndsWith("s"))
            {
                return (name + "s");
            }
            return name;
        }

        public static string Singular(string name)
        {
            if (name.EndsWith("es", StringComparison.InvariantCultureIgnoreCase))
            {
                string str = name.Substring(0, name.Length - 2);
                if ((str.EndsWith("x", StringComparison.InvariantCultureIgnoreCase) || name.EndsWith("ch", StringComparison.InvariantCultureIgnoreCase)) || name.EndsWith("ss", StringComparison.InvariantCultureIgnoreCase))
                {
                    return str;
                }
            }
            if (name.EndsWith("ies", StringComparison.InvariantCultureIgnoreCase))
            {
                return (name.Substring(0, name.Length - 3) + "y");
            }
            if (!(!name.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) || name.EndsWith("ss", StringComparison.InvariantCultureIgnoreCase)))
            {
                return name.Substring(0, name.Length - 1);
            }
            return name;
        }

        public static string SplitWords(string name)
        {
            StringBuilder builder = null;
            bool flag = char.IsLower(name[0]);
            int count = 0;
            int length = name.Length;
            while (count < length)
            {
                bool flag2 = char.IsLower(name[count]);
                if (flag && !flag2)
                {
                    if (builder == null)
                    {
                        builder = new StringBuilder();
                        builder.Append(name, 0, count);
                    }
                    builder.Append(" ");
                }
                if (builder != null)
                {
                    builder.Append(name[count]);
                }
                flag = flag2;
                count++;
            }
            if (builder != null)
            {
                return builder.ToString();
            }
            return name;
        }
    }
}
