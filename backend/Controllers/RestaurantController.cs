using backend.BussinessLogic;
using backend.Dao.Specification;
using backend.Dtos.RestaurantDtos;
using backend.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        public RestaurantBusinessLogic restaurantBusinessLogic;
        public RestaurantController(RestaurantBusinessLogic Bussiness)
        {
            restaurantBusinessLogic = Bussiness;
        }

        // execute list all restaurant
        [HttpGet]
        public async Task<ActionResult> ListRestaurant()
        {
            var output = await restaurantBusinessLogic.SelectAllRestaurant();
            if (output == null)
            {
                return NotFound();
            }
            return Ok(output);
        }

        //execute add new restaurant
        [HttpPost]

        public async Task<IActionResult> Add([FromForm]RestaurantImageDto restaurant)
        {

            await restaurantBusinessLogic.Create(restaurant);

            return Ok(restaurant);
        }

        //execute update restaurant
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] Restaurant_Update_Dto restaurant)
        {

            await restaurantBusinessLogic.Update(restaurant);
            return Ok(restaurant);
        }

        //execute delete restaurant
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await restaurantBusinessLogic.Delete(id);
            return Ok(new
            {
                message = "Delete success"
            });
        }
        //get hotel by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByRestaurantId(int id)
        {
            var res = await restaurantBusinessLogic.GetByRestaurantId(id);
            return Ok(res);
        }
        [HttpPost]
        public async Task<ActionResult> ListRestaurantPagination(SpecParams pagination)
        {
            var output = await restaurantBusinessLogic.SelectAllRestaurantPagination(pagination);

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
