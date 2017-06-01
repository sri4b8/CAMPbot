using Infrastructure.Seedwork.Data;
using System.Data.Common;

namespace CAMPBot.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>Murali Poola</author>
    public class AppUnitOfWork : AdoUnitOfWork
    {
        public AppUnitOfWork(DbProviderFactory dbProviderFactory, string connectionString) :
            base(dbProviderFactory, connectionString)
        {

        }
    }
}