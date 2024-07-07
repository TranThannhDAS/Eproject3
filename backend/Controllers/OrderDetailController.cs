using backend.BussinessLogic;
using backend.Dao.Specification;
using backend.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        public OrderDetailBusinessLogic orderDetailBusinessLogic;
        public OrderDetailController(OrderDetailBusinessLogic Bussiness)
        {
            orderDetailBusinessLogic = Bussiness;
        }

        // execute list all orderDetail
        [HttpGet]
        public async Task<ActionResult> ListOrderDetail()
        {
            var output = await orderDetailBusinessLogic.SelectAllOrderDetail();
            if (output == null)
            {
                return NotFound();
            }
            return Ok(output);
        }

        //execute add new orderDetail
        [HttpPost]

        public async Task<IActionResult> Add(OrderDetail orderDetail)
        {

            await orderDetailBusinessLogic.Create(orderDetail);

            return Ok(orderDetail);
        }

        //execute update orderDetail
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(OrderDetail orderDetail,int id)
        {

            await orderDetailBusinessLogic.Update(orderDetail, id);
            return Ok(orderDetail);
        }

        //execute delete orderDetail
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await orderDetailBusinessLogic.Delete(id);
            return Ok(new
            {
                message = "Delete success"
            });
        }
        [HttpPost]
        public async Task<ActionResult> ListOrderDetailPagination(SpecParams pagination)
        {
            var output = await orderDetailBusinessLogic.SelectAllOrderDetailPagination(pagination);

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
