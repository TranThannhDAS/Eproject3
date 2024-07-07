using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.OrderDetailSpec
{
    public class SearchOrderDetailSpec : BaseSpecification<OrderDetail>
    {
        public SearchOrderDetailSpec(SpecParams param)
            : base(l =>
            string.IsNullOrEmpty(param.Search) ||
            l.Users.Name.ToLower().Contains(param.Search.ToLower()) && (param.IsActive == null || l.IsActive == param.IsActive)
        )
        {
            Includes.Add(s => s.Users);
        }
    }
}