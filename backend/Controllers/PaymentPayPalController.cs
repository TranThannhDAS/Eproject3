using AutoMapper;
using backend.BussinessLogic;
using backend.Dao;
using backend.Dao.Specification.Order1;
using backend.Dtos.OrderDetailDtos;
using backend.Dtos.PaymentDtos;
using backend.Entity;
using backend.Exceptions;
using backend.Helper;
using backend.Model;
using backend.Model.Paypal.Capture;
using backend.Model.Paypal.Input;
using backend.Model.Paypal.Output;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using webapi.Dao.UnitofWork;
using webapi.Data;
using static StackExchange.Redis.Role;

namespace backend.Controllers
{
    [Route("/api/[controller]/[action]")]
    [ApiController]
    public class PaymentPayPalController : ControllerBase
    {
        public IConfiguration configuration;
        public HttpClient _client;
        public IUnitofWork unitofWork;
        public OrderBusinessLogic OrderBusinessLogic;
        public OrderDetailBusinessLogic OrderDetailBusinessLogic;
        public UserBussinessLogic UserBussinessLogic;
        public TourDetailBusinessLogic TourDetailBusinessLogic;
        public IMapper mapper;
        public Search_TourDetail_Dao searchDao;
        public DataContext context;
        public PaymentPayPalController(IConfiguration _configuration, HttpClient _httpClient, 
            IUnitofWork _unitofWork, OrderBusinessLogic _OrderBusinessLogic, OrderDetailBusinessLogic _OrderDetailBusinessLogic
            , UserBussinessLogic userBussinessLogic, TourDetailBusinessLogic tourDetailBusinessLogic, IMapper mapper, Search_TourDetail_Dao searchDao, DataContext dataContext)
        {
            configuration = _configuration;
            this._client = _httpClient;
            unitofWork = _unitofWork;
            OrderBusinessLogic = _OrderBusinessLogic;
            OrderDetailBusinessLogic = _OrderDetailBusinessLogic;
            UserBussinessLogic = userBussinessLogic;
            TourDetailBusinessLogic = tourDetailBusinessLogic;
            this.mapper = mapper;
            this.searchDao = searchDao;
            context = dataContext;
        }
        [HttpGet]
        public async Task<AuthorizationResponseData?> GetAuthorizationRequest()
        {
            var clientID = configuration.GetSection("PayPal").GetSection("ClientId").Value;
            var clientSecret = configuration.GetSection("Paypal").GetSection("ClientSecret").Value;
            //var baseUrl = configuration.GetSection("Paypal").GetSection("BaseUrl").Value;
            //_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var byteArray = Encoding.ASCII.GetBytes($"{clientID}:{clientSecret}");
            _client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(byteArray));

            List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();

            postData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));

            HttpResponseMessage response = await _client.PostAsync("https://api-m.sandbox.paypal.com/v1/oauth2/token", new FormUrlEncodedContent(postData));

            var responseAsString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var authorizationResponse = JsonConvert.DeserializeObject<AuthorizationResponseData>(responseAsString);
                if(authorizationResponse == null)
                {
                    throw new NotFoundExceptions("Không tìm thấy");
                }
                return authorizationResponse;
            }
            else
            {
                // Phản hồi không thành công, hiển thị nội dung lỗi
                Console.WriteLine("Lỗi phản hồi từ PayPal:");
                Console.WriteLine(responseAsString);
            }

            return null;

        }

     [HttpPost]
    public async Task<PaypalCreateOrderOutput> CreateOrder(OrderDetailDtos orderDetail)
    {
        var token = await GetAuthorizationRequest();

        var paypalInput = new PaypalCreateOrderInput
        {
            intent = "CAPTURE",
            purchase_units = new List<PurchaseUnitInput>
        {
            new PurchaseUnitInput
            {
                items = new List<ItemInput>
                {
                    new ItemInput
                    {
                        name = orderDetail.Name,
                        description = orderDetail.Description,
                        quantity = orderDetail.quantity,
                        unit_amount = new UnitAmountInput
                        {
                            currency_code = "USD",
                            value =  orderDetail.Price
                        }
                    }
                },
                amount = new AmountInput
                {
                    currency_code = "USD",
                    value = (int.Parse(orderDetail.Price)*int.Parse(orderDetail.quantity)).ToString(),
                    breakdown = new BreakdownInput
                    {
                        item_total = new ItemTotalInput
                        {
                            currency_code = "USD",
                            value = (int.Parse(orderDetail.Price)*int.Parse(orderDetail.quantity)).ToString()
                        }

                    }
                }
            }
        },
            application_context = new ApplicationContext
            {
                return_url = "https://example.com/return",
                cancel_url = "https://example.com/cancel"
            }
        };

        //_client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.access_token);


            var requestContent = JsonConvert.SerializeObject(paypalInput);
            //_client.DefaultRequestHeaders.Remove("Authorization");
            //_client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.access_token);
            //_client.SetBearerToken(token.access_token);
            

            var httpRequestMessage = new HttpRequestMessage
            {
                Content = new StringContent(requestContent, new MediaTypeHeaderValue("application/json"))
            };
            // Đặt giá trị "Content-Type"
            httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            // Tạo một giá trị ngẫu nhiên cho "PayPal-Request-Id" (hoặc bạn có thể sử dụng giá trị duy nhất khác)
            var payPalRequestId = Guid.NewGuid().ToString();
            httpRequestMessage.Headers.Add("PayPal-Request-Id", payPalRequestId);

            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);

            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await _client.PostAsync("https://api-m.sandbox.paypal.com/v2/checkout/orders", httpRequestMessage.Content);
            var responseAsString = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<PaypalCreateOrderOutput>(responseAsString);
         
            return result;

    }
        [HttpGet("{orderid}")]
        public async Task<ActionResult> GetOrderID(string orderid)
        {
            var token = await GetAuthorizationRequest();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);

            var response = await _client.GetAsync($"https://api-m.sandbox.paypal.com/v2/checkout/orders/{orderid}");

            var responseAsString = await response.Content.ReadAsStringAsync();
            var result_complete = JsonConvert.DeserializeObject<GetOrderDtos>(responseAsString);
         
            if (result_complete.status == "COMPLETE")
            {
                var result = GetCapturePayment(responseAsString);               
                return Ok(result);

            }
            else
            {
                var result = GetPaymentOutPutCreate(responseAsString);
                return Ok(result);
            }

        }
        [HttpGet("{json}")]
        public async Task<CapturePayment> GetCapturePayment(string json)
        {
            var result_complete = JsonConvert.DeserializeObject<CapturePayment>(json);
            return result_complete;

        }
        [HttpGet]
        public async Task<PaypalCreateOrderOutput> GetPaymentOutPutCreate(string json)
        {
            var result_complete = JsonConvert.DeserializeObject<PaypalCreateOrderOutput>(json);
            return result_complete;

        }

        /// <summary>
        /// kiểm tra xem TOURDETAILID này có trongdatabase chưa theo StartDate và Name nếu chưa thì Create
        /// Tìm kiếm xem có order này theo TourDetailID chưa
        /// nếu chưa thì
        ///       Tạo Order trước dựa trên TourDetailID: Tour_Detail_ID,
        /// Sau đó lưu vào OrderDetail có cột OrderID, Quantity, Price,UserID,Description,Type_Paymen,PAYMENT_ID
        ///        Update bảng Order Price, Quantity bằng cách lấy hết dữ liệu có trong Orderdetail và loop để lấy Price và Quantity
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CapturePayment1(PaymentPayPalDtos payment)
        {
            var token = await GetAuthorizationRequest();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);

            var captureData = new { note_to_payer = "Capture payment for order " + payment.orderid }; // Thay thế bằng thông tin cần thiết


            var captureJson = JsonConvert.SerializeObject(captureData);

            // Tạo nội dung yêu cầu
            var requestContent = new StringContent(captureJson, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("https://api-m.sandbox.paypal.com/v2/checkout/orders/" + payment.orderid + "/capture", requestContent);

            var responseAsString = await response.Content.ReadAsStringAsync();
            var check = JsonConvert.DeserializeObject<CapturePayment>(responseAsString);
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    if (check.status == "COMPLETED")
                    {

                        var exist_tour_detail = await searchDao.QueryDao(payment.Tour_Detail_Payment_Dto.Start_Date, payment.Tour_Detail_Payment_Dto.TourId);

                        if (exist_tour_detail == null)
                        {
                            var tour_detail = mapper.Map<Tour_Detail_PaymentPaypal_Dto, TourDetail>(payment.Tour_Detail_Payment_Dto);
                            exist_tour_detail = await TourDetailBusinessLogic.Create(tour_detail);
                        }

                        var response_capture = await _client.GetAsync($"https://api-m.sandbox.paypal.com/v2/checkout/orders/{payment.orderid}");
                        var responseAsString_capture = await response_capture.Content.ReadAsStringAsync();
                        var result = await GetCapturePayment(responseAsString_capture);

                        var check_duplicate_order = await OrderBusinessLogic.GetEntityByCondition(exist_tour_detail.Id);
                        if (check_duplicate_order == null)
                        {
                            var order = new Entity.Order();
                            order.Tour_Detail_ID = exist_tour_detail.Id;
                            check_duplicate_order = await OrderBusinessLogic.Create(order);
                        }
                        var oderdetail = new OrderDetail
                        {
                            OrderID = check_duplicate_order.Id,
                            Quantity = int.Parse(result.purchase_units[0].items[0].quantity),
                            Price = Double.Parse(result.purchase_units[0].payments.captures[0].seller_receivable_breakdown.net_amount.value),
                            UserID = payment.UserID,
                            Description = result.purchase_units[0].items[0].description + " |Paypal fee: " + Double.Parse(result.purchase_units[0].payments.captures[0].seller_receivable_breakdown.paypal_fee.value),
                            Type_Payment = "PayPal",
                            Payment_ID = result.id,
                            Tour_Detail_ID = exist_tour_detail.Id
                        };
                        await OrderDetailBusinessLogic.Create(oderdetail);
                        //CẬP NHẬT LẠI TOURDetail
                        exist_tour_detail.Quantity -= oderdetail.Quantity;
                        await TourDetailBusinessLogic.Update(exist_tour_detail);

                        // Lấy danh sách orderDetail liên quan đến tour_detail đã được thanh toán
                        var list_orderdetail = await OrderDetailBusinessLogic.SelectAllOrderDetail2(exist_tour_detail.Id);

                        // Tính toán lại totalOrderPrice và totalOrderQuantity dựa trên danh sách orderDetail
                        var totalOrderPrice = list_orderdetail.Sum(orderDetail => orderDetail.Price);
                        var totalOrderQuantity = list_orderdetail.Sum(orderDetail => orderDetail.Quantity);

                        // Cập nhật lại thông tin cho check_duplicate_order
                        check_duplicate_order.Price = totalOrderPrice;
                        check_duplicate_order.Number_people = totalOrderQuantity;
                        await OrderBusinessLogic.Update(check_duplicate_order);
                        //
                        transaction.Commit();
                        return Ok(new
                        {
                            message = "successful"
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            message = "Failed"
                        });
                    }

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("You have successfully paid but encounter problems during processing, please contact management", ex);
                }
            }

        }

    }
}
