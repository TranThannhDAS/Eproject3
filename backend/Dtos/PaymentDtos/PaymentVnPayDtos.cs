namespace backend.Dtos.PaymentDtos
{
    public class PaymentVnPayDtos : PaymentPayPalDtos
    {
        public string vnp_ResponseCode { get; set; }
        public string vnp_TransactionStatus { get; set; }
        public string Description { get; set; } // vnp_OrderInfo
        public double Amount { get; set; } // vnp_Amount
        public int quantity { get; set; }
        
    }
}
