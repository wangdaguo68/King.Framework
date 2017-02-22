namespace King.Framework.Common
{
    using System;

    public class BusinessLogicAttribute : Attribute
    {
        private readonly string _description;
        private readonly string _entityName;

        public BusinessLogicAttribute(string entityName, string description)
        {
            if (entityName == null)
            {
                throw new ArgumentNullException("entityName");
            }
            this._description = description;
            this._entityName = entityName;
        }

        public BusinessLogicAttribute(Type type, string description)
        {
            this._description = description;
            this._entityName = type.Name;
        }

        public string Description
        {
            get
            {
                return this._description;
            }
        }

        public string EntityName
        {
            get
            {
                return this._entityName;
            }
        }
    }
}
