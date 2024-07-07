using backend.BussinessLogic;
using backend.Dao.Specification;
using backend.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TransportationController : ControllerBase
    {
        public TransportationBusinessLogic transportationBusinessLogic;
        public TransportationController(TransportationBusinessLogic transportationBussiness)
        {
            transportationBusinessLogic = transportationBussiness;
        }

        // execute list all transportation
        [HttpGet]
        public async Task<ActionResult> ListTransportation()
        {
            var output = await transportationBusinessLogic.SelectAllTransportation();
            if (output == null)
            {
                return NotFound();
            }
            return Ok(output);
        }

        //execute add new transportation
        [HttpPost]

        public async Task<IActionResult> Add(Transportation transportation)
        {

            await transportationBusinessLogic.Create(transportation);

            return Ok(transportation);
        }

        //execute update transportation
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Transportation transportation,int id)
        {

            await transportationBusinessLogic.Update(transportation, id);
            return Ok(transportation);
        }

        //execute delete transportation
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await transportationBusinessLogic.Delete(id);
            return Ok(new
            {
                message = "Delete success"
            });
        }
        [HttpPost]
        public async Task<ActionResult> ListTransportationPagination(SpecParams pagination)
        {
            var output = await transportationBusinessLogic.SelectAllTransportationPagination(pagination);

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
