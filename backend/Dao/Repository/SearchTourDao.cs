using backend.Dtos.TourDtos;
using backend.Entity;
using webapi.Data;

namespace backend.Dao.Repository
{
    public class SearchTourDao
    {
        private DataContext context;
        public SearchTourDao(DataContext context)
        {
            this.context = context;
        }
        //
        //public async Task getDepartureTime(TourDto tourDto)
        //{
        //    var tourDetail = context.TourDetail.Where(a => a.CreateDate == tourDto.Departure_Time);

        //}

    }
}
