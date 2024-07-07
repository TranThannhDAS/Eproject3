using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.LocationSpec
{
    public class GetRestaurantHasLocationId : BaseSpecification<Restaurant>
    {
        public GetRestaurantHasLocationId(int locationId)
        : base(restaurant => restaurant.LocationId == locationId)
        {
        }
    }
}
