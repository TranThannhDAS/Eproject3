using AutoMapper;
using backend.Dao.Specification.FeedBackSpec;
using backend.Dao.Specification;
using backend.Dtos.FeedBackDtos;
using backend.Entity;
using backend.Exceptions;
using backend.Helper;
using webapi.Dao.UnitofWork;
using webapi.Data;

namespace backend.BussinessLogic
{
    public class FeedBackBusinessLogic
    {
        public IUnitofWork unitofWork;
        public IMapper mapper;
        public DataContext context;
        public FeedBackBusinessLogic(IUnitofWork _unitofWork, IMapper mapper, DataContext context)
        {
            unitofWork = _unitofWork;
            this.mapper = mapper;
            this.context = context;
        }

        //list feedback
        public async Task<IReadOnlyList<FeedBack>> SelectAllFeedBack()
        {
            var data = await unitofWork.Repository<FeedBack>().GetAllAsync();
            return data;
        }

        //create feedback
        public async Task Create(FeedBack feedback)
        {
            if (feedback is null)
            {
                throw new NotFoundExceptions("Cattegory not found");
            }
            await unitofWork.Repository<FeedBack>().AddAsync(feedback);
            var check = await unitofWork.Complete();
            if (check < 1)
            {
                throw new BadRequestExceptions("chua dc thuc thi");
            }
        }

        //update feedback
        public async Task Update(FeedBack feedback,int id)
        {
            if (feedback is null)
            {
                throw new NotFoundExceptions("not found");
            }

            var existingFeedBack = await unitofWork.Repository<FeedBack>().GetByIdAsync(id);

            if (existingFeedBack is null)
            {
                throw new NotFoundExceptions("not found");
            }
            existingFeedBack.UpdateDate = feedback.UpdateDate;
            existingFeedBack.CreateDate = feedback.CreateDate;
            existingFeedBack.UpdateBy = feedback.UpdateBy;
            existingFeedBack.CreateBy = feedback.CreateBy;
            existingFeedBack.IsActive = feedback.IsActive;
            existingFeedBack.Title = feedback.Title;
            existingFeedBack.Name = feedback.Name;
            existingFeedBack.Email = feedback.Email;
            existingFeedBack.Phone = feedback.Phone;
            await unitofWork.Repository<FeedBack>().Update(existingFeedBack);
            var check = await unitofWork.Complete();
            if (check < 1)
            {
                throw new BadRequestExceptions("chua dc thuc thi");
            }
        }

        //delete feedback
        public async Task Delete(int id)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var existingFeedBack = await unitofWork.Repository<FeedBack>().GetByIdAsync(id);
                    if (existingFeedBack == null)
                    {
                        throw new NotFoundExceptions("not found");
                    }
                    await unitofWork.Repository<FeedBack>().Delete(existingFeedBack);
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
        public async Task<Pagination<FeedBackDto>> SelectAllFeedBackPagination(SpecParams specParams)
        {

            var spec = new SearchFeedBackSpec(specParams);
            var resorts = await unitofWork.Repository<FeedBack>().GetAllWithAsync(spec);

            var data = mapper.Map<IReadOnlyList<FeedBack>, IReadOnlyList<FeedBackDto>>(resorts);
            var feedbackPage = data.Skip((specParams.PageIndex - 1) * specParams.PageSize).Take(specParams.PageSize).ToList();

            var countSpec = new SearchFeedBackSpec(specParams);
            var count = await unitofWork.Repository<FeedBack>().GetCountWithSpecAsync(countSpec);

            var totalPageIndex = count % specParams.PageSize == 0 ? count / specParams.PageSize : (count / specParams.PageSize) + 1;

            var pagination = new Pagination<FeedBackDto>(specParams.PageIndex, specParams.PageSize, feedbackPage, count, totalPageIndex);

            return pagination;
        }
    }
}
