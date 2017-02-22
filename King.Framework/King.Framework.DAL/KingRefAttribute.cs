namespace King.Framework.DAL
{
    using System;

    public class KingRefAttribute : Attribute
    {
        private readonly string _collectionNavigationPropertyName;
        private readonly string _objectNavigationPropertyName;
        private readonly Type _refType;

        public KingRefAttribute(Type refType)
        {
            this._refType = refType;
        }

        public KingRefAttribute(Type refType, string objectNavigationPropertyName)
        {
            this._refType = refType;
            this._objectNavigationPropertyName = objectNavigationPropertyName;
        }

        public KingRefAttribute(Type refType, string objectNavigationPropertyName, string collectionNavigationPropertyName)
        {
            this._refType = refType;
            this._objectNavigationPropertyName = objectNavigationPropertyName;
            this._collectionNavigationPropertyName = collectionNavigationPropertyName;
        }

        public string CollectionNavigationPropertyName
        {
            get
            {
                return this._collectionNavigationPropertyName;
            }
        }

        public string ObjectNavigationPropertyName
        {
            get
            {
                return this._objectNavigationPropertyName;
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
