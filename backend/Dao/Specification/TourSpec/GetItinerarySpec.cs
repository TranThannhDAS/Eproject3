using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.TourSpec
{
    public class GetItinerarySpec : BaseSpecification<Itinerary>
    {
        public GetItinerarySpec(int tourId)
        : base(order => order.TourID == tourId)
        {
        }
    }
}
