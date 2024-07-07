using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.CategorySpec
{
    public class GetListTourSpec : BaseSpecification<Tour>
    {
        public GetListTourSpec(int TourId)
        : base(query => query.category_id == TourId)
        {
        }
    }
}
