using backend.BussinessLogic;
using backend.Dao.Specification;
using backend.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        public ServiceBusinessLogic serviceBusinessLogic;
        public ServiceController(ServiceBusinessLogic Bussiness)
        {
            serviceBusinessLogic = Bussiness;
        }

        // execute list all service
        [HttpGet]
        public async Task<ActionResult> ListService()
        {
            var output = await serviceBusinessLogic.SelectAllService();
            if (output == null)
            {
                return NotFound();
            }
            return Ok(output);
        }

        //execute add new service
        [HttpPost]

        public async Task<IActionResult> Add(Service service)
        {

            await serviceBusinessLogic.Create(service);

            return Ok(service);
        }

        //execute update service
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Service service,int id)
        {

            await serviceBusinessLogic.Update(service, id);
            return Ok(service);
        }

        //execute delete service
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await serviceBusinessLogic.Delete(id);
            return Ok(new
            {
                message = "Delete success"
            });
        }
        [HttpPost]
        public async Task<ActionResult> ListServicePagination(SpecParams pagination)
        {
            var output = await serviceBusinessLogic.SelectAllServicePagination(pagination);

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
