using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.LocationSpec
{
    public class GetHotelHasLocationId : BaseSpecification<Hotel>
    {
        public GetHotelHasLocationId(int locationId)
        : base(hotel => hotel.LocationId == locationId)
        {
        }
    }
}
