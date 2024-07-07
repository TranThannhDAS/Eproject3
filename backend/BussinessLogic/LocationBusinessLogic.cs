
using AutoMapper;
using backend.Dao.Specification;
using backend.Dao.Specification.LocationSpec;
using backend.Dtos.LocationDtos;
using backend.Entity;
using backend.Exceptions;
using backend.Helper;
using System.Drawing.Printing;
using webapi.Dao.UnitofWork;
using webapi.Data;

namespace backend.BussinessLogic
{
    public class LocationBusinessLogic
    {
        public IUnitofWork unitofWork;
        public IMapper mapper;
        public DataContext context;
        public LocationBusinessLogic(IUnitofWork _unitofWork, IMapper mapper ,DataContext context)
        {
            unitofWork = _unitofWork;
            this.mapper = mapper;
            this.context = context;
        }

        //list category
        public async Task<IReadOnlyList<Location1>> SelectAllLocation()
        {
            var data = await unitofWork.Repository<Location1>().GetAllAsync();
            return data;
        }
        
        //create category
        public async Task Create(Location1 location)
        {
            if (location is null)
            {
                throw new NotFoundExceptions("Cattegory not found");
            }
            await unitofWork.Repository<Location1>().AddAsync(location);
            var check = await unitofWork.Complete();
            if (check < 1)
            {
                throw new BadRequestExceptions("chua dc thuc thi");
            }
        }

        //update location
        public async Task Update(Location1 location, int id)
        {
            if (location is null)
            {
                throw new NotFoundExceptions("not found");
            }

            var existingLocation1 = await unitofWork.Repository<Location1>().GetByIdAsync(id);

            if (existingLocation1 is null)
            {
                throw new NotFoundExceptions("not found");
            }
            existingLocation1.UpdateDate = location.UpdateDate;
            existingLocation1.CreateDate = location.CreateDate;
            existingLocation1.UpdateBy = location.UpdateBy;
            existingLocation1.CreateBy = location.CreateBy;
            existingLocation1.IsActive = location.IsActive;
            existingLocation1.State = location.State;
            
            

            await unitofWork.Repository<Location1>().Update(existingLocation1);
            var check = await unitofWork.Complete();
            if (check < 1)
            {
                throw new BadRequestExceptions("chua dc thuc thi");
            }
        }

        //delete location
        public async Task Delete(int id)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var existingLocation1 = await unitofWork.Repository<Location1>().GetByIdAsync(id);
                    var HotelHaveLocationId = await unitofWork.Repository<Hotel>().GetAllWithAsync(new GetHotelHasLocationId(id));
                    var ResortHaveLocationId = await unitofWork.Repository<Resorts>().GetAllWithAsync(new GetResortHasLocationId(id));
                    var RestaurantHaveLocationId = await unitofWork.Repository<Restaurant>().GetAllWithAsync(new GetRestaurantHasLocationId(id));
                    if (existingLocation1 == null)
                    {
                        throw new NotFoundExceptions("not found");
                    }
                    // Xóa tất cả các khách sạn liên quan
                    foreach (var hotel in HotelHaveLocationId)
                    {
                        await unitofWork.Repository<Hotel>().Delete(hotel);
                    }
                    // Xóa tất cả các resort liên quan
                    foreach (var resort in ResortHaveLocationId)
                    {
                        await unitofWork.Repository<Resorts>().Delete(resort);
                    }
                    // Xóa tất cả các resort liên quan
                    foreach (var restaurant in RestaurantHaveLocationId)
                    {
                        await unitofWork.Repository<Restaurant>().Delete(restaurant);
                    }

                    await unitofWork.Repository<Location1>().Delete(existingLocation1);

                    var check = await unitofWork.Complete();
                    if (check < 1)
                    {
                        throw new BadRequestExceptions("chua dc thuc thi");
                    }
                    transaction.Commit(); // Commit giao dịch nếu mọi thứ thành công
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // Rollback giao dịch nếu có ngoại lệ
                    throw ex;
                }
            }
            
        }
        public async Task<Pagination<LocationDtos>> SelectAllLocation1Pagination(SpecParams specParams)
        {
           
            var spec = new SearchLocationSpec(specParams);
            var products = await unitofWork.Repository<Location1>().GetAllWithAsync(spec);

            var data = mapper.Map<IReadOnlyList<Location1>,IReadOnlyList<LocationDtos>>(products);

            var locationPage = data.Skip((specParams.PageIndex - 1) * specParams.PageSize).Take(specParams.PageSize).ToList();
            
            var countSpec = new SearchLocationSpec(specParams);
            var count = await unitofWork.Repository<Location1>().GetCountWithSpecAsync(countSpec);

            var totalPageIndex = count % specParams.PageSize == 0 ? count / specParams.PageSize : (count / specParams.PageSize) + 1;

            var pagination = new Pagination<LocationDtos>(specParams.PageIndex, specParams.PageSize, locationPage, count , totalPageIndex);


            return pagination;
        }

    }
}
