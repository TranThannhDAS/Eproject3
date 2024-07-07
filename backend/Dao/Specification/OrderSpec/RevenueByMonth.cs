using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.OrderSpec
{
    public class RevenueByMonth : BaseSpecification<Order>
    {
        public RevenueByMonth(int year , int month)
        : base(query => query.CreateDate.Year == year && query.CreateDate.Month == month)
        {
        }
    }
}
