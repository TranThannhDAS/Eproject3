using System.ComponentModel.DataAnnotations;
using webapi.Base;

namespace backend.Entity
{
    public class Payment : BaseCreateDate
    {
        /// <summary>
        /// Merchant OrderId
        /// </summary>
        [Key]
        public string OrderId { get; set; }
        /// <summary>
        /// Payment amount
        /// </summary>
        public int Amount { get; set; }
        public string OrderDescription { get; set; }

        public string BankCode { get; set; }

        /// <summary>
        /// Order Status
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Creaed date
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// VNPAY Transaction Id
        /// </summary>
        public string Vnp_TransactionNo { get; set; }
        public string Vpn_Message { get; set; }
        public string Vpn_TxnResponseCode { get; set; }
    }
}
