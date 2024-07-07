using backend.BussinessLogic;
using backend.Dao;
using backend.Dao.Specification;
using backend.Dtos.TourDetailDtos;
using backend.Dtos.TourDtos;
using backend.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TourDetailController : ControllerBase
    {
        public TourDetailBusinessLogic tourDetailBusinessLogic;
        public Search_Tour_Dao tourDetailSearch;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TourDetailController(TourDetailBusinessLogic Bussiness, Search_Tour_Dao search_Tour_Dao, IHttpContextAccessor httpContextAccessor)
        {
            tourDetailBusinessLogic = Bussiness;
            tourDetailSearch = search_Tour_Dao;
            _httpContextAccessor = httpContextAccessor;
        }

        // execute list all tourDetail
        [HttpGet]
        public async Task<ActionResult> ListTourDetail()
        {
            var output = await tourDetailBusinessLogic.SelectAllTourDetail();
            if (output == null)
            {
                return NotFound();
            }
            return Ok(output);
        }

        //execute add new tourDetail
        [HttpPost]

        public async Task<IActionResult> Add(TourDetail tourDetail)
        {

           var test =  await tourDetailBusinessLogic.Create(tourDetail);

            return Ok(test);
        }

        //execute update tourDetail
        [HttpPut]
        public async Task<IActionResult> Update(TourDetail tourDetail)
        {

            await tourDetailBusinessLogic.Update(tourDetail);
            return Ok(tourDetail);
        }

        //execute delete tourDetail
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await tourDetailBusinessLogic.Delete(id);
            return Ok(new { message = "Delete success" });
        }
        [HttpPut]
        public async Task<IActionResult> Update_User(TourDetail_By_Update_UserDto tourDetail_By_Update_UserDto)
        {
            await tourDetailBusinessLogic.Update_User(tourDetail_By_Update_UserDto);
            return Ok("Success");
        }
        [HttpPost]
        public async Task<IActionResult> SearchTour(Search_Tour_Dto_Input search_Tour_Dto)
        {
            var result = await tourDetailSearch.Search_Tour(search_Tour_Dto);        
            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult> ListTourDetailPagination(SpecParams pagination)
        {
            var output = await tourDetailBusinessLogic.SelectAllTourDetailPagination(pagination);

            // Kiểm tra xem trang có dữ liệu hay không
            if (output.Data.Count == 0)
            {
                return NotFound();
            }

            // Trả về dữ liệu phân trang và thông tin về trang
            return Ok(output);
        }


        //get tourDetail by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByTourDetailId(int id)
        {
            var tourDetail = await tourDetailBusinessLogic.GetByTourDetailId(id);
            if (tourDetail == null)
            {
                return NotFound("not found tourDetail");
            }
            return Ok(tourDetail);
        }

    }
}
