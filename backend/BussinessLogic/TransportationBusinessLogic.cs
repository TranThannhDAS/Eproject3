using AutoMapper;
using backend.Dao.Specification.TransportationSpec;
using backend.Dao.Specification;
using backend.Dtos.TransportationDtos;
using backend.Entity;
using backend.Exceptions;
using backend.Helper;
using webapi.Dao.UnitofWork;
using backend.Dao.Specification.CategorySpec;
using webapi.Data;

namespace backend.BussinessLogic
{
    public class TransportationBusinessLogic
    {
        public IUnitofWork unitofWork;
        public IMapper mapper;
        public DataContext context;
        private TourBusinessLogic tourBusiness;
        public TransportationBusinessLogic(IUnitofWork _unitofWork, IMapper mapper , DataContext context, TourBusinessLogic tourBusiness)
        {
            unitofWork = _unitofWork;
            this.mapper = mapper;
            this.context = context;
            this.tourBusiness = tourBusiness;
        }

        //list category
        public async Task<IReadOnlyList<Transportation>> SelectAllTransportation()
        {
            var data = await unitofWork.Repository<Transportation>().GetAllAsync();
            return data;
        }

        //create category
        public async Task Create(Transportation transportation)
        {
            if (transportation is null)
            {
                throw new NotFoundExceptions("Cattegory not found");
            }




            await unitofWork.Repository<Transportation>().AddAsync(transportation);
            var check = await unitofWork.Complete();
            if (check < 1)
            {
                throw new BadRequestExceptions("chua dc thuc thi");
            }
        }

        //update transportation
        public async Task Update(Transportation transportation,int id)
        {
            if (transportation is null)
            {
                throw new NotFoundExceptions("not found");
            }

            var existingTransportation = await unitofWork.Repository<Transportation>().GetByIdAsync(id);

            if (existingTransportation is null)
            {
                throw new NotFoundExceptions("not found");
            }
            existingTransportation.UpdateDate = transportation.UpdateDate;
            existingTransportation.CreateDate = transportation.CreateDate;
            existingTransportation.UpdateBy = transportation.UpdateBy;
            existingTransportation.CreateBy = transportation.CreateBy;
            existingTransportation.IsActive = transportation.IsActive;
            existingTransportation.Name = transportation.Name;
            existingTransportation.Price = transportation.Price;
            existingTransportation.Description = transportation.Description;


            await unitofWork.Repository<Transportation>().Update(existingTransportation);
            var check = await unitofWork.Complete();
            if (check < 1)
            {
                throw new BadRequestExceptions("chua dc thuc thi");
            }
        }

        //delete transportation
        public async Task Delete(int id)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var existingTransportation = await unitofWork.Repository<Transportation>().GetByIdAsync(id);
                    if (existingTransportation == null)
                    {
                        throw new NotFoundExceptions("not found");
                    }
                    var Tour = await unitofWork.Repository<Tour>().GetAllWithAsync(new TransGetListTourSpec(id));
                    if (Tour.Any())
                    {
                        foreach (var item in Tour)
                        {
                            await tourBusiness.Delete(item.Id);
                        }
                        //await unitofWork.Repository<Tour>().DeleteRange(Tour);
                    }
                    await unitofWork.Repository<Transportation>().Delete(existingTransportation);
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
        public async Task<Pagination<TransportationDto>> SelectAllTransportationPagination(SpecParams specParams)
        {

            var spec = new SearchTransportationSpec(specParams);
            var resorts = await unitofWork.Repository<Transportation>().GetAllWithAsync(spec);

            var data = mapper.Map<IReadOnlyList<Transportation>, IReadOnlyList<TransportationDto>>(resorts);
            var transportationPage = data.Skip((specParams.PageIndex - 1) * specParams.PageSize).Take(specParams.PageSize).ToList();

            var countSpec = new SearchTransportationSpec(specParams);
            var count = await unitofWork.Repository<Transportation>().GetCountWithSpecAsync(countSpec);

            var totalPageIndex = count % specParams.PageSize == 0 ? count / specParams.PageSize : (count / specParams.PageSize) + 1;

            var pagination = new Pagination<TransportationDto>(specParams.PageIndex, specParams.PageSize, transportationPage, count, totalPageIndex);

            return pagination;
        }
    }
}
