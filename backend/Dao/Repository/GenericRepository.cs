using Microsoft.EntityFrameworkCore;
using webapi.Base;
using webapi.Dao.Specification;
using webapi.Data;

namespace webapi.Dao.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseCreateDate
    {
        private DataContext context;
        public GenericRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task AddAsync(T entity)
        {
            await context.Set<T>().AddAsync(entity);
        }

        public async Task Delete(T entity)
        {
            context.Set<T>().Remove(entity);
        }
        public async Task Update(T entity)
        {
            context.Set<T>().Update(entity);
        }
        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await context.Set<T>().ToListAsync();
        }
        //Lấy tất cả bản ghi theo điều kiện
        public async Task<IReadOnlyList<T>> GetAllWithAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await context.Set<T>().FindAsync(id);
        }
        // Đếm số lượng phần tử trả ra theo điều kiện
        public async Task<int> GetCountWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }
        // Lấy phần tử đầu tiên theo điều kiện
        public async Task<T> GetEntityWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }   
        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(context.Set<T>(), spec);
        }

        public async Task DeleteRange(IReadOnlyList<T> entity)
        {
            context.Set<T>().RemoveRange(entity); 
        }
    }
}
