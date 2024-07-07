using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using webapi.Base;

namespace backend.Model
{
    public class OrderInfo : BaseCreateDate
    {
        /// <summary>
        /// Merchant OrderId
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string OrderId { get; set; }
        /// <summary>
        /// Payment amount
        /// </summary>
        public string? Amount { get; set; }
        public int Quantity { get; set; }
        public string? OrderDescription { get; set; }

        public string? BankCode { get; set; }
        public string? Tour_name { get; set; }

        /// <summary>
        /// Order Status
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// Creaed date
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// VNPAY Transaction Id
        /// </summary>
        public string? Vnp_TransactionNo { get; set; }
        public string? Vpn_Message { get; set; }
        public string? Vpn_TxnResponseCode { get; set; }
    }
}
