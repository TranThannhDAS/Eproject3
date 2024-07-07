using backend.BussinessLogic;
using backend.Dao.Specification;
using backend.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        public OrderBusinessLogic orderBusinessLogic;
        public OrderController(OrderBusinessLogic Bussiness)
        {
            orderBusinessLogic = Bussiness;
        }

        // execute list all order
        [HttpGet]
        public async Task<ActionResult> ListOrder()
        {
            var output = await orderBusinessLogic.SelectAllOrder();
            if (output == null)
            {
                return NotFound();
            }
            return Ok(output);
        }


        [HttpPost]
        public async Task<ActionResult> ListOrderPagination(SpecParams pagination)
        {
            var output = await orderBusinessLogic.SelectAllOrderPagination(pagination);
            if(output.Data.Count == 0) { 
            
                return NotFound();
            }
            return Ok(output);
        }
        //execute add new order
        [HttpPost]

        public async Task<IActionResult> Add(Order order)
        {

           var check_order =  await orderBusinessLogic.Create(order);

            return Ok(check_order);
        }

        //execute update order
        [HttpPut]
        public async Task<IActionResult> Update(Order order)
        {

            await orderBusinessLogic.Update(order);
            return Ok(order);
        }

        //execute delete order
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await orderBusinessLogic.Delete(id);
            return Ok(new
            {
                message = "Delete success"
            });
        }
        [HttpGet("{TourDetailID}")]
        public async Task<ActionResult> GetOrderByTourDetailId(int TourDetailID)
        {
            var output = await orderBusinessLogic.GetEntityByCondition(TourDetailID);
            if (output == null)
            {
                return NotFound();
            }
            return Ok(output);
        }
        [HttpGet]
        [Route("{year}/{month}")]
        public async Task<IActionResult> GetRevenueByMonth(int year, int month)
        {
            try
            {
                double revenueForMonth = await orderBusinessLogic.CalculateRevenueByMonth(year, month);

                return Ok(new { revenueForMonth = revenueForMonth });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return StatusCode(500, "Đã xảy ra lỗi trong quá trình tính toán");
            }
        }

        [HttpGet]
        [Route("{year}/{month}/{day}")]
        public async Task<IActionResult> GetRevenueByDay(int year, int month,int day)
        {
            try
            {
                double revenueForDay = await orderBusinessLogic.CalculateRevenueByDay(year, month,day);

                return Ok(new { revenueForDay = revenueForDay });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return StatusCode(500, "Đã xảy ra lỗi trong quá trình tính toán");
            }
        }
        [HttpGet]
        [Route("{year}")]
        public async Task<IActionResult> GetRevenueByYear(int year)
        {
            try
            {
                double revenueForYear = await orderBusinessLogic.CalculateRevenueByYear(year);

                return Ok(new { revenueForYear = revenueForYear });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return StatusCode(500, "Đã xảy ra lỗi trong quá trình tính toán");
            }
        }
        //get order by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByOrderId(int id)
        {
            var order = await orderBusinessLogic.GetByOrderId(id);
            if (order == null)
            {
                return NotFound("not found order");
            }
            return Ok(order);
        }
    }
}
