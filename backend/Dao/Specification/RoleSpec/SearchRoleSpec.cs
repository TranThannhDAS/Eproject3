using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.RoleSpec
{
    public class SearchRoleSpec : BaseSpecification<Role>
    {
        public SearchRoleSpec(SpecParams param)
            : base(l =>
            string.IsNullOrEmpty(param.Search) ||
            l.Name.ToLower().Contains(param.Search.ToLower()) && (param.IsActive == null || l.IsActive == param.IsActive)
        )
        {

        }
    }
}
