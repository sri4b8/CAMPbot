using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Routing;
using Infrastructure.Seedwork.DI;
using Infrastructure.Seedwork.Data;
using log4net;
using IDependencyResolver = System.Web.Http.Dependencies.IDependencyResolver;

namespace DAL.Infrastructure.DI
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>Vamsee M Kamabathula</author>
    public class DependencyResolverAdapter :
       IDependencyResolver,
       System.Web.Mvc.IDependencyResolver,
       IControllerActivator,
        System.Web.Http.Dispatcher.IHttpControllerActivator
    {
        private readonly UnitOfWorkScope _unitOfWorkScope;
        private static readonly ILog log = LogManager.GetLogger(typeof(DependencyResolverAdapter));

        private IContainer _container;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        public DependencyResolverAdapter(IContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");
            _container = container;
        }

        #region IDependencyResolver

        IDependencyScope IDependencyResolver.BeginScope()
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("BeginScope() ");
            }
            return this;
        }

        object IDependencyScope.GetService(Type serviceType)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("GetService [{0}]", serviceType);
            }
            if (_container.HasType(serviceType))
            {
                return _container.Build(serviceType);
            }
            return null;
        }

        IEnumerable<object> IDependencyScope.GetServices(Type serviceType)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("GetServices [{0}]", serviceType);
            }
            return _container.BuildAll(serviceType);
        }

        void IDisposable.Dispose()
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Disposing resolver adapter");
            }
       
            //if(_container != null)
            //{
            //    _container.Dispose();
            //    _container = null;    
            //}
        }

        #endregion

        #region System.Web.Mvc.IDependencyResolver

        object System.Web.Mvc.IDependencyResolver.GetService(Type serviceType)
        {
            if (typeof(IControllerActivator) == (serviceType))
            {
                return this;
            }
            if (_container.HasType(serviceType))
            {
                return _container.Build(serviceType);
            }
            return null;
        }

        IEnumerable<object> System.Web.Mvc.IDependencyResolver.GetServices(Type serviceType)
        {
            return _container.BuildAll(serviceType);
        }

        #endregion


        #region IControllerActivator

        IController IControllerActivator.Create(RequestContext requestContext, Type controllerType)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("ControllerActivator.Create called: {0}", controllerType);
                }
                if (_container == null)
                {
                    if (log.IsWarnEnabled)
                    {
                        log.WarnFormat("IControllerActivator.Create: container is null");
                    }
                    throw new InvalidOperationException("IControllerActivator.Create: container is null");
                }

                if (_container.HasType(controllerType))
                {
                    return _container.Build(controllerType) as IController;
                }
            }
            catch (Exception exp)
            {
                if (log.IsErrorEnabled)
                {
                    log.ErrorFormat("Error while creating Controller of type [{0}] Exception :[{1}]", controllerType, exp);
                }

                throw;
            }
            return null;
        }

        #endregion

        #region Implementation of IHttpControllerActivator

        IHttpController IHttpControllerActivator.Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("IHttpControllerActivator.Create: controllerType={0}", controllerType);
            }
            try
            {
                var scopeResolver = (DependencyResolverAdapter)request.GetDependencyScope();

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("Got the scoped resolver instance");
                }

                var container = scopeResolver._container;

                if (container.HasType(controllerType))
                {
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("Container has controllerType={0}", controllerType);
                    }
                    return container.Build(controllerType) as IHttpController;
                }

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("controllerType {0} not defined in container", controllerType);
                }
            }
            catch (Exception exp)
            {
                if (log.IsErrorEnabled)
                {
                    log.ErrorFormat("Error while creating API Controller of type [{0}] Exception :[{1}]", controllerType, exp);
                }
                throw;
            }

            return null;
        }

        #endregion
    }
}
