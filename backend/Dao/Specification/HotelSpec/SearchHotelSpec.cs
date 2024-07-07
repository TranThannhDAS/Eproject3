using backend.Entity;
using webapi.Dao.Specification;

namespace backend.Dao.Specification.HotelSpec
{
    public class SearchHotelSpec : BaseSpecification<Hotel>
    {
        public SearchHotelSpec(SpecParams param)
        : base(l =>
            (string.IsNullOrEmpty(param.Search) ||
            param.Search.ToLower().Contains(l.Name.ToLower())) &&
                (param.Location == null || l.location1.State.ToLower().Contains(param.Location)) &&
                (param.Rating == null || l.Rating == param.Rating) && (param.IsActive == null|| l.IsActive == param.IsActive)
        )
        {
            Includes.Add(s => s.location1);
        }
    }
}
