namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct EntityInfo
    {
        private object instance;
        private MappingEntity mapping;
        public EntityInfo(object instance, MappingEntity mapping)
        {
            this.instance = instance;
            this.mapping = mapping;
        }

        public object Instance
        {
            get
            {
                return this.instance;
            }
        }
        public MappingEntity Mapping
        {
            get
            {
                return this.mapping;
            }
        }
    }
}
