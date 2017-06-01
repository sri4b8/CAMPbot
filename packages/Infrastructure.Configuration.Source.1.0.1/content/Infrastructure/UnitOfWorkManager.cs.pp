using Infrastructure.Seedwork.Data;

namespace $rootnamespace$.Infrastructure
{

    /// <summary>
    /// 
    /// </summary>
    /// <author>Murali Poola</author>
    public class UnitOfWorkManager : AbstractUnitOfWorkManager
    {
        public UnitOfWorkManager(AppUnitOfWork unitOfWork)
            : base(unitOfWork)
        {

        }
    }
}