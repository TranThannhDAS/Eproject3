using backend.Dtos.TourDtos;
using backend.Entity;
using backend.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using webapi.Data;

namespace backend.Dao
{
    public class Top_10_Tour_Dao
    {
        private DataContext context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ImageService Image;

        public Top_10_Tour_Dao(DataContext dataContext, IHttpContextAccessor httpContextAccessor, ImageService imageService)
        {
            this.context = dataContext;
            this._httpContextAccessor = httpContextAccessor;
            this.Image = imageService;
        }
        public async Task<List<Top_10_Output>> Top_10_Tour()
        {
            var httpRequest = _httpContextAccessor.HttpContext.Request;
            var top10Tours = await (from order in context.Order
                              join tourDetail in context.TourDetail on order.Tour_Detail_ID equals tourDetail.Id
                              join tour in context.Tour on tourDetail.TourId equals tour.Id
                              group new { order, tour } by new { tourDetail.TourId } into grouped
                              orderby grouped.Sum(od => od.order.Number_people) descending
                              select new Top_10_Output
                              {
                                  Number_People = grouped.Sum(od => od.order.Number_people),
                                  Tour_ID = grouped.Key.TourId,
                                  Id = grouped.First().order.Id,                               
                                  Name = grouped.First().tour.Name,
                                  Price = grouped.First().tour.Price,
                                  category_id = grouped.First().tour.category_id,
                                  Description = grouped.First().tour.Description,
                                  image = grouped.First().tour.image,
                                  quantity_limit = grouped.First().tour.quantity_limit,
                                  Rating = grouped.First().tour.Rating,
                                  Type = grouped.First().tour.Type,
                                  Range_time = grouped.First().tour.Range_time,
                                  Discount = grouped.First().tour.Discount,
                                  Transportation_ID = grouped.First().tour.Transportation_ID,
                                  UrlImage = Image.GetUrlImage(grouped.First().tour.Name.Replace(" ", "-") + "-" + grouped.First().tour.Departure_location.Replace(" ", "-"), "tour", httpRequest)
                                 
                              })
                         .Take(10)
                         .ToListAsync();
            return top10Tours;


            /* 
         var httpRequest = _httpContextAccessor.HttpContext.Request;

         var result = await(from order in context.Order
                     join tour in context.Tour on order.Tour_ID equals tour.Id
                      where top10TourIds.Contains(tour.Id)
                            select new Top_10_Output
                             {       
                                 Id = order.Id,
                                 Number_People = order.Number_people,
                                 Tour_ID = tour.Id,
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
                             })
                             .ToListAsync();

         return result;
         */
        }
    }
    public class Top_10_Output : Search_Tour_Dto_Output
    {
        public int? Number_People { get; set; }
        public int? Tour_ID { get; set; }
    }
}
