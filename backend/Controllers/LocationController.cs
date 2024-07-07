using backend.BussinessLogic;
using backend.Dao.Specification;
using backend.Entity;
using backend.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        public LocationBusinessLogic locationBussinessLogic;
        public LocationController(LocationBusinessLogic locationBussiness)
        {
            locationBussinessLogic = locationBussiness;
        }

        // execute list all location
        [HttpGet]
        public async Task<ActionResult> ListLocation1()
        {
            var output = await locationBussinessLogic.SelectAllLocation();
            if (output == null)
            {
                return NotFound();
            }
            return Ok(output);
        }
        [HttpPost]
        public async Task<ActionResult> ListLocation1Pagination(SpecParams pagination)
        {
            var output = await locationBussinessLogic.SelectAllLocation1Pagination(pagination);

            // Kiểm tra xem trang có dữ liệu hay không
            if (output.Data.Count == 0)
            {
                return NotFound();
            }

            // Trả về dữ liệu phân trang và thông tin về trang
            return Ok(output);
        }
        //execute add new location
        [HttpPost]

        public async Task<IActionResult> Add(Location1 location)
        {

            await locationBussinessLogic.Create(location);

            return Ok(location);
        }

        //execute update location
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Location1 location,int id)
        {

            await locationBussinessLogic.Update(location, id);
            return Ok(location);
        }

        //execute delete location
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await locationBussinessLogic.Delete(id);
            return Ok(new
            {
                message = "Delete success"
            });
        }
    }
}
