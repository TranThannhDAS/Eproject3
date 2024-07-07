using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.ResortSpec
{
    public class ResortDeleteItinerarySpec : BaseSpecification<Itinerary>
    {
        public ResortDeleteItinerarySpec(int resortId)
        : base(query => query.ResortID == resortId && query.Type.Contains("Resort"))
        {
        }
    }
}
