using backend.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;

namespace backend.Dtos.PaymentDtos
{
    public class PaymentPayPalDtos
    {
        public int TourDetailID { get; set; }
        public int UserID { get; set; }
        public string orderid { get; set; }  //Trong vnpay là vnp_TxnRef còn PayPal là ID
        public Tour_Detail_PaymentPaypal_Dto Tour_Detail_Payment_Dto { get; set; }
    }
    public class Tour_Detail_PaymentPaypal_Dto
    {
        public int TourId { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime? End_Date { get; set; }
        public int? Quantity { get; set; }
        public int? Staff_Id { get; set; } = 1;
    

    }
}