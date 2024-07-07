using AutoMapper;
using backend.BussinessLogic;
using backend.Dao;
using backend.Dao.Specification;
using backend.Dao.Specification.TourSpec;
using backend.Dtos.TourDtos;
using backend.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TourController : ControllerBase
    {
        public TourBusinessLogic tourBusinessLogic;
        public Top_10_Tour_Dao top_10_Tour_Dao;
        public TourController(TourBusinessLogic Bussiness, Top_10_Tour_Dao top_10_Tour_Dao)
        {
            tourBusinessLogic = Bussiness;
            this.top_10_Tour_Dao = top_10_Tour_Dao;
        }

        // execute list all tour
        [HttpGet]
        public async Task<ActionResult> ListTour()
        {
            var output = await tourBusinessLogic.SelectAllTour();
            if (output == null)
            {
                return NotFound();
            }
            return Ok(output);
        }

        //execute add new tour
        [HttpPost]

        public async Task<IActionResult> Add([FromForm] TourDto tourdto)
        {
            await tourBusinessLogic.Create(tourdto);

            return Ok(tourdto);
        }

        //execute update tour
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] Tour_Update_Dto tourdto)
        {

            await tourBusinessLogic.Update(tourdto);
            return Ok(tourdto);
        }

        //execute delete tour
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await tourBusinessLogic.Delete(id);
            return Ok(new
            {
                message = "Delete success"
            });
        }
        //get tour by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByTourId(int id)
        {
           var result =  await tourBusinessLogic.GetByTourId(id);
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> Get_Top_10_Tour()
        {
            var result = await top_10_Tour_Dao.Top_10_Tour();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> ListTourPagination(SpecParams pagination)
        {
            var output = await tourBusinessLogic.SelectAllTourPagination(pagination);

            // Kiểm tra xem trang có dữ liệu hay không
            if (output.Data.Count == 0)
            {
                return NotFound();
            }

            // Trả về dữ liệu phân trang và thông tin về trang
            return Ok(output);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> UpdateRating(int id)
        {
            var averageRating = await tourBusinessLogic.CreateWithRating(id);
            return Ok(new { averageRating = averageRating });
        }

    }
}
