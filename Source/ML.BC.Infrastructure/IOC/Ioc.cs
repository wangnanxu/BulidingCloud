using System;
using System.Reflection;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;
using ML.BC.Infrastructure.Aop;

namespace ML.BC.Infrastructure
{
    public class Ioc
    {
        private static readonly UnityContainer _container;

        static Ioc()
        {
            _container = new UnityContainer();

            UnityConfigurationSection configuration = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            configuration.Configure(_container);
        }

        public static void Register<TInterface, TImpmentation>() where TImpmentation : TInterface
        {
            _container.RegisterType<TInterface, TImpmentation>();
        }

        public static void RegisterInheritedTypes(Assembly assembly, Type baseType)
        {
            _container.RegisterInheritedTypes(assembly, baseType);
        }

        public static T GetService<T>()
        {
            var temp = _container.Resolve<T>();
            if (!(temp is IAop)) return temp;

            var enableLogFunctionData = false;
            bool.TryParse(ConfigurationManager.AppSettings["EnableLogFunctionData"], out enableLogFunctionData);

            var enableCustomCache = false;
            bool.TryParse(ConfigurationManager.AppSettings["EnableCustomCache"], out enableCustomCache);

            if (!enableLogFunctionData && !enableCustomCache) return temp;

            var tempProxy = new AopProxy<T>(temp);
            var proxyService = (T)tempProxy.GetTransparentProxy();
            return proxyService;
        }
    }
}
