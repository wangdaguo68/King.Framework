namespace King.Framework.Manager
{
    using King.Framework.Repository.Schemas;
    using System;
    using System.Runtime.CompilerServices;

    public class DeleteInfo
    {
        public override bool Equals(object obj)
        {
            return (this.GetHashCode() == obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            return (this.EntitySchema.EntityType.GetHashCode() ^ this.Id.GetHashCode());
        }

        public IEntitySchema EntitySchema { get; set; }

        public string Group
        {
            get
            {
                string str = (this.RefObject == null) ? "_____NOFileld_____" : this.RefObject.ReferenceField;
                return string.Format("{0}_{1}", this.EntitySchema.EntityName, str);
            }
        }

        public object Id { get; set; }

        public string Key
        {
            get
            {
                return string.Format("{0}_{1}_{2}", this.EntitySchema.EntityName, this.RefObject.ReferenceField, this.Id);
            }
        }

        public ReferencedObject RefObject { get; set; }
    }
}
