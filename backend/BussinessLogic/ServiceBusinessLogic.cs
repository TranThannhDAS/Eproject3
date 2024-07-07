using AutoMapper;
using backend.Dao.Specification.ServiceSpec;
using backend.Dao.Specification;
using backend.Dtos.ServiceDtos;
using backend.Entity;
using backend.Exceptions;
using backend.Helper;
using webapi.Dao.UnitofWork;

namespace backend.BussinessLogic
{
    public class ServiceBusinessLogic
    {
        public IUnitofWork unitofWork;
        public IMapper mapper;
        public ServiceBusinessLogic(IUnitofWork _unitofWork, IMapper mapper)
        {
            unitofWork = _unitofWork;
            this.mapper = mapper;
        }

        //list service
        public async Task<IReadOnlyList<Service>> SelectAllService()
        {
            var data = await unitofWork.Repository<Service>().GetAllAsync();
            return data;
        }

        //create service
        public async Task Create(Service service)
        {
            if (service is null)
            {
                throw new NotFoundExceptions("Cattegory not found");
            }
            await unitofWork.Repository<Service>().AddAsync(service);
            var check = await unitofWork.Complete();
            if (check < 1)
            {
                throw new BadRequestExceptions("chua dc thuc thi");
            }
        }

        //update service
        public async Task Update(Service service, int id)
        {
            if (service is null)
            {
                throw new NotFoundExceptions("not found");
            }

            var existingService = await unitofWork.Repository<Service>().GetByIdAsync(id);

            if (existingService is null)
            {
                throw new NotFoundExceptions("not found");
            }
            existingService.UpdateDate = service.UpdateDate;
            existingService.CreateDate = service.CreateDate;
            existingService.UpdateBy = service.UpdateBy;
            existingService.CreateBy = service.CreateBy;
            existingService.IsActive = service.IsActive;
            existingService.Tour = service.Tour;
            existingService.Name = service.Name;
            existingService.Description = service.Description;
            
            await unitofWork.Repository<Service>().Update(existingService);
            var check = await unitofWork.Complete();
            if (check < 1)
            {
                throw new BadRequestExceptions("chua dc thuc thi");
            }
        }

        //delete service
        public async Task Delete(int id)
        {

            var existingService = await unitofWork.Repository<Service>().GetByIdAsync(id);
            if (existingService == null)
            {
                throw new NotFoundExceptions("not found");
            }
            await unitofWork.Repository<Service>().Delete(existingService);
            var check = await unitofWork.Complete();
            if (check < 1)
            {
                throw new BadRequestExceptions("chua dc thuc thi");
            }
        }
        public async Task<Pagination<ServiceDto>> SelectAllServicePagination(SpecParams specParams)
        {

            var spec = new SearchServiceSpec(specParams);
            var service = await unitofWork.Repository<Service>().GetAllWithAsync(spec);

            var data = mapper.Map<IReadOnlyList<Service>, IReadOnlyList<ServiceDto>>(service);
            var servicePage = data.Skip((specParams.PageIndex - 1) * specParams.PageSize).Take(specParams.PageSize).ToList();

            var countSpec = new SearchServiceSpec(specParams);
            var count = await unitofWork.Repository<Service>().GetCountWithSpecAsync(countSpec);

            var totalPageIndex = count % specParams.PageSize == 0 ? count / specParams.PageSize : (count / specParams.PageSize) + 1;

            var pagination = new Pagination<ServiceDto>(specParams.PageIndex, specParams.PageSize, servicePage, count, totalPageIndex);

            return pagination;
        }
    }
}
