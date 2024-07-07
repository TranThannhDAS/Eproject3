
using AutoMapper;
using backend.Dao.Specification.TourSpec;
using backend.Dao.Specification;
using backend.Dtos.TourDtos;
using backend.Entity;
using backend.Exceptions;
using backend.Helper;
using Microsoft.AspNetCore.Http;
using webapi.Dao.UnitofWork;
using webapi.Data;
using Microsoft.EntityFrameworkCore;
using backend.Dtos.HotelDtos;
using backend.Dtos.RestaurantDtos;
using backend.Dtos.StaffDtos;
using backend.Dao.Specification.TransportationSpec;

namespace backend.BussinessLogic
{
    public class TourBusinessLogic
    {
        public IUnitofWork unitofWork;
        private ImageService Image;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IMapper mapper;
        private DataContext context;
        private TourDetailBusinessLogic TourDetailBusinessLogic;

        public TourBusinessLogic(IUnitofWork _unitofWork, ImageService imageService, IHttpContextAccessor httpContextAccessor, IMapper mapper, DataContext context, TourDetailBusinessLogic tourDetailBusinessLogic)
        {
            unitofWork = _unitofWork;
            Image = imageService;
            _httpContextAccessor = httpContextAccessor;
            this.mapper = mapper;
            this.context = context;
            TourDetailBusinessLogic = tourDetailBusinessLogic;
        }

        //list tour
        public async Task<IEnumerable<object>> SelectAllTour()
        {
            var data = await unitofWork.Repository<Tour>().GetAllAsync();
            var httpRequest = _httpContextAccessor.HttpContext.Request;
            var result = new List<object>();

            foreach (var tour in data)
            {
                var tourInfo = new
                {
                    tour.Id,
                    tour.Name,
                    tour.Price,
                    tour.category_id,
                    tour.Description,
                    tour.quantity_limit,
                    tour.Rating,
                    tour.Type,
                    tour.Range_time,
                    tour.Discount,
                    tour.Transportation_ID,
                    tour.Departure_location,
                    //Thêm các trường cần thêm
                    UrlImage = Image.GetUrlImage(tour.Name, "tour", httpRequest) // Gọi phương thức GetUrlImage11 cho từng bản ghi
                };

                result.Add(tourInfo);
            }
            return result;
        }

        //create tour
        public async Task Create(TourDto tourdto)
        {

            var tour = mapper.Map<TourDto, Tour>(tourdto);
            var Name_replace = tourdto.Name.Replace(" ", "-");
            var image_folder = Name_replace + "-" + tour.Departure_location.Replace(" ", "-");
            var images = await Image.Upload_Image(image_folder, "tour", tourdto.fileCollection);
            foreach (var image in images)
            {
                tour.AddImage(image);
            }
            await unitofWork.Repository<Tour>().AddAsync(tour);
            var check = await unitofWork.Complete();
            if (check < 1)
            {
                throw new BadRequestExceptions("chua dc thuc thi");
            }
        }
        public async Task<double> CreateWithRating(int id)
        {

            var tour = await unitofWork.Repository<Tour>().GetByIdAsync(id);
            if (tour is null)
            {
                throw new NotFoundExceptions("Tour not found");
            }
            var tourDetail = await unitofWork.Repository<TourDetail>().GetAllWithAsync(new GetOrderSpec(id));
            var orderDetailList = new List<OrderDetail>();
            foreach (var iteam in tourDetail)
            {
                var orderDetail = await unitofWork.Repository<OrderDetail>().GetAllWithAsync(new SelectAllRatingOrderDetailSpec(iteam.Id));
                orderDetailList.AddRange(orderDetail);
            }
            double averageRating = 0;
            if (orderDetailList != null && orderDetailList.Any())
            {
                var ratings = orderDetailList.Select(od => od.Rating).ToList();

                if (ratings.Any())
                {
                    averageRating = (double)ratings.Average();
                    tour.Rating = (int)averageRating;
                }
                else
                {
                    throw new BadRequestExceptions("Rating is null");
                }

            }
            return averageRating;
        }




        //update tour
        public async Task Update(Tour_Update_Dto tourdto)
        {

            var tour = mapper.Map<Tour_Update_Dto, Tour>(tourdto);

            var existingTour = await unitofWork.Repository<Tour>().GetByIdAsync(tour.Id);

            if (existingTour is null)
            {
                throw new NotFoundExceptions("not found");
            }
            var Name_replace = tourdto.Name.Replace(" ", "-");
            var image_folder = Name_replace + "-" + existingTour.Departure_location.Replace(" ", "-");
            var images = await Image.Update_Image(image_folder, existingTour.Name.Replace(" ", "-") + "-" + existingTour.Departure_location.Replace(" ", "-"), "tour", tourdto.path, tourdto.fileCollection);
            images.Add("JPG.JPG");
            foreach (var image in images)
            {
                tour.AddImage(image);
            }
            existingTour.UpdateDate = tour.UpdateDate;
            existingTour.UpdateBy = tour.UpdateBy;
            existingTour.CreateBy = tour.CreateBy;
            existingTour.IsActive = tour.IsActive;
            existingTour.Name = tour.Name;
            existingTour.Price = tour.Price;
            existingTour.category_id = tour.category_id;
            existingTour.Description = tour.Description;
            existingTour.image = tour.image;
            existingTour.quantity_limit = tour.quantity_limit;
            existingTour.Rating = tour.Rating;
            existingTour.Type = tour.Type;

            existingTour.Transportation_ID = tour.Transportation_ID;
            existingTour.Departure_location = tour.Departure_location;
            existingTour.Range_time = tour.Range_time;
            await unitofWork.Repository<Tour>().Update(existingTour);
            var check = await unitofWork.Complete();
            if (check < 1)
            {
                throw new BadRequestExceptions("chua dc thuc thi");
            }
        }

        //delete tour
        public async Task Delete(int id)
        {
            
                try
                {
                    var existingTour = await unitofWork.Repository<Tour>().GetByIdAsync(id);
                    if (existingTour == null)
                    {
                        throw new NotFoundExceptions("not found");
                    }
                    var Tour = await unitofWork.Repository<TourDetail>().GetAllWithAsync(new GetTourDetailSpec(id));
                    var iti = await unitofWork.Repository<Itinerary>().GetAllWithAsync(new GetItinerarySpec(id));
                    var service = await unitofWork.Repository<Service>().GetAllWithAsync(new GetServiceSpec(id));
                    if (Tour.Any())
                    {
                        foreach (var item in Tour)

                        {
                            await TourDetailBusinessLogic.Delete(item.Id);
                        }
                    }
                        

                    if (iti.Any())
                    {
                        await unitofWork.Repository<Itinerary>().DeleteRange(iti);
                    }
                    if (service.Any())
                    {
                        await unitofWork.Repository<Service>().DeleteRange(service);
                    }
                    await unitofWork.Repository<Tour>().Delete(existingTour);
                    var check = await unitofWork.Complete();
                    if (check < 1)
                    {
                        throw new BadRequestExceptions("chua dc thuc thi");
                    }
                    //var Name_replace = existingTour.Name.Replace(" ", "-");
                    //var image_folder = Name_replace + "-" + existingTour.Departure_location.Replace(" ", "-");
                    //var delete_image = Image.DeleteImage(image_folder, "tour");


                }
                catch (Exception ex)
                {

                    throw ex;
                }
            
        }

        //get tour by id
        public async Task<object> GetByTourId(int id)
        {
            var existingTour = await unitofWork.Repository<Tour>().GetByIdAsync(id);
            if (existingTour == null)
            {
                throw new NotFoundExceptions("not found");
            }
            var httpRequest = _httpContextAccessor.HttpContext.Request;
            var Name_replace = existingTour.Name.Replace(" ", "-");
            var image_folder = Name_replace + "-" + existingTour.Departure_location.Replace(" ", "-");
            var result = new
            {
                existingTour.Id,
                existingTour.Name,
                existingTour.Price,
                existingTour.category_id,
                existingTour.Description,
                existingTour.quantity_limit,
                existingTour.Rating,
                existingTour.Type,
                existingTour.Range_time,
                existingTour.Discount,
                existingTour.Transportation_ID,
                existingTour.Departure_location,
                //thêm cách cột còn lại
                urlImage = Image.GetUrlImage(image_folder, "tour", httpRequest)
            };


            return result;
        }
        public async Task<Pagination<TourPageDto>> SelectAllTourPagination(SpecParams specParams)
        {

            var httpRequest = _httpContextAccessor.HttpContext.Request;
            var spec = new SearchTourSpec(specParams);
            var result = new List<TourPageDto>();
            int count = await unitofWork.Repository<Tour>().GetCountWithSpecAsync(spec);
            var tourPage = new List<Tour>();
            if (string.IsNullOrEmpty(specParams.Search) && specParams.Rating == null && specParams.Location == null)
            {
                tourPage = await context.Tour.Skip((specParams.PageIndex - 1) * specParams.PageSize).Take(specParams.PageSize).ToListAsync();
            }
            else
            {
                var tours = await unitofWork.Repository<Tour>().GetAllWithAsync(spec);

                tourPage = tours.Skip((specParams.PageIndex - 1) * specParams.PageSize).Take(specParams.PageSize).ToList();

            }
            foreach (var tour in tourPage)
            {
                var Name_replace = tour.Name.Replace(" ", "-");
                var image_folder = Name_replace + "-" + tour.Departure_location.Replace(" ", "-");
                var cat = context.Category.FirstOrDefault(l => l.Id == tour.category_id);
                var trans = context.Transportation.FirstOrDefault(l => l.Id == tour.Transportation_ID);
                var tourInfo = new TourPageDto
                {
                    Id = tour.Id,
                    Name = tour.Name,
                    Price = tour.Price,
                    category_Name = cat.Name,
                    Description = tour.Description,
                    quantity_limit = tour.quantity_limit,
                    Rating = tour.Rating, // Sử dụng giá trị mặc định nếu tour.Rating là null
                    Type = tour.Type, // Sử dụng giá trị mặc định nếu tour.Type là null
                    Range_time = tour.Range_time ?? 4, // Sử dụng giá trị mặc định nếu tour.Range_time là null
                    Discount = tour.Discount,
                    Transportation_Name = trans.Name,
                    Departure_location = tour.Departure_location,
                    UrlImage = Image.GetUrlImage(image_folder, "tour", httpRequest)
                };
                result.Add(tourInfo);
            }
            var totalPageIndex = count % specParams.PageSize == 0 ? count / specParams.PageSize : (count / specParams.PageSize) + 1;

            var pagination = new Pagination<TourPageDto>(specParams.PageIndex, specParams.PageSize, result, count, totalPageIndex);

            return pagination;

        }



    }
}
