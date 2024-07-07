using System.Linq.Expressions;
using webapi.Base;

namespace webapi.Dao.Specification
{
    public interface ISpecification<T> where T : BaseCreateDate
    {
        //điều kiện where
        Expression<Func<T, bool>> Criteria { get; set; }
        //điều kiện join
        List<Expression<Func<T, object>>> Includes { get; set; }
        Expression<Func<T, object>> OrderBy { get; set; }
        Expression<Func<T, object>> OrderByDescending { get; set; }
        // phân trang
        int Take { get; set; }
        int Skip { get; set; }
        bool IsPaginationEnabled { get; set; }
    }
}
