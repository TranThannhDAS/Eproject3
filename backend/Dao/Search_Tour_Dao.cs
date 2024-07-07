using backend.Dtos.TourDtos;
using backend.Entity;
using backend.Helper;
using backend.Model.Paypal.Output;
using Microsoft.EntityFrameworkCore;
using webapi.Data;

namespace backend.Dao
{
    public class Search_Tour_Dao
    {
        private DataContext context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ImageService Image;


        public Search_Tour_Dao(DataContext _context, IHttpContextAccessor httpContextAccessor, ImageService imageService)
        {
            context = _context;
            _httpContextAccessor = httpContextAccessor;
            Image = imageService;
        }

        public async Task<List<Search_Tour_Dto_Output>> Search_Tour(Search_Tour_Dto_Input search_Tour_Dto)
        {
            double minprice = 0;
            double maxprice = 0;
            if (search_Tour_Dto.Price != null)
            {
                string[] words = search_Tour_Dto.Price.Split('-');
                int i = 0;
                if (words.Length >= 2)
                {
                    for (i = 0; i < 2; i++)
                    {
                        if (i == 0)
                        {
                            minprice = double.Parse(words[i]);
                        }
                        if (i == 1)
                        {
                            maxprice = double.Parse(words[i]);
                        }

                    }
                }
                else
                {
                    minprice = double.Parse(search_Tour_Dto.Price);
                    maxprice = double.MaxValue;
                }
            }      
            var httpRequest = _httpContextAccessor.HttpContext.Request;
            var query = from tour in context.Tour
                        join category in context.Category on tour.category_id equals category.Id
                        join tour_detail in context.TourDetail on tour.Id equals tour_detail.TourId
                        where (string.IsNullOrEmpty(search_Tour_Dto.Name) || tour.Name.Equals(search_Tour_Dto.Name))
                            && (!search_Tour_Dto.category_Id.HasValue || tour.category_id == search_Tour_Dto.category_Id.Value)
                            && (!search_Tour_Dto.Rating.HasValue || tour.Rating == search_Tour_Dto.Rating.Value)
                            && (!search_Tour_Dto.Departure_Time.HasValue || tour_detail.Start_Date == search_Tour_Dto.Departure_Time.Value)
                            &&(string.IsNullOrEmpty(search_Tour_Dto.Price) || tour.Price >= minprice && tour.Price <= maxprice)
                        select new Search_Tour_Dto_Output
                        {
                            Id = tour.Id,
                            Name = tour.Name,
                            Price = tour.Price,
                            category_id = tour.category_id,
                            Description = tour.Description,
                            image = tour.image,
                            quantity_limit = tour.quantity_limit,
                            Rating = tour.Rating,
                            Type = tour.Type,
                            Range_time = tour.Range_time,
                            Discount = tour.Discount,
                            Transportation_ID = tour.Transportation_ID,
                            UrlImage = Image.GetUrlImage(tour.Name, "tour", httpRequest)
                        };
            var result = await query.ToListAsync();
            return result;
        }

        //}
    }
}
