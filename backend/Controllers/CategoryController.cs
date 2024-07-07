using backend.BussinessLogic;
using backend.Dao.Repository;
using backend.Dao.Specification;
using backend.Entity;
using backend.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System;

namespace backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    public class CategoryController : ControllerBase
    {
        public CategoryBusinessLogic categoryBussinessLogic;
        //private readonly IResponseCacheService responseCacheService;
        public CategoryController(CategoryBusinessLogic categoryBussiness)
        {
            categoryBussinessLogic = categoryBussiness;
            //responseCacheService = responseCache;
        }

        // execute list all category
        [HttpGet]
        //[Cache(1400)]
        public async Task<ActionResult> ListCategory()
        {
            var output = await categoryBussinessLogic.SelectAllCategory();
            if (output == null)
            {
                return NotFound();
            }
            return Ok(output);
        }

        //execute add new category
        [HttpPost]

        public async Task<IActionResult> Add(Category category)
        {
            
            await categoryBussinessLogic.Create(category);

            var api = "/api/Category/ListCategory*";
            //responseCacheService.RemoveCache(api);
            return Ok(category);
        }

        //execute update category
        [HttpPut("{id}")]
        public async Task<IActionResult> Update( Category category, int id)
        {

            await categoryBussinessLogic.Update(category,id);
            return Ok(category);
        }

        

        //execute delete category
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await categoryBussinessLogic.Delete(id);
            return Ok(new { message = "Delete success" });
        }
        [HttpPost]
        public async Task<ActionResult> ListCategoryPagination(SpecParams pagination)
        {
            var output = await categoryBussinessLogic.SelectAllCategoryPagination(pagination);

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
