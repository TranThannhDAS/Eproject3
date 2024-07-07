using AutoMapper;
using backend.BussinessLogic;
using backend.Dtos.OrderDetailDtos;
using backend.Dtos.PaymentDtos;
using backend.Entity;
using backend.Exceptions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Specialized;
using System.Security.Policy;
using System.Web;

namespace backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PaymentVnPayController : ControllerBase
    {
        private readonly PaymentBussinessLogic paymentBussinessLogic;
        private readonly IMapper mapper;
        public PaymentVnPayController(PaymentBussinessLogic _paymentBussinessLogic, IMapper _mapper)
        {
            paymentBussinessLogic = _paymentBussinessLogic;
            mapper = _mapper;
        }
        [HttpPost]
        public async Task<ActionResult> GetPayment(OrderDetailDtos orderDetail)
        {
            var url = paymentBussinessLogic.GetUrlPayment(orderDetail);
            return Ok(new
            {
                message = url.ToString(),
            });
        }
        
        [HttpPost]
        public async Task<ActionResult> CreateDataAsync(PaymentVnPayDtos paymentVnPay)
        {
            var result = await paymentBussinessLogic.CreateDataAsync(paymentVnPay);
            return Ok(new
            {
                message = result
            });
        }
        [HttpGet]
        public async Task<IActionResult> CallBack()
        {
            string fullUrl = HttpContext.Request.GetDisplayUrl();
            Uri uri = new Uri(fullUrl);
            NameValueCollection queryParameters = HttpUtility.ParseQueryString(uri.Query);
            string orderid = queryParameters["vnp_TxnRef"];
            string vnp_Amount = queryParameters["vnp_Amount"];
            string description = queryParameters["vnp_OrderInfo"];
            string vnp_responeCode = queryParameters["vnp_ResponseCode"];
            string vnp_TransactionStatus = queryParameters["vnp_TransactionStatus"];

            return Ok(new
            {
                orderid = orderid,
                vnp_Amount = vnp_Amount,
                description = description,
                vnp_responeCode = vnp_responeCode,
                vnp_TransactionStatus = vnp_TransactionStatus,
            });
        }


    }
}
