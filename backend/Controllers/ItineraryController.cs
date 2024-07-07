using backend.BussinessLogic;
using backend.Dao.Specification;
using backend.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ItineraryController : ControllerBase
    {
        public ItineraryBusinessLogic itineraryBusinessLogic;
        public ItineraryController(ItineraryBusinessLogic Bussiness)
        {
            itineraryBusinessLogic = Bussiness;
        }

        // execute list all itinerary
        [HttpGet]
        public async Task<ActionResult> ListItinerary()
        {
            var output = await itineraryBusinessLogic.SelectAllItinerary();
            if (output == null)
            {
                return NotFound();
            }
            return Ok(output);
        }

        //[HttpPost]
        //public async Task<ActionResult> ListItineraryPagination(int pageIndex = 1, int pageSize = 10)
        //{
        //    var output = await itineraryBusinessLogic.SelectAllItineraryPagination(pageIndex, pageSize);

        //    // Kiểm tra xem trang có dữ liệu hay không
        //    if (output.Data.Count == 0)
        //    {
        //        return NotFound();
        //    }

        //    // Trả về dữ liệu phân trang và thông tin về trang
        //    return Ok(output);
        //}


        //execute add new itinerary
        [HttpPost]
        public async Task<IActionResult> Add(Itinerary itinerary)
        {

            await itineraryBusinessLogic.Create(itinerary);

            return Ok(itinerary);
        }

        //execute update itinerary
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Itinerary itinerary,int id)
        {

            await itineraryBusinessLogic.Update(itinerary, id);
            return Ok(itinerary);
        }

        //execute delete itinerary
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await itineraryBusinessLogic.Delete(id);
            return Ok(new
            {
                message = "Delete success"
            });
        }
        [HttpPost]
        public async Task<ActionResult> ListItineraryPagination(SpecParams pagination)
        {
            var output = await itineraryBusinessLogic.SelectAllItineraryPagination(pagination);

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
