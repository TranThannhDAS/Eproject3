using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.InformationSpec
{
    public class SearchInformationSpec : BaseSpecification<Information>
    {
        public SearchInformationSpec(SpecParams param)
            : base(l =>
            string.IsNullOrEmpty(param.Search) ||
            l.Location.ToLower().Contains(param.Search.ToLower()) && (param.IsActive == null || l.IsActive == param.IsActive)
        )
        {

        }
    }
}
