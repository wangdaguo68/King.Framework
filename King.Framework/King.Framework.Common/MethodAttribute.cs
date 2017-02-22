namespace King.Framework.Common
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public class MethodAttribute : Attribute
    {
        private string description;
        private string operationName = "";

        public MethodAttribute(Type entityType, OperationEnum operationEnum, string description = "")
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType");
            }
            this.operationName = operationEnum.ToString();
            this.description = description;
            this.EntityType = entityType;
        }

        public string Description
        {
            get
            {
                return this.description;
            }
        }

        public Type EntityType { get; set; }

        public string OperationName
        {
            get
            {
                return this.operationName;
            }
        }
    }
}
