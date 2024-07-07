using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.TourSpec
{
    public class GetTourDetailSpec : BaseSpecification<TourDetail>
    {
        public GetTourDetailSpec(int TourId)
        : base(query => query.TourId == TourId)
        {
        }
    }
}
