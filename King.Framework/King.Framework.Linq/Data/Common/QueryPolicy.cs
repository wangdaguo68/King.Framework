namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Reflection;

    public class QueryPolicy
    {
        public static readonly QueryPolicy Default = new QueryPolicy();

        public virtual QueryPolice CreatePolice(QueryTranslator translator)
        {
            return new QueryPolice(this, translator);
        }

        public virtual bool IsDeferLoaded(MemberInfo member)
        {
            return false;
        }

        public virtual bool IsIncluded(MemberInfo member)
        {
            return false;
        }
    }
}
