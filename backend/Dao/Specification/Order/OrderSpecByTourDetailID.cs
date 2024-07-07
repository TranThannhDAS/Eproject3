using webapi.Dao.Specification;
using backend.Entity;

namespace backend.Dao.Specification.Order1
{
    public class OrderSpecByTourDetailID : BaseSpecification<Order>
    {
        public OrderSpecByTourDetailID(int Tour_detail_id):
            base(p => p.Tour_Detail_ID == Tour_detail_id)
        {
            
        }
    }
}
