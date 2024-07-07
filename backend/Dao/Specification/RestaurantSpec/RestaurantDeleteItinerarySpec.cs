using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.RestaurantSpec
{
    public class RestaurantDeleteItinerarySpec : BaseSpecification<Itinerary>
    {
        public RestaurantDeleteItinerarySpec(int restaurantId)
        : base(query => query.RestaurantID == restaurantId && query.Type.Contains("Restaurant")
              )
        {
        }
    }
}
