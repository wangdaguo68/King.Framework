namespace King.Framework.Ioc
{
    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;

    public static class IocHelper
    {
        private static readonly UnityContainer defaultContainer = new UnityContainer();

        public static IEnumerable<TService> GetAllInstances<TService>()
        {
            return defaultContainer.ResolveAll<TService>(new ResolverOverride[0]);
        }

        public static IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return defaultContainer.ResolveAll(serviceType, new ResolverOverride[0]);
        }

        public static TService GetInstance<TService>()
        {
            return defaultContainer.Resolve<TService>(new ResolverOverride[0]);
        }

        public static TService GetInstance<TService>(string key)
        {
            return defaultContainer.Resolve<TService>(key, new ResolverOverride[0]);
        }

        public static object GetInstance(Type serviceType)
        {
            return defaultContainer.Resolve(serviceType, new ResolverOverride[0]);
        }

        public static object GetInstance(Type serviceType, string key)
        {
            return defaultContainer.Resolve(serviceType, key, new ResolverOverride[0]);
        }

        internal static IServiceLocator GetServiceLocator()
        {
            return new UnityServiceLocator(defaultContainer);
        }

        public static bool IsRegistered(Type interfaceType)
        {
            return defaultContainer.IsRegistered(interfaceType);
        }

        public static void RegisterSingleInstanceType<TInterface, TImplemention>() where TImplemention : TInterface
        {
            defaultContainer.RegisterType<TInterface, TImplemention>(new ContainerControlledLifetimeManager(), new InjectionMember[0]);
        }

        public static void RegisterType<TInterface, TImplemention>() where TImplemention : TInterface
        {
            defaultContainer.RegisterType<TInterface, TImplemention>(new InjectionMember[0]);
        }

        public static void RegisterType<TInterface, TImplementation>(string key) where TImplementation : TInterface
        {
            defaultContainer.RegisterType<TInterface, TImplementation>(key, new InjectionMember[0]);
        }

        public static T Resolve<T>()
        {
            return defaultContainer.Resolve<T>(new ResolverOverride[0]);
        }

        public static T Resolve<T>(string key)
        {
            return defaultContainer.Resolve<T>(key, new ResolverOverride[0]);
        }
    }
}
