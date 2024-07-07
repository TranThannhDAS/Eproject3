using Microsoft.EntityFrameworkCore;
using webapi.Base;

namespace webapi.Dao.Specification
{
    public static class SpecificationEvaluator<TEntity> where TEntity : BaseCreateDate
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> InputQuery, ISpecification<TEntity> spec)
        {
            var query = InputQuery;

            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);


            if (spec.OrderByDescending != null)
                query = query.OrderBy(spec.OrderByDescending);


            if (spec.IsPaginationEnabled)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }

            if (spec.Includes != null)
            {
                query = spec.Includes.Aggregate(query, (currentQuery, includeProperty) => currentQuery.Include(includeProperty));
            }

            return query;
        }
    }
}
