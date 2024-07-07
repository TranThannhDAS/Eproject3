using backend.BussinessLogic;
using backend.Dao.Specification;
using backend.Dtos.StaffDtos;
using backend.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        public StaffBusinessLogic staffBusinessLogic;
        public StaffController(StaffBusinessLogic Bussiness)
        {
            staffBusinessLogic = Bussiness;
        }

        // execute list all staff
        [HttpGet]
        public async Task<ActionResult> ListStaff()
        {
            var output = await staffBusinessLogic.SelectAllStaff();
            if (output == null)
            {
                return NotFound();
            }
            return Ok(output);
        }

        //execute add new staff
        [HttpPost]

        public async Task<IActionResult> Add([FromForm] StaffImageDto staff)
        {

            await staffBusinessLogic.Create(staff);

            return Ok(staff);
        }

        //execute update staff
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] Staff_Update_Dto staff)
        {

            await staffBusinessLogic.Update(staff);
            return Ok(staff);
        }

        //execute delete staff
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await staffBusinessLogic.Delete(id);
            return Ok(new
            {
                message = "Delete success"
            });
        }
        [HttpPost]
        public async Task<ActionResult> ListStaffPagination(SpecParams pagination)
        {
            var output = await staffBusinessLogic.SelectAllStaffPagination(pagination);

            // Kiểm tra xem trang có dữ liệu hay không
            if (output.Data.Count == 0)
            {
                return NotFound();
            }

            // Trả về dữ liệu phân trang và thông tin về trang
            return Ok(output);
        }
        //get staff by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByStaffId(int id)
        {
            var staff = await staffBusinessLogic.GetByStaffId(id);
            if (staff == null)
            {
                return NotFound("not found staff");
            }
            return Ok(staff);
        }
    }
}
