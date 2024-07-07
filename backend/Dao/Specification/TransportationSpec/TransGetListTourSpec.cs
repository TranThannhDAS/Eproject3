using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.TransportationSpec
{
    public class TransGetListTourSpec : BaseSpecification<Tour>
    {
        public TransGetListTourSpec(int CateId)
        : base(query => query.Transportation_ID == CateId)
        {
        }
    }
}
