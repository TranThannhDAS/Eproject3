using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.RestaurantSpec
{
    public class RestaurantByAddressSpecification : BaseSpecification<Restaurant>
    {
        public RestaurantByAddressSpecification(string restaurantAddress, int id)
        : base(res => res.Address.ToLower() == restaurantAddress.ToLower() && res.Id != id)
        {
        }
    }
}
