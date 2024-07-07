using AutoMapper;
using backend.Dao.Specification;
using backend.Dao.Specification.RoleSpec;
using backend.Entity;
using backend.Exceptions;
using backend.Helper;
using webapi.Dao.UnitofWork;
using webapi.Data;

namespace backend.BussinessLogic
{
    public class RoleBusinessLogic
    {
        public IUnitofWork unitofWork;
        public IMapper mapper;
        public DataContext context;
        public RoleBusinessLogic(IUnitofWork _unitofWork, IMapper mapper, DataContext context)
        {
            unitofWork = _unitofWork;
            this.mapper = mapper;
            this.context = context;
        }

        //list role
        public async Task<IReadOnlyList<Role>> SelectAllRole()
        {
            var data = await unitofWork.Repository<Role>().GetAllAsync();
            return data;
        }

        //create role
        public async Task Create(Role role)
        {
            if (role is null)
            {
                throw new NotFoundExceptions("Role not found");
            }

            //if (await IsRoleNameDuplicate(role.Name))
            //{
            //    throw new BadRequestExceptions("Role Name is exist.");
            //}


            await unitofWork.Repository<Role>().AddAsync(role);
            var check = await unitofWork.Complete();
            if (check < 1)
            {
                throw new BadRequestExceptions("chua dc thuc thi");
            }
        }

        //update role
        public async Task Update(Role role, int id)
        {
            if (role is null)
            {
                throw new NotFoundExceptions("not found");
            }

            var existingRole = await unitofWork.Repository<Role>().GetByIdAsync(id);

            if (existingRole is null)
            {
                throw new NotFoundExceptions("not found");
            }
            existingRole.UpdateDate = role.UpdateDate;
            existingRole.UpdateBy = role.UpdateBy;
            existingRole.Name = role.Name;
            existingRole.IsActive = role.IsActive;

            await unitofWork.Repository<Role>().Update(existingRole);
            var check = await unitofWork.Complete();
            if (check < 1)
            {
                throw new BadRequestExceptions("chua dc thuc thi");
            }
        }

        //delete role
        //public async Task Delete(int id)
        //{
        //    using (var transaction = context.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            var existingRole = await unitofWork.Repository<Role>().GetByIdAsync(id);
        //            if (existingRole == null)
        //            {
        //                throw new NotFoundExceptions("not found");
        //            }
        //            var user = await unitofWork.Repository<User>().GetAllWithAsync(new GetUserSpec(id));
        //            if (user.Any())
        //            {
        //                await unitofWork.Repository<User>().DeleteRange(user);
        //            }
        //            await unitofWork.Repository<Role>().Delete(existingRole);
        //            var check = await unitofWork.Complete();
        //            if (check < 1)
        //            {
        //                throw new BadRequestExceptions("chua dc thuc thi");
        //            }
        //            transaction.Commit(); // Commit giao dịch nếu mọi thứ thành công
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback(); // Rollback giao dịch nếu có ngoại lệ
        //            throw ex;
        //        }
        //    }
        //}


        //duplicate name
        //private async Task<bool> IsRoleNameDuplicate(string roleName)
        //{

        //    // Sử dụng GetEntityWithSpecAsync để kiểm tra trùng lặp
        //    var duplicateRole = await unitofWork.Repository<Role>()
        //        .GetEntityWithSpecAsync(new RoleByNameSpecification(roleName));

        //    return duplicateRole != null;
        //}
        public async Task<Pagination<Role>> SelectAllRolePagination(SpecParams specParams)
        {

            var spec = new SearchRoleSpec(specParams);
            var resorts = await unitofWork.Repository<Role>().GetAllWithAsync(spec);

            var data = mapper.Map<IReadOnlyList<Role>, IReadOnlyList<Role>>(resorts);
            var rolePage = data.Skip((specParams.PageIndex - 1) * specParams.PageSize).Take(specParams.PageSize).ToList();

            var countSpec = new SearchRoleSpec(specParams);
            var count = await unitofWork.Repository<Role>().GetCountWithSpecAsync(countSpec);

            var totalPageIndex = count % specParams.PageSize == 0 ? count / specParams.PageSize : (count / specParams.PageSize) + 1;

            var pagination = new Pagination<Role>(specParams.PageIndex, specParams.PageSize, rolePage, count, totalPageIndex);

            return pagination;
        }
    }
}
