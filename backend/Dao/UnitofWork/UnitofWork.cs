using Microsoft.EntityFrameworkCore.Storage;
using System.Collections;
using webapi.Base;
using webapi.Dao.Repository;
using webapi.Data;

namespace webapi.Dao.UnitofWork
{
    public class UnitofWork : IUnitofWork
    {
        private DataContext context;
        private Hashtable _repositories;
        private IDbContextTransaction _transaction; // Thêm biến để theo dõi giao dịch

        public UnitofWork(DataContext context)
        {
            this.context = context;
        }

        public async Task<int> Complete()
        {
           
                    await context.SaveChangesAsync();

            return 1;
        }

    

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseCreateDate
        {
            if (_repositories == null) { _repositories = new Hashtable(); }

            var type = typeof(TEntity).Name;
            if (!_repositories.ContainsKey(type))
            {
                var repo = new GenericRepository<TEntity>(context);
                _repositories.Add(type, repo);
            }
            return _repositories[type] as IGenericRepository<TEntity>;
        }
    }

}
