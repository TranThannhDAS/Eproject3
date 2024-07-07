using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.OrderDetail1
{
    public class OrderDetailSpec : BaseSpecification<OrderDetail>
    {
        public OrderDetailSpec(int id): base(p=> p.Tour_Detail_ID == id) { }
    }
}
