namespace King.Framework.Repository.Schemas
{
    using System;
    using System.Runtime.CompilerServices;

    public class ReferencedObject
    {
        public override string ToString()
        {
            return string.Format("EntityType :{0} , ReferenceField :{1} , DeleteFlag :{2} , ReferenceType :{3}", new object[] { this.EntityType, this.ReferenceField, this.DeleteFlag, this.ReferenceType });
        }

        public King.Framework.Repository.Schemas.DeleteFlag DeleteFlag { get; set; }

        public Type EntityType { get; set; }

        public string ReferenceField { get; set; }

        public King.Framework.Repository.Schemas.ReferenceType ReferenceType { get; set; }
    }
}

