using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.TourDetailSpec
{
    public class TourDetailDeleteOrderDetailSpec : BaseSpecification<OrderDetail>
    {
        public TourDetailDeleteOrderDetailSpec(int TourDetailId)
        : base(query => query.Tour_Detail_ID == TourDetailId)
        {
        }
    }
}
