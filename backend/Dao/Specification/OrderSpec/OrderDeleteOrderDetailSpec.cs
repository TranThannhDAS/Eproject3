using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.OrderSpec
{
    public class OrderDeleteOrderDetailSpec : BaseSpecification<OrderDetail>
    {
        public OrderDeleteOrderDetailSpec(int OrderId)
        : base(query => query.OrderID == OrderId )
        {
        }
    }
}
