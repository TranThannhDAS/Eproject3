using backend.Model.Paypal.Output;
using System.Runtime.Serialization;

namespace backend.Model.Paypal.Capture
{
    public class CapturePayment
    {
        public string id { get; set; }
        public string intent { get; set; }
        public string status { get; set; }
        public PaymentSource payment_source { get; set; }
        public List<PurchaseUnit> purchase_units { get; set; }
        public Payer_Capture payer { get; set; }
        public DateTime create_time { get; set; }
        public DateTime update_time { get; set; }
        public List<Link> links { get; set; }
    }

    public class PaymentSource
    {
        public Paypal paypal { get; set; }
    }

    public class Paypal
    {
        public string email_address { get; set; }
        public string account_id { get; set; }
        public string account_status { get; set; }
        public Name name { get; set; }
        public Address address { get; set; }
    }

    public class Name
    {
        public string given_name { get; set; }
        public string surname { get; set; }
    }

    public class Address
    {
        public string country_code { get; set; }
    }

    public class PurchaseUnit
    {
        public string reference_id { get; set; }
        public Amount_Capture amount { get; set; }
        public Payee_Capture payee { get; set; }
        public string description { get; set; }
        public List<Item> items { get; set; }
        public Shipping_Capture shipping { get; set; }
        public Payments_Capture payments { get; set; }
    }

    public class Amount_Capture
    {
        public string currency_code { get; set; }
        public string value { get; set; }
        public Breakdown breakdown { get; set; }
    }

    public class Breakdown
    {
        public ItemTotal item_total { get; set; }
        public Amount_Capture shipping { get; set; }
        public Amount_Capture handling { get; set; }
        public Amount_Capture insurance { get; set; }
        public Amount_Capture shipping_discount { get; set; }
    }

    public class ItemTotal
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }

    public class Payee_Capture
    {
        public string email_address { get; set; }
        public string merchant_id { get; set; }
    }

    public class Item_Capture
    {
        public string name { get; set; }
        public UnitAmount_Capture unit_amount { get; set; }
        public Tax_Capture tax { get; set; }
        public string quantity { get; set; }
        public string description { get; set; }
        public string image_url { get; set; }
    }

    public class UnitAmount_Capture
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }

    public class Tax_Capture
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }

    public class Shipping_Capture
    {
        public Name name { get; set; }
        public Address address { get; set; }
    }

    public class Payments_Capture
    {
        public List<Capture_Capture> captures { get; set; }
    }

    public class Capture_Capture
    {
        public string id { get; set; }
        public string status { get; set; }
        public Amount_Capture amount { get; set; }
        public bool final_capture { get; set; }
        public SellerProtection_Capture seller_protection { get; set; }
        public SellerReceivableBreakdown_Capture seller_receivable_breakdown { get; set; }
        public List<Link> links { get; set; }
        public DateTime create_time { get; set; }
        public DateTime update_time { get; set; }
    }

    public class SellerProtection_Capture
    {
        public string status { get; set; }
        public List<string> dispute_categories { get; set; }
    }

    public class SellerReceivableBreakdown_Capture
    {
        public Amount_Capture gross_amount { get; set; }
        public Amount_Capture paypal_fee { get; set; }
        public Amount_Capture net_amount { get; set; }
    }

    public class Link_Capture
    {
        public string href { get; set; }
        public string rel { get; set; }
        public string method { get; set; }
    }
    public class Payer_Capture
    {
        public Name name { get; set; }
        public string email_address { get; set; }
        public string payer_id { get; set; }
        public Address address { get; set; }
    }


}
