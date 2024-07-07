using webapi.Base;
using webapi.Dao.Specification;

namespace webapi.Dao.Repository
{
    public interface IGenericRepository<T>
        where T : BaseCreateDate
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);

        //Có điền kiện
        Task<IReadOnlyList<T>> GetAllWithAsync(ISpecification<T> spec);

        Task AddAsync(T entity);
        Task Update( T entity);
        Task Delete( T entity);
        Task<T> GetEntityWithSpecAsync(ISpecification<T> spec);
        Task<int> GetCountWithSpecAsync(ISpecification<T> spec);
        Task DeleteRange(IReadOnlyList<T> entity);

    }
}
