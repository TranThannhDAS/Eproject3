using AutoMapper;
using backend.Dao;
using backend.Dtos.OrderDetailDtos;
using backend.Dtos.PaymentDtos;
using backend.Entity;
using backend.Exceptions;
using backend.Model;
using Microsoft.EntityFrameworkCore;
using webapi.Dao.UnitofWork;
using webapi.Data;

namespace backend.BussinessLogic
{
    public class PaymentBussinessLogic
    {
        public IUnitofWork unitofWork;
        private IConfiguration _configuration;
        private UserBussinessLogic UserBussinessLogic;
        private TourDetailBusinessLogic TourDetailBusinessLogic;
        private OrderBusinessLogic OrderBusinessLogic;
        private OrderDetailBusinessLogic OrderDetailBusinessLogic;
        public Search_TourDetail_Dao searchDao;
        public IMapper mapper;
        public DataContext context;
        public PaymentBussinessLogic(IUnitofWork _unitofWork, IConfiguration configuration, UserBussinessLogic userBussinessLogic, TourDetailBusinessLogic tourDetailBusinessLogic, OrderBusinessLogic orderBusinessLogic, OrderDetailBusinessLogic orderDetailBusinessLogic,Search_TourDetail_Dao search_TourDetail_Dao, IMapper mapper, DataContext dataContext)
        {
            unitofWork = _unitofWork;
            _configuration = configuration;
            this.UserBussinessLogic = userBussinessLogic;
            this.TourDetailBusinessLogic = tourDetailBusinessLogic;
            OrderBusinessLogic = orderBusinessLogic;
            OrderDetailBusinessLogic = orderDetailBusinessLogic;
            searchDao = search_TourDetail_Dao;
            this.mapper = mapper;
            context = dataContext;
        }
        public async Task<IReadOnlyList<OrderInfo>> SelectAllCategory()
        {
            var data = await unitofWork.Repository<OrderInfo>().GetAllAsync();
            return data;
        }
        public async Task CreatePayment(OrderInfo payment)
        {
            if (payment == null)
            {
                throw new NotFoundExceptions("Payment not found");
            }

            await unitofWork.Repository<OrderInfo>().AddAsync(payment);
            var check = await unitofWork.Complete();

            if (check > 0)
            {
                throw new BadRequestExceptions("Create Payment failed");
            }

        }
        public string GetUrlPayment(OrderDetailDtos orderDetail)
        {

            //Get Config Info
            string vnp_Returnurl = _configuration.GetSection("VNPayInfo").GetSection("vnp_Returnurl").Value; //URL nhan ket qua tra ve 
            string vnp_Url = _configuration.GetSection("VNPayInfo").GetSection("vnp_Url").Value; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = _configuration.GetSection("VNPayInfo").GetSection("vnp_TmnCode").Value; //Ma website
            string vnp_HashSecret = _configuration.GetSection("VNPayInfo").GetSection("vnp_HashSecret").Value; //Chuoi bi mat

            //Get payment input
            OrderInfo order = new OrderInfo();
            //Save order to db
            order.OrderId = Guid.NewGuid().ToString();
            order.Amount = orderDetail.Price;
            order.Quantity = int.Parse(orderDetail.quantity);
            order.OrderDescription = orderDetail.Description;
            order.CreatedDate = DateTime.Now;
            order.BankCode = "NCB";
            order.Status = 0;
            order.Tour_name = orderDetail.Name;
            string locale = "vn";
            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", ((Double.Parse(order.Amount)* order.Quantity) *100).ToString());
            //if (cboBankCode.SelectedItem != null && !string.IsNullOrEmpty(cboBankCode.SelectedItem.Value))
            //{
            //    vnpay.AddRequestData("vnp_BankCode", cboBankCode.SelectedItem.Value);
            //}
            //vnpay.AddRequestData("vnp_BankCode", "NCB");
            vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());


            if (!string.IsNullOrEmpty(locale))
            {
                vnpay.AddRequestData("vnp_Locale", locale);
            }
            else
            {
                vnpay.AddRequestData("vnp_Locale", "vn");
            }
            vnpay.AddRequestData("vnp_OrderInfo", order.OrderDescription);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.OrderId);


            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return paymentUrl;
        }
        
        public async Task<string> CreateDataAsync(PaymentVnPayDtos paymentVnPay)
        {
            VnPayLibrary vnpay = new VnPayLibrary();
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    if (paymentVnPay.vnp_ResponseCode == "00" && paymentVnPay.vnp_TransactionStatus == "00")
                    {

                        var exist_tour_detail = await searchDao.QueryDao(paymentVnPay.Tour_Detail_Payment_Dto.Start_Date, paymentVnPay.Tour_Detail_Payment_Dto.TourId);

                        if (exist_tour_detail == null)
                        {
                            var tour_detail = mapper.Map<Tour_Detail_PaymentPaypal_Dto, TourDetail>(paymentVnPay.Tour_Detail_Payment_Dto);
                            exist_tour_detail = await TourDetailBusinessLogic.Create(tour_detail);
                        }
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
                            Quantity = paymentVnPay.quantity,
                            Price = paymentVnPay.Amount / 100,
                            UserID = paymentVnPay.UserID,
                            Description = paymentVnPay.Description,
                            Type_Payment = "VNPAY",
                            Payment_ID = paymentVnPay.orderid,
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
                        return "Successfully";

                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("You have successfully paid but encounter problems during processing, please contact management");
                }
            }
            return "Payment failed";

        }
        
    }

}
