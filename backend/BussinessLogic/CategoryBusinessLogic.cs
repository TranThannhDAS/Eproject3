using AutoMapper;
using backend.Dao.Specification;
using backend.Dao.Specification.CategorySpec;
using backend.Dtos.CategoryDtos;
using backend.Entity;
using backend.Exceptions;
using backend.Helper;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Web.Http.ModelBinding;
using webapi.Dao.Specification;
using webapi.Dao.UnitofWork;
using webapi.Data;

namespace backend.BussinessLogic
{
    public class CategoryBusinessLogic
    {
        public IUnitofWork unitofWork;
        public IMapper mapper;
        public DataContext context;
        private TourBusinessLogic tourBusiness;
        public CategoryBusinessLogic(IUnitofWork _unitofWork, IMapper mapper,DataContext context, TourBusinessLogic tourBusiness)
        {
            unitofWork = _unitofWork;
            this.mapper = mapper;
            this.context = context;
            this.tourBusiness = tourBusiness;
        }

        //list category
        public async Task<IReadOnlyList<Category>> SelectAllCategory()
        {
            var data = await unitofWork.Repository<Category>().GetAllAsync();
            return data;
        }

        //create category
        public async Task Create(Category category)
        {
            if (category is null)
            {
                throw new NotFoundExceptions("Cattegory not found");
            }

            if (await IsCategoryNameDuplicate(category.Name))
            {
                throw new BadRequestExceptions("Category Name is exist.");
            }


            await unitofWork.Repository<Category>().AddAsync(category);
            var check = await unitofWork.Complete();
            if (check < 1)
            {
                throw new BadRequestExceptions("chua dc thuc thi");
            }
        }

        //update category
        public async Task Update(Category category,int id)
        {
            if (category is null)
            {
                throw new NotFoundExceptions("not found");
            }

            var existingCategory = await unitofWork.Repository<Category>().GetByIdAsync(id);

            if (existingCategory is null)
            {
                throw new NotFoundExceptions("not found");
            }
            existingCategory.UpdateDate = category.UpdateDate;
            existingCategory.CreateDate = category.CreateDate;
            existingCategory.UpdateBy = category.UpdateBy;
            existingCategory.CreateBy = category.CreateBy;
            existingCategory.Name = category.Name;
            existingCategory.IsActive = category.IsActive;

            await unitofWork.Repository<Category>().Update(existingCategory);
            var check = await unitofWork.Complete();
            if (check < 1)
            {
                throw new BadRequestExceptions("chua dc thuc thi");
            }
        }

        //delete category
        public async Task Delete(int id)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var existingCategory = await unitofWork.Repository<Category>().GetByIdAsync(id);
                    if (existingCategory == null)
                    {
                        throw new NotFoundExceptions("not found");
                    }
                    var Tour = await unitofWork.Repository<Tour>().GetAllWithAsync(new GetListTourSpec(id));
                    if (Tour.Any())
                    {
                        foreach (var item in Tour)
                        {
                            await tourBusiness.Delete(item.Id);
                        }
                        //await unitofWork.Repository<Tour>().DeleteRange(Tour);
                    }
                    await unitofWork.Repository<Category>().Delete(existingCategory);
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


        //duplicate name
        private async Task<bool> IsCategoryNameDuplicate(string categoryName)
        {

            // Sử dụng GetEntityWithSpecAsync để kiểm tra trùng lặp
            var duplicateCategory = await unitofWork.Repository<Category>()
                .GetEntityWithSpecAsync(new CategoryByNameSpecification(categoryName));

            return duplicateCategory != null;
        }
        public async Task<Pagination<CategoryDtos>> SelectAllCategoryPagination(SpecParams specParams)
        {

            var spec = new SearchCategorySpec(specParams);
            var resorts = await unitofWork.Repository<Category>().GetAllWithAsync(spec);

            var data = mapper.Map<IReadOnlyList<Category>, IReadOnlyList<CategoryDtos>>(resorts);
            var categoryPage = data.Skip((specParams.PageIndex - 1) * specParams.PageSize).Take(specParams.PageSize).ToList();

            var countSpec = new SearchCategorySpec(specParams);
            var count = await unitofWork.Repository<Category>().GetCountWithSpecAsync(countSpec);

            var totalPageIndex = count % specParams.PageSize == 0 ? count / specParams.PageSize : (count / specParams.PageSize) + 1;

            var pagination = new Pagination<CategoryDtos>(specParams.PageIndex, specParams.PageSize, categoryPage, count, totalPageIndex);

            return pagination;
        }
    }
}