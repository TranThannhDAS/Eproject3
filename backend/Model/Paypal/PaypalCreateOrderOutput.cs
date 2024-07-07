using backend.Model.Paypal.Input;
using System.Runtime.Serialization;

namespace backend.Model.Paypal.Output
{
    public class PaypalCreateOrderOutput
    {
        public string id { get; set; }
        public string intent { get; set; }
        public string status { get; set; }
        public List<PurchaseUnitOutput> purchase_units { get; set; }
        public DateTime create_time { get; set; }
        [DataMember(Name = "Link")]
        public List<Link> links { get; set; }
    }
    public class PurchaseUnitOutput
    {
        public string reference_id { get; set; }
        public AmountOutput amount { get; set; }
        public Payee payee { get; set; }
        public List<Item> items { get; set; }
    }
    public class Item
    { 
        public string name { get; set; }
        public UnitAmountOutput unit_amount { get; set; }
        public string quantity { get; set; }
        public string? description { get; set; }
    }
    public class Link
    {
        public string href { get; set; }
        public string rel { get; set; }
        public string method { get; set; }
    }
    public class Payee
    {
        public string email_address { get; set; }
        public string merchant_id { get; set; }
    }
    public class UnitAmountOutput
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }
    public class AmountOutput
    {
        public string currency_code { get; set; }
        public string value { get; set; }
        public BreakdownOutput breakdown { get; set; }

    }
    public class BreakdownOutput
    {
        public ItemTotalOutput item_total { get; set; }
    }
    public class ItemTotalOutput
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }
}
