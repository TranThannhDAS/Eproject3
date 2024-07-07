using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.TransportationSpec
{
    public class SearchTransportationSpec : BaseSpecification<Transportation>
    {
        public SearchTransportationSpec(SpecParams param)
            : base(l =>
            string.IsNullOrEmpty(param.Search) ||
            l.Name.ToLower().Contains(param.Search.ToLower()) && (param.IsActive == null || l.IsActive == param.IsActive)
        )
        {           
        }
    }
}
