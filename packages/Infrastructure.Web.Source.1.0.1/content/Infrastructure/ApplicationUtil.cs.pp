using System;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using Infrastructure.Seedwork.DI;
using $rootnamespace$.Infrastructure.DI;
using log4net;

namespace $rootnamespace$.Infrastructure
{
    /// <summary>
    /// Contains methods to initialization a web application.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <example>
    /// The following example demonstrates the hooks required in Global.asax to integrated
    /// <c>Infrastructure.Web</c> into your application:
    /// <code lang="C#" source="Infrastructure\ApplicationUtil-example.txt"/>
    /// </example>
    /// <seealso cref="DependencyResolverAdapter"/>
       /// <author>Vamsee M Kamabathula</author>
    public static class ApplicationUtil
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ApplicationUtil));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootContainer"></param>
        public static void Initialize(IConfigurableContainer rootContainer)
        {
            Initialize(rootContainer, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootContainer">The root IoC container of the application.</param>
        /// <param name="registerGlobalFilters"></param>
        public static void Initialize(IConfigurableContainer rootContainer, bool registerGlobalFilters)
        {
            if (rootContainer == null) throw new ArgumentNullException("rootContainer");

            var resolver = new DependencyResolverAdapter(rootContainer);

            // this is for MVC3
            DependencyResolver.SetResolver(resolver);

            // this is the new way of setting a resolver in MVC4 Web Api
            GlobalConfiguration.Configuration.DependencyResolver = resolver;

            rootContainer.RegisterSingleton(typeof(IHttpControllerActivator), resolver);
        }
    }
}
