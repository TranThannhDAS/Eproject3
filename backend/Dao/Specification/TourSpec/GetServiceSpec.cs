using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.TourSpec
{
    public class GetServiceSpec : BaseSpecification<Service>
    {
        public GetServiceSpec(int tourId)
        : base(order => order.Tour_ID == tourId)
        {
        }
    }
}
