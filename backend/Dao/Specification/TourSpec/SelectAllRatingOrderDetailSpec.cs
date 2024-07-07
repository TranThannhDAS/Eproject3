using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.TourSpec
{
    public class SelectAllRatingOrderDetailSpec : BaseSpecification<OrderDetail>
    {
        public SelectAllRatingOrderDetailSpec(int tourDetail)
        : base(orderDetail => orderDetail.Tour_Detail_ID == tourDetail)
        {
        }
    }
}
