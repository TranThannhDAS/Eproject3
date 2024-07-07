using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.TourDetailSpec
{
    public class SearchTourDetailSpec : BaseSpecification<TourDetail>
    {
        public SearchTourDetailSpec(SpecParams param)
            : base(l =>
            string.IsNullOrEmpty(param.Search) ||
            l.tour.Name.ToLower().Contains(param.Search.ToLower()) && (param.IsActive == null || l.IsActive == param.IsActive)
        )
        {
            Includes.Add(s => s.tour);
        }
    }
}
