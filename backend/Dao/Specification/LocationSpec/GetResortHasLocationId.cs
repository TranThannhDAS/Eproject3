using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.LocationSpec
{
    public class GetResortHasLocationId : BaseSpecification<Resorts>
    {
        public GetResortHasLocationId(int locationId)
        : base(resort => resort.LocationId == locationId)
        {
        }
    }
}
