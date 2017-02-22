using King.Framework.Interfaces;
using King.Framework.Repository.Schemas;

namespace King.Framework.Repository
{
    using King.Framework.Ioc;
    using System;

    public static class AppInit
    {
        public static void Start()
        {
            IocHelper.RegisterType<IEntityProvider, MetaEntityProvider>();
        }
    }
}

