using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.TourDetailSpec
{
    public class TourDetailDeleteOrderSpec : BaseSpecification<Order>
    {
        public TourDetailDeleteOrderSpec(int TourDetailId)
        : base(query => query.Tour_Detail_ID == TourDetailId)
        {
        }
    }
}
