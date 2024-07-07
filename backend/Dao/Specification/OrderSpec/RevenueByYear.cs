using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.OrderSpec
{
    public class RevenueByYear : BaseSpecification<Order>
    {
        public RevenueByYear(int year)
        : base(query => query.CreateDate.Year == year )
        {
        }
    }
}
