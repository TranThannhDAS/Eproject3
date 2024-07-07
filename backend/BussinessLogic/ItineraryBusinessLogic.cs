using AutoMapper;
using backend.Dao.Specification.ItinerarySpec;
using backend.Dao.Specification;
using backend.Dtos.ItineraryDtos;
using backend.Entity;
using backend.Exceptions;
using backend.Helper;
using webapi.Dao.UnitofWork;
using webapi.Data;

namespace backend.BussinessLogic
{
    public class ItineraryBusinessLogic
    {
        public IUnitofWork unitofWork;
        public IMapper mapper;
        public DataContext context;
        public ItineraryBusinessLogic(IUnitofWork _unitofWork, IMapper mapper,DataContext context)
        {
            unitofWork = _unitofWork;
            this.mapper = mapper;
            this.context = context;
        }

        //list itinerary
        public async Task<IReadOnlyList<Itinerary>> SelectAllItinerary()
        {
            var data = await unitofWork.Repository<Itinerary>().GetAllAsync();
            return data;
        }
        //
        //public async Task<Pagination<Itinerary>> SelectAllItineraryPagination(int pageIndex, int pageSize)
        //{
        //    var data = await unitofWork.Repository<Itinerary>().GetAllAsync();

        //    // Tính toán chỉ mục bắt đầu dựa trên pageIndex và pageSize
        //    var skip = (pageIndex - 1) * pageSize;

        //    // Lấy một trang dữ liệu dựa trên chỉ mục bắt đầu và kích thước trang
        //    var itineraryPage = data.Skip(skip).Take(pageSize).ToList();

        //    // Tạo đối tượng Pagination và trả về
        //    var pagination = new Pagination<Itinerary>(pageIndex, pageSize, itineraryPage);
        //    return pagination;
        //}
        //create itinerary
        public async Task Create(Itinerary itinerary)
        {
            if (itinerary is null)
            {
                throw new NotFoundExceptions("Cattegory not found");
            }
            if (itinerary.Type.Contains("Hotel"))
            {
                itinerary.ResortID = 1;
                itinerary.RestaurantID = 1;
            }else if (itinerary.Type.Contains("Restaurant"))
            {
                itinerary.ResortID = 1;
                itinerary.HotelId = 1;
            }
            else if(itinerary.Type.Contains("Resort"))
            {
                itinerary.HotelId = 1;
                itinerary.RestaurantID= 1;
            }
            await unitofWork.Repository<Itinerary>().AddAsync(itinerary);
            var check = await unitofWork.Complete();
            if (check < 1)
            {
                throw new BadRequestExceptions("chua dc thuc thi");
            }
        }

        //update itinerary
        public async Task Update(Itinerary itinerary, int id)
        {
            if (itinerary is null)
            {
                throw new NotFoundExceptions("not found");
            }

            var existingItinerary = await unitofWork.Repository<Itinerary>().GetByIdAsync(id);

            if (existingItinerary is null)
            {
                throw new NotFoundExceptions("not found");
            }
            existingItinerary.UpdateDate = itinerary.UpdateDate;
            existingItinerary.CreateDate = itinerary.CreateDate;
            existingItinerary.UpdateBy = itinerary.UpdateBy;
            existingItinerary.CreateBy = itinerary.CreateBy;
            existingItinerary.IsActive = itinerary.IsActive;
            existingItinerary.TourID = itinerary.TourID;
            existingItinerary.Sequence = itinerary.Sequence;
            existingItinerary.Description = itinerary.Description;
            existingItinerary.StartTime = itinerary.StartTime;
            existingItinerary.EndTime = itinerary.EndTime;        
            if (itinerary.Type.Contains("Hotel"))
            {
                existingItinerary.ResortID = 1;
                existingItinerary.RestaurantID = 1;
                existingItinerary.HotelId = itinerary.HotelId;
            }
            else if (itinerary.Type.Contains("Restaurant"))
            {
                existingItinerary.ResortID = 1;
                existingItinerary.HotelId = 1;
                existingItinerary.RestaurantID = itinerary.RestaurantID;
            }
            else if (itinerary.Type.Contains("Resort"))
            {
                existingItinerary.HotelId = 1;
                existingItinerary.RestaurantID = 1;
                existingItinerary.ResortID = itinerary.ResortID;
            }
            await unitofWork.Repository<Itinerary>().Update(existingItinerary);
            var check = await unitofWork.Complete();
            if (check < 1)
            {
                throw new BadRequestExceptions("chua dc thuc thi");
            }
        }

        //delete itinerary
        public async Task Delete(int id)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var existingItinerary = await unitofWork.Repository<Itinerary>().GetByIdAsync(id);
                    if (existingItinerary == null)
                    {
                        throw new NotFoundExceptions("not found");
                    }
                    await unitofWork.Repository<Itinerary>().Delete(existingItinerary);
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
        public async Task<Pagination<ItineraryDto>> SelectAllItineraryPagination(SpecParams specParams)
        {

            var spec = new SearchItinerarySpec(specParams);
            var itinerary = await unitofWork.Repository<Itinerary>().GetAllWithAsync(spec);

            var data = mapper.Map<IReadOnlyList<Itinerary>, IReadOnlyList<ItineraryDto>>(itinerary);
            var itireraryPage = data.Skip((specParams.PageIndex - 1) * specParams.PageSize).Take(specParams.PageSize).ToList();

            var countSpec = new SearchItinerarySpec(specParams);
            var count = await unitofWork.Repository<Itinerary>().GetCountWithSpecAsync(countSpec);

            var totalPageIndex = count % specParams.PageSize == 0 ? count / specParams.PageSize : (count / specParams.PageSize) + 1;

            var pagination = new Pagination<ItineraryDto>(specParams.PageIndex, specParams.PageSize, itireraryPage, count, totalPageIndex);

            return pagination;
        }
    }
}
