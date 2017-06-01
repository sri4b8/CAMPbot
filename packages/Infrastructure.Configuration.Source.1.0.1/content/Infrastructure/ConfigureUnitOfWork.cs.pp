using Infrastructure.Seedwork.DI;
using $rootnamespace$.Infrastructure;

namespace $rootnamespace$.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>Murali Poola</author>
    public class ConfigureUnitOfWork : IConfigureContainer
    {
        public void Configure(IConfigurableContainer container)
        {
            var dbProviderFactory = Configuration.DbProviderFactory;
            var connectionString = Configuration.ConnectionString;

            container.RegisterType(c => new AppUnitOfWork(dbProviderFactory, connectionString), true, true);
            container.RegisterType(typeof(UnitOfWorkManager), false, true);
        }
    }
}