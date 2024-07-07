using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.TourSpec
{
    public class GetOrderSpec : BaseSpecification<TourDetail>
    {
        public GetOrderSpec(int tourId)
        : base(order => order.TourId == tourId)
        {
        }
    }
}
