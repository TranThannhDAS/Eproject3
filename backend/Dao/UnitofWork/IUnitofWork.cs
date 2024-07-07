using webapi.Base;
using webapi.Dao.Repository;
namespace webapi.Dao.UnitofWork
{
    public interface IUnitofWork
    {
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseCreateDate;

        Task<int> Complete();
    }
}
