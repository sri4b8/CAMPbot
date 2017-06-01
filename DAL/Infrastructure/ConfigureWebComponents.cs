using Infrastructure.Seedwork.DI;
using log4net;
using System;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace DAL.Infrastructure
{
    /// <summary>
    /// Registers web components with DI Container
    /// </summary>
    /// <author>Vamsee M Kambathula</author>
    public class ConfigureWebComponents : IConfigureContainer
    {
        // ReSharper disable InconsistentNaming
        private static readonly ILog log = LogManager.GetLogger(typeof(ConfigureWebComponents));
        // ReSharper restore InconsistentNaming

        #region Implementation of IConfigureContainer

        void IConfigureContainer.Configure(IConfigurableContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");

            Type[] types = typeof(ConfigureWebComponents).Assembly.GetTypes();

            int controllerCount = types
                .Where(t => typeof(IController).IsAssignableFrom(t))
                .Aggregate(0, (i, controllerType) =>
                {
                    // Vamsee: 13-May-2013: Controller need not to be in unit of work.
                    // Only actions are part of unit of work.
                    container.RegisterType(controllerType, false, false);
                    return i + 1;
                });

            int httpControllerCount = types
                .Where(t => typeof(IHttpController).IsAssignableFrom(t))
                .Aggregate(0, (i, apiControllerType) =>
                {
                    // Vamsee: 13-May-2013: Controller need not to be in unit of work.
                    // Only actions are part of unit of work.
                    container.RegisterType(apiControllerType, false, false);
                    return i + 1;
                });

            int modulesCount = types
                .Where(t => typeof(IHttpModule).IsAssignableFrom(t))
                .Aggregate(0, (i, moduleType) =>
                {
                    // vamsee, srinivas: 24-Jan-2013: We don't want modules to be in a unit of work.
                    // each method in the module should begin and end a unit of work.
                    container.RegisterType(moduleType, false, false);
                    return i + 1;
                });

            if (log.IsInfoEnabled)
            {
                log.InfoFormat("Registered web components: controller={0}, httpControllers={1}, modules={2}",
                    controllerCount, httpControllerCount, modulesCount);
            }
        }

        #endregion
    }
}