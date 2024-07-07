using backend.Entity;
using Microsoft.EntityFrameworkCore;
using webapi.Data;

namespace backend.Dao
{
    public class Search_TourDetail_Dao
    {
        private readonly DataContext context;
        public Search_TourDetail_Dao(DataContext dataContext)
        {
            context = dataContext;
        }
        public async Task<TourDetail> QueryDao(DateTime start_date, int TourID)
        {
            var result = from td in context.TourDetail
                         join t in context.Tour on td.TourId equals t.Id
                         where td.Start_Date == start_date && t.Id == TourID
                         select new TourDetail
                         {
                             Id = td.Id,
                             TourId = t.Id,
                             Start_Date = td.Start_Date,
                             Quantity = td.Quantity,
                             Staff_Id = td.Staff_Id,
                             Description = td.Description,
                         };

            var check = await result.FirstOrDefaultAsync();
            return check;
        }
    }
}
