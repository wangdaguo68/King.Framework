namespace King.Framework.DAL
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class KingCollectionAttribute : Attribute
    {
        private readonly string _refPropertyName;
        private readonly Type _refType;

        public KingCollectionAttribute(string refPropertyName)
        {
            this._refPropertyName = refPropertyName;
            this._refType = null;
        }

        public string RefPropertyName
        {
            get
            {
                return this._refPropertyName;
            }
        }

        public Type RefType
        {
            get
            {
                return this._refType;
            }
        }
    }
}
