using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.RoleSpec
{
    public class GetUserSpec : BaseSpecification<User>
    {
        public GetUserSpec(int RoleId)
        : base(query => query.RoleId == RoleId)
        {
        }
    }
}
