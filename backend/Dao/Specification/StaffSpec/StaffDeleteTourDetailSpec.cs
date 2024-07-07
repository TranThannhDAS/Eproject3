using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.StaffSpec
{
    public class StaffDeleteTourDetailSpec : BaseSpecification<TourDetail>
    {
        public StaffDeleteTourDetailSpec(int StaffId)
        : base(query => query.Staff_Id == StaffId)
        {
        }
    }
}
