using System.Security.Policy;

namespace backend.Dtos.PaymentDtos
{
    public class PaymentDtos
    {
        //Số tiền thanh toán
        public int Vnp_Amount { get; set; }
        //Mã Ngân hàng thanh toán
        public string Vnp_BankCode { get; set; }
        //Mã giao dịch tại Ngân hàng
        public string Vnp_BankTranNo { get; set; }
        
        public string Vnp_CardType { get; set; }
        //Thông tin mô tả nội dung thanh toán
        //(Tiếng Việt, không dấu).
        //Ví dụ: **Nap tien cho thue bao 0123456789. So tien 100,000 VND**
        public string Vnp_OrderInfo { get; set; }
        //Thời gian thanh toán. Định dạng: yyyyMMddHHmmss
        public string Vnp_PayDate { get; set; }
        //Mã phản hồi kết quả thanh toán.
        //Quy định mã trả lời 00 ứng với kết quả Thành công cho tất cả các API
        public string Vnp_Response { get; set; }
        //Mã website của merchant trên hệ thống của VNPAY
        public string Vnp_TmnCode { get; set; }
        //Mã giao dịch ghi nhận tại hệ thống VNPAY
        public string Vnp_TransactionNo { get; set; }
        //Mã phản hồi kết quả thanh toán. Tình trạng của giao dịch tại Cổng thanh toán VNPAY.
        //-00: Giao dịch thanh toán được thực hiện thành công tại VNPAY
        //-Khác 00: Giao dịch không thành công tại VNPAY
        public string Vnp_TransactionStatus { get; set; }
        //Giống mã gửi sang VNPAY khi gửi yêu cầu thanh toán
        public string Vnp_TxnRef { get; }
        //Mã kiểm tra (checksum) để đảm bảo dữ liệu của giao dịch không bị thay đổi
        //trong quá trình chuyển từ VNPAY về Website TMĐT.
        public string Vnp_SecureHash { get; }


    }
}