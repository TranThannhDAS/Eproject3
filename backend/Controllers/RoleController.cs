using backend.BussinessLogic;
using backend.Dao.Specification;
using backend.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        public RoleBusinessLogic roleBusinessLogic;
        //private readonly IResponseCacheService responseCacheService;
        public RoleController(RoleBusinessLogic roleBussiness)
        {
            roleBusinessLogic = roleBussiness;
            //responseCacheService = responseCache;
        }

        // execute list all role
        [HttpGet]
        //[Cache(1400)]
        public async Task<ActionResult> ListRole()
        {
            var output = await roleBusinessLogic.SelectAllRole();
            if (output == null)
            {
                return NotFound();
            }
            return Ok(output);
        }

        //execute add new role
        [HttpPost]

        public async Task<IActionResult> Add(Role role)
        {

            await roleBusinessLogic.Create(role);

            var api = "/api/Role/ListRole*";
            //responseCacheService.RemoveCache(api);
            return Ok(role);
        }

        //execute update role
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Role role,int id)
        {

            await roleBusinessLogic.Update(role, id);
            return Ok(role);
        }



        //execute delete role
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    await roleBusinessLogic.Delete(id);
        //    return Ok();
        //}
        [HttpPost]
        public async Task<ActionResult> ListRolePagination(SpecParams pagination)
        {
            var output = await roleBusinessLogic.SelectAllRolePagination(pagination);

            // Kiểm tra xem trang có dữ liệu hay không
            if (output.Data.Count == 0)
            {
                return NotFound();
            }

            // Trả về dữ liệu phân trang và thông tin về trang
            return Ok(output);
        }
    }
}
