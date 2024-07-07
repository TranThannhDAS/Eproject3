using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.CategorySpec
{
    public class SearchCategorySpec : BaseSpecification<Category>
    {
        public SearchCategorySpec(SpecParams param)
            : base(l =>
            string.IsNullOrEmpty(param.Search) ||
            l.Name.ToLower().Contains(param.Search.ToLower()) && (param.IsActive == null || l.IsActive == param.IsActive)
        )
        {
            
        }
    }
}
