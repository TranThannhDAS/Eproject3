using backend.BussinessLogic;
using backend.Dao.Specification;
using backend.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FeedBackController : ControllerBase
    {
        public FeedBackBusinessLogic feedBackBusinessLogic;
        public FeedBackController(FeedBackBusinessLogic Bussiness)
        {
            feedBackBusinessLogic = Bussiness;
        }

        // execute list all tour
        [HttpGet]
        public async Task<ActionResult> ListFeedBack()
        {
            var output = await feedBackBusinessLogic.SelectAllFeedBack();
            if (output == null)
            {
                return NotFound();
            }
            return Ok(output);
        }

        //execute add new tour
        [HttpPost]

        public async Task<IActionResult> Add(FeedBack tour)
        {

            await feedBackBusinessLogic.Create(tour);

            return Ok(tour);
        }

        //execute update tour
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(FeedBack tour, int id)
        {

            await feedBackBusinessLogic.Update(tour, id);
            return Ok(tour);
        }

        //execute delete tour
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await feedBackBusinessLogic.Delete(id);
            return Ok(new { message = "Delete success" });
        }
        [HttpPost]
        public async Task<ActionResult> ListFeedBackPagination(SpecParams pagination)
        {
            var output = await feedBackBusinessLogic.SelectAllFeedBackPagination(pagination);

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
