using AutoMapper;
using backend.Dao.Specification;
using backend.Dao.Specification.HotelSpec;
using backend.Dao.Specification.ResortSpec;
using backend.Dao.Specification.RestaurantSpec;
using backend.Dtos.HotelDtos;
using backend.Dtos.ResortDtos;
using backend.Dtos.RestaurantDtos;
using backend.Entity;
using backend.Exceptions;
using backend.Helper;
using Microsoft.EntityFrameworkCore;
using webapi.Dao.UnitofWork;
using webapi.Data;

namespace backend.BussinessLogic
{
    public class RestaurantBusinessLogic
    {
        public IUnitofWork unitofWork;
        public IMapper mapper;
        private ImageService Image;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private DataContext context;

        public RestaurantBusinessLogic(IUnitofWork _unitofWork, IMapper mapper, ImageService Image, IHttpContextAccessor _httpContextAccessor, DataContext dataContext)
        {
            unitofWork = _unitofWork;
            this.mapper = mapper;
            this.Image = Image;
            this._httpContextAccessor = _httpContextAccessor;
            context = dataContext;
        }

        //list restaurant
        public async Task<IEnumerable<object>> SelectAllRestaurant()
        {
            var data = await unitofWork.Repository<Restaurant>().GetAllAsync();
            var httpRequest = _httpContextAccessor.HttpContext.Request;
            var result = new List<object>();

            foreach (var restaurant in data)
            {
                var restaurantInfo = new
                {
                    restaurant.Id,
                    restaurant.Name,
                    restaurant.Price_range,
                    restaurant.Rating,
                    restaurant.LocationId,
                    restaurant.Description,
                    restaurant.Image,
                    restaurant.Address,
                    restaurant.PhoneNumbber,
                    restaurant.Links,
                    UrlImage = Image.GetUrlImage(restaurant.Name, "restaurant", httpRequest) // Gọi phương thức GetUrlImage11 cho từng bản ghi
                };

                result.Add(restaurantInfo);
            }
            return result;
        }

        //create category
        public async Task Create(RestaurantImageDto restaurantDto)
        {
            var restaurant = mapper.Map<RestaurantImageDto, Restaurant>(restaurantDto);
            if (restaurant is null)
            {
                throw new NotFoundExceptions("Restaurant not found");
            }

            if (await IsRestaurantNameDuplicate(restaurant.Address,restaurant.Id))
            {
                throw new BadRequestExceptions("Restaurant Address is exist.");
            }
            var Name_replace = restaurantDto.Name.Replace(" ", "-");
            var image_folder = Name_replace + "-" + restaurant.CreateDate.ToString("yyyy-MM-dd-HH-mm-ss");
            var images = await Image.Upload_Image(image_folder, "restaurant", restaurantDto.fileCollection);
            foreach (var image in images)
            {
                restaurant.AddImage(image);
            }
            await unitofWork.Repository<Restaurant>().AddAsync(restaurant);
            var check = await unitofWork.Complete();
            if (check < 1)
            {
                throw new BadRequestExceptions("chua dc thuc thi");
            }
        }

        //update restaurant
        public async Task Update(Restaurant_Update_Dto restaurantDto)
        {
            var restaurant = mapper.Map<Restaurant_Update_Dto, Restaurant>(restaurantDto);
            if (restaurant is null)
            {
                throw new NotFoundExceptions("not found");
            }

            var existingRestaurant = await unitofWork
                .Repository<Restaurant>()
                .GetByIdAsync(restaurant.Id);

            if (existingRestaurant is null)
            {
                throw new NotFoundExceptions("not found");
            }
            var Name_replace = restaurantDto.Name.Replace(" ", "-");
            var image_folder = Name_replace + "-" + existingRestaurant.CreateDate.ToString("yyyy-MM-dd-HH-mm-ss");
            var images = await Image.Update_Image(image_folder, existingRestaurant.Name.Replace(" ", "-") +"-" + existingRestaurant.CreateDate.ToString("yyyy-MM-dd-HH-mm-ss"), "restaurant", restaurantDto.path, restaurantDto.fileCollection);
            images.Add("JPG.JPG");
            foreach (var image in images)
            {
                restaurant.AddImage(image);
            }
            existingRestaurant.UpdateDate = restaurant.UpdateDate;
            existingRestaurant.UpdateBy = restaurant.UpdateBy;
            existingRestaurant.CreateBy = restaurant.CreateBy;
            existingRestaurant.Name = restaurant.Name;
            existingRestaurant.IsActive = restaurant.IsActive;
            existingRestaurant.Address = restaurant.Address;
            existingRestaurant.PhoneNumbber = restaurant.PhoneNumbber;
            existingRestaurant.Image = restaurant.Image;
            existingRestaurant.Price_range = restaurant.Price_range;
            existingRestaurant.Rating = restaurant.Rating;
            existingRestaurant.Description = restaurant.Description;
            existingRestaurant.Links = restaurant.Links;
            existingRestaurant.LocationId = restaurant.LocationId;
            if (await IsRestaurantNameDuplicate(restaurant.Address,restaurant.Id))
            {
                throw new BadRequestExceptions("Restaurant Address is exist.");
            }

            await unitofWork.Repository<Restaurant>().Update(existingRestaurant);
            var check = await unitofWork.Complete();
            if (check < 1)
            {
                throw new BadRequestExceptions("chua dc thuc thi");
            }
        }

        //delete restaurant
        public async Task Delete(int id)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var existingRestaurant = await unitofWork.Repository<Restaurant>().GetByIdAsync(id);
                    if (existingRestaurant == null)
                    {
                        throw new NotFoundExceptions("not found");
                    }

                    var itineraryHaveRestaurantId = await unitofWork.Repository<Itinerary>().GetAllWithAsync(new RestaurantDeleteItinerarySpec(id));
                    if (itineraryHaveRestaurantId.Any())
                    {
                        await unitofWork.Repository<Itinerary>().DeleteRange(itineraryHaveRestaurantId);
                    }

                    await unitofWork.Repository<Restaurant>().Delete(existingRestaurant);

                    var check = await unitofWork.Complete();
                    if (check < 1)
                    {
                        throw new BadRequestExceptions("chua dc thuc thi");
                    }
                    var Name_replace = existingRestaurant.Name.Replace(" ", "-");
                    var image_folder = Name_replace + "-" + existingRestaurant.CreateDate.ToString("yyyy-MM-dd-HH-mm-ss");
                    var delete_image = Image.DeleteImage(image_folder, "restaurant");
                    transaction.Commit(); // Commit giao dịch nếu mọi thứ thành công
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // Rollback giao dịch nếu có ngoại lệ
                    throw ex;
                }
            }
         
        }

        //get restaurant by id
        public async Task<object> GetByRestaurantId(int id)
        {
            var restaurant = await unitofWork.Repository<Restaurant>().GetByIdAsync(id);
            if (restaurant == null)
            {
                throw new NotFoundExceptions("not found");
            }
            var httpRequest = _httpContextAccessor.HttpContext.Request;
            var Name_replace = restaurant.Name.Replace(" ", "-");
            var image_folder = Name_replace + "-" + restaurant.CreateDate.ToString("yyyy-MM-dd-HH-mm-ss");
            var result = new
            {
                restaurant.Id,
                restaurant.Name,
                restaurant.Price_range,
                restaurant.Rating,
                restaurant.LocationId,
                restaurant.Description,
                restaurant.Image,
                restaurant.Address,
                restaurant.PhoneNumbber,
                restaurant.Links,
                urlImage = Image.GetUrlImage(image_folder, "restaurant", httpRequest)
            };
            return result;
        }

        //duplicate name
        private async Task<bool> IsRestaurantNameDuplicate(string restaurantName,int id)
        {
            // Sử dụng GetEntityWithSpecAsync để kiểm tra trùng lặp
            var duplicateRestaurant = await unitofWork
                .Repository<Restaurant>()
                .GetEntityWithSpecAsync(new RestaurantByAddressSpecification(restaurantName, id));

            return duplicateRestaurant != null;
        }
        public async Task<Pagination<RestaurantDto>> SelectAllRestaurantPagination(SpecParams specParams)
        {

            var httpRequest = _httpContextAccessor.HttpContext.Request;
            var spec = new SearchRestaurantSpec(specParams);
            var result = new List<RestaurantDto>();
            int count = await unitofWork.Repository<Restaurant>().GetCountWithSpecAsync(spec);
            var restaurantPage = new List<Restaurant>();
            if (string.IsNullOrEmpty(specParams.Search) && string.IsNullOrEmpty(specParams.Location) && specParams.Rating == null)
            {
                restaurantPage = await context.Restaurant.Skip((specParams.PageIndex - 1) * specParams.PageSize).Take(specParams.PageSize).ToListAsync();
            }
            else
            {
                var restaurants = await unitofWork.Repository<Restaurant>().GetAllWithAsync(spec);

                restaurantPage = restaurants.Skip((specParams.PageIndex - 1) * specParams.PageSize).Take(specParams.PageSize).ToList();

            }
            foreach (var restaurant in restaurantPage)
            {
                var Name_replace = restaurant.Name.Replace(" ", "-");
                var image_folder = Name_replace + "-" + restaurant.CreateDate.ToString("yyyy-MM-dd-HH-mm-ss");
                var location = context.Locations.FirstOrDefault(l => l.ID == restaurant.LocationId);
                var restaurantInfo = new RestaurantDto
                {
                    Id = restaurant.Id,
                    Name = restaurant.Name,
                    Price_range = restaurant.Price_range,
                    Rating = restaurant.Rating,
                    LocationId = restaurant.LocationId,
                    Location = location.State,
                    PhoneNumber = restaurant.PhoneNumbber,
                    UrlImage = Image.GetUrlImage(image_folder, "restaurant", httpRequest)
                };
                result.Add(restaurantInfo);
            }
            var totalPageIndex = count % specParams.PageSize == 0 ? count / specParams.PageSize : (count / specParams.PageSize) + 1;

            var pagination = new Pagination<RestaurantDto>(specParams.PageIndex, specParams.PageSize, result, count, totalPageIndex);

            return pagination;
        }
    }
}
