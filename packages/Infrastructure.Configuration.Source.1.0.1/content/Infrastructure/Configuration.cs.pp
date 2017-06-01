using System;
using System.Configuration;
using System.Data.Common;
using System.Transactions;
using Infrastructure.DI.Spring;
using Infrastructure.Seedwork.Configuration;
using Infrastructure.Seedwork.DI;
using Infrastructure.Seedwork.Data;
using log4net;

namespace $rootnamespace$.Infrastructure
{
    /// <summary>
    /// Allows configurtion of components in <c>Infrastructure.Web</c>.
    /// </summary>
    /// <author>Vamsee M Kamabathula</author>
    public static class Configuration
    {
    // ReSharper disable InconsistentNaming
        private static readonly ILog log = LogManager.GetLogger(typeof(Configuration));
    // ReSharper restore InconsistentNaming

        private static Func<IContainer, UnitOfWorkScope> _unitOfWorkScopeFactory;
        private static string _connectionString;

        static Configuration()
        {
            var settings = new UnitOfWorkScopeSettings(
                TransactionScopeOption.RequiresNew,
                new TransactionOptions
                    {
                        IsolationLevel =
                            IsolationLevel.ReadCommitted,
                        Timeout = TimeSpan.FromSeconds(60)
                    });

            UnitOfWorkScopeFactory = container =>
                                     new UnitOfWorkScope(container, settings);
        }

        /// <summary>
        /// Returns the factory method to be used for creating <see cref="UnitOfWorkScope"/> instances.
        /// </summary>
        /// <remarks>
        /// This never returns <c>null</c>.
        /// </remarks>
        public static Func<IContainer, UnitOfWorkScope> UnitOfWorkScopeFactory
        {
            get { return _unitOfWorkScopeFactory; }
            set
            {
                if(value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _unitOfWorkScopeFactory = value;
            }
        }
        
        public static string ConnectionString
        {
        	get
        	{
        		//TODO:: Read connection string information from configuration file.
        		return _connectionString ??
        	           (_connectionString = ConfigurationManager.ConnectionStrings["SqlConnection"].ConnectionString);
	        }
        }
        
        public static DbProviderFactory DbProviderFactory
        {
        	get
        	{
        		//TODO:: Configure provider name as per selected persistence storage
        		// Use Provider name "System.Data.SqlClient" for SQL Server
        		// Use provider name "Oracle.DataAccess.Client" for Oracle
        		return DbProviderFactories.GetFactory("System.Data.SqlClient");
        	}
        }
        
        public static IContainer InitializeInfrastructure()
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Initializing infrastructure");
            }

            IConfigurableContainer rootContainer = new SpringContainer();
            //ApplicationUtil.Initialize(rootContainer);

            ScanAssembliesAndConfigureTypes(rootContainer);

            return rootContainer;
        }

        private static void ScanAssembliesAndConfigureTypes(IConfigurableContainer rootContainer)
        {
            // TODO:: Configure assembly name as per your requirement
            IAssemblyScanner scanner = new AssemblyScanner(rootContainer);
            scanner.AssembliesFromApplicationBaseDirectory(
                (string assemblyName) =>
                assemblyName.StartsWith("$rootnamespace$", StringComparison.OrdinalIgnoreCase));

            scanner.ForAllTypes<IConfigureContainer>(t =>
                {
                    if (t.IsAbstract || t.IsInterface) return;

                    var configurer = (IConfigureContainer) Activator.CreateInstance(t);
                    configurer.Configure(rootContainer);
                });
        }
    }
}
