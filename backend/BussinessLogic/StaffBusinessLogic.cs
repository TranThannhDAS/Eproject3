using AutoMapper;
using backend.Dao.Specification.StaffSpec;
using backend.Dao.Specification;
using backend.Dtos.StaffDtos;
using backend.Entity;
using backend.Exceptions;
using backend.Helper;
using webapi.Dao.UnitofWork;
using backend.Dtos.ResortDtos;
using webapi.Data;
using Microsoft.EntityFrameworkCore;
using backend.Dtos.RestaurantDtos;

namespace backend.BussinessLogic
{
    public class StaffBusinessLogic
    {
        public IUnitofWork unitofWork;
        public IMapper mapper;
        private ImageService Image;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private DataContext context;
        public StaffBusinessLogic(IUnitofWork _unitofWork, IMapper mapper, ImageService Image, IHttpContextAccessor _httpContextAccessor , DataContext context)
        {
            unitofWork = _unitofWork;
            this.mapper = mapper;
            this.Image = Image;
            this._httpContextAccessor = _httpContextAccessor;
            this.context = context;
        }

        //list staff
        public async Task<IEnumerable<object>> SelectAllStaff()
        {
            var data = await unitofWork.Repository<Staff>().GetAllAsync();
            var httpRequest = _httpContextAccessor.HttpContext.Request;
            var result = new List<object>();

            foreach (var staff in data)
            {
                var staffInfo = new
                {
                    staff.Id,
                    staff.Name,
                    staff.Image,
                    staff.Phone,
                    staff.Email,
                    staff.PersonId,
                    UrlImage = Image.GetUrlImage(staff.Name, "staff", httpRequest) // Gọi phương thức GetUrlImage11 cho từng bản ghi
                };

                result.Add(staffInfo);
            }
            return result;
        }

        //create staff
        public async Task Create(StaffImageDto staffDto)
        {
            var staff = mapper.Map<StaffImageDto, Staff>(staffDto);
            if (staff is null)
            {
                throw new NotFoundExceptions("Staff not found");
            }
            var Name_replace = staffDto.Name.Replace(" ", "-");
            var image_folder = Name_replace + "-" + staff.PersonId;
            var images = await Image.Upload_Image(image_folder, "staff", staffDto.fileCollection);
            foreach (var image in images)
            {
                staff.AddImage(image);
            }
            await unitofWork.Repository<Staff>().AddAsync(staff);
            var check = await unitofWork.Complete();
            if (check < 1)
            {
                throw new BadRequestExceptions("chua dc thuc thi");
            }
        }

        //update staff
        public async Task Update(Staff_Update_Dto staffDto)
        {
            var staff = mapper.Map<Staff_Update_Dto, Staff>(staffDto);
            if (staff is null)
            {
                throw new NotFoundExceptions("not found");
            }

            var existingStaff = await unitofWork.Repository<Staff>().GetByIdAsync(staff.Id);

            if (existingStaff is null)
            {
                throw new NotFoundExceptions("not found");
            }
            var Name_replace = staffDto.Name.Replace(" ", "-");
            var image_folder = Name_replace + "-" + existingStaff.PersonId;
            var images = await Image.Update_Image(image_folder, existingStaff.Name.Replace(" ", "-") + "-" + existingStaff.PersonId, "staff", staffDto.path, staffDto.fileCollection);
            images.Add("JPG.JPG");
            foreach (var image in images)
            {
                staff.AddImage(image);
            }
            existingStaff.UpdateDate = staff.UpdateDate;
            existingStaff.UpdateBy = staff.UpdateBy;
            existingStaff.CreateBy = staff.CreateBy;
            existingStaff.IsActive = staff.IsActive;
            existingStaff.Name = staff.Name;
            existingStaff.Image = staff.Image;
            existingStaff.Phone = staff.Phone;
            existingStaff.Email = staff.Email;
            existingStaff.PersonId = staff.PersonId;

            await unitofWork.Repository<Staff>().Update(existingStaff);
            var check = await unitofWork.Complete();
            if (check < 1)
            {
                throw new BadRequestExceptions("chua dc thuc thi");
            }
        }

        //delete staff
        public async Task Delete(int id)
        {

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var existingStaff = await unitofWork.Repository<Staff>().GetByIdAsync(id);
                    if (existingStaff == null)
                    {
                        throw new NotFoundExceptions("not found");
                    }

                    var StaffHaveTourDetailId = await unitofWork.Repository<TourDetail>().GetAllWithAsync(new StaffDeleteTourDetailSpec(id));
                    if (StaffHaveTourDetailId.Any())
                    {
                        await unitofWork.Repository<TourDetail>().DeleteRange(StaffHaveTourDetailId);
                    }


                    await unitofWork.Repository<Staff>().Delete(existingStaff);

                    var check = await unitofWork.Complete();
                    if (check < 1)
                    {
                        throw new BadRequestExceptions("chua dc thuc thi");
                    }
                    var Name_replace = existingStaff.Name.Replace(" ", "-");
                    var image_folder = Name_replace + "-" + existingStaff.PersonId;
                    var delete_image = Image.DeleteImage(image_folder, "staff");
                    transaction.Commit(); // Commit giao dịch nếu mọi thứ thành công
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // Rollback giao dịch nếu có ngoại lệ
                    throw ex;
                }
            }
        }
        //get staff by id
        public async Task<object> GetByStaffId(int id)
        {
            var staff = await unitofWork.Repository<Staff>().GetByIdAsync(id);
            if (staff == null)
            {
                throw new NotFoundExceptions("not found");
            }
            var Name_replace = staff.Name.Replace(" ", "-");
            var image_folder = Name_replace + "-" + staff.PersonId;
            var httpRequest = _httpContextAccessor.HttpContext.Request;
            var result = new
            {
                staff.Id,
                staff.Name,
                staff.Image,
                staff.Phone,
                staff.Email,
                staff.PersonId,
                urlImage = Image.GetUrlImage(image_folder, "staff", httpRequest)
            };
            return result;
        }
        public async Task<Pagination<StaffDto>> SelectAllStaffPagination(SpecParams specParams)
        {

            var httpRequest = _httpContextAccessor.HttpContext.Request;
            var spec = new SearchStaffSpec(specParams);
            var result = new List<StaffDto>();
            int count = await unitofWork.Repository<Staff>().GetCountWithSpecAsync(spec);
            var staffPage = new List<Staff>();
            if (string.IsNullOrEmpty(specParams.Search))
            {
                staffPage = await context.Staff.Skip((specParams.PageIndex - 1) * specParams.PageSize).Take(specParams.PageSize).ToListAsync();
            }
            else
            {
                var staffs = await unitofWork.Repository<Staff>().GetAllWithAsync(spec);

                staffPage = staffs.Skip((specParams.PageIndex - 1) * specParams.PageSize).Take(specParams.PageSize).ToList();

            }
            foreach (var staff in staffPage)
            {
                var Name_replace = staff.Name.Replace(" ", "-");
                var image_folder = Name_replace + "-" + staff.PersonId;
                var staffInfo = new StaffDto
                {
                    Id = staff.Id,
                    Name = staff.Name,
                    Phone = staff.Phone,
                    Email = staff.Email,
                    PersonId = staff.PersonId,
                    UrlImage = Image.GetUrlImage(image_folder, "staff", httpRequest)
                };
                result.Add(staffInfo);
            }
            var totalPageIndex = count % specParams.PageSize == 0 ? count / specParams.PageSize : (count / specParams.PageSize) + 1;

            var pagination = new Pagination<StaffDto>(specParams.PageIndex, specParams.PageSize, result, count, totalPageIndex);

            return pagination;
        }
     
    }
}
