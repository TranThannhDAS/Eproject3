using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.TourSpec
{
    public class SearchTourSpec : BaseSpecification<Tour>
    {
        public SearchTourSpec(SpecParams param)
            : base(l =>
            (string.IsNullOrEmpty(param.Search) ||
            l.Name.ToLower().Contains(param.Search.ToLower())) &&

                (param.Rating == null || l.Rating == param.Rating) && (param.IsActive == null || l.IsActive == param.IsActive)

        //&&
        //    (param.Departure_Time == null || l.TourDetail == param.Rating)
        )
        {
            Includes.Add(s => s.category);
            Includes.Add(s => s.TourDetail);
        }
    }
}