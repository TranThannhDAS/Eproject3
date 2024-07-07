using backend.BussinessLogic;
using backend.Dao.Specification;
using backend.Entity;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InformationController : ControllerBase
    {

        public InformationBusinessLogic informationBusinessLogic;
        public InformationController(InformationBusinessLogic Bussiness)
        {
            informationBusinessLogic = Bussiness;
        }

        // execute list all information
        [HttpGet]
        public async Task<ActionResult> ListInformation()
        {
            var output = await informationBusinessLogic.SelectAllInformation();
            if (output == null)
            {
                return NotFound();
            }
            return Ok(output);
        }

        //[HttpPost]
        //public async Task<ActionResult> ListInformationPagination(int pageIndex = 1, int pageSize = 10)
        //{
        //    var output = await informationBusinessLogic.SelectAllInformationPagination(pageIndex, pageSize);

        //    // Kiểm tra xem trang có dữ liệu hay không
        //    if (output.Data.Count == 0)
        //    {
        //        return NotFound();
        //    }

        //    // Trả về dữ liệu phân trang và thông tin về trang
        //    return Ok(output);
        //}


        //execute add new information
        [HttpPost]
        public async Task<IActionResult> Add(Information information)
        {

            await informationBusinessLogic.Create(information);

            return Ok(information);
        }

        //execute update information
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Information information,int id)
        {

            await informationBusinessLogic.Update(information, id);
            return Ok(information);
        }

        //execute delete information
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await informationBusinessLogic.Delete(id);
            return Ok(new
            {
                message = "Delete success"
            });
        }
        [HttpPost]
        public async Task<ActionResult> ListInformationPagination(SpecParams pagination)
        {
            var output = await informationBusinessLogic.SelectAllInformationPagination(pagination);

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
