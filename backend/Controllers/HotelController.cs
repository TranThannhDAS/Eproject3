using backend.BussinessLogic;
using backend.Dao.Specification;
using backend.Dtos;
using backend.Dtos.HotelDtos;
using backend.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        public HotelBusinessLogic hotelBusinessLogic;

        public HotelController(HotelBusinessLogic Bussiness)
        {
            hotelBusinessLogic = Bussiness;
        }

        // execute list all hotel
        [HttpGet]
        public async Task<ActionResult> ListHotel()
        {
            var output = await hotelBusinessLogic.SelectAllHotel();
            if (output == null)
            {
                return NotFound();
            }
            return Ok(output);
        }

        [HttpPost]
        public async Task<ActionResult> ListHotelPagination(SpecParams pagination)
        {
            var output = await hotelBusinessLogic.SelectAllHotelPagination(pagination);

            // Kiểm tra xem trang có dữ liệu hay không
            if (output.Data.Count == 0)
            {
                return NotFound();
            }

            // Trả về dữ liệu phân trang và thông tin về trang
            return Ok(output);
        }

        //execute add new hotel
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] HotelImageDto hotel)
        {
            await hotelBusinessLogic.Create(hotel);

            return Ok(hotel);
        }

        //execute update hotel
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] Hotel_Update_Dto hotel)
        {
            await hotelBusinessLogic.Update(hotel);
            return Ok(hotel);
        }

        //execute delete hotel
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
             var result = await hotelBusinessLogic.Delete(id);
            return Ok(new { message = result });
        }

        //get hotel by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByHotelId(int id)
        {
            var hotel = await hotelBusinessLogic.GetByHotelId(id);
            if (hotel == null)
            {
                return NotFound("not found hotel");
            }
            return Ok(hotel);
        }
    }
}
