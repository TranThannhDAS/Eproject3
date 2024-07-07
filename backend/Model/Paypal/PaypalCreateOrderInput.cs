namespace backend.Model.Paypal.Input
{
    public class PaypalCreateOrderInput
    {
        public string intent { get; set; }
        public List<PurchaseUnitInput> purchase_units { get; set; }
        public ApplicationContext application_context { get; set; }
    }
    public class PurchaseUnitInput
    {
        public List<ItemInput> items { get; set; }
        public AmountInput amount { get; set; }
    }
    public class ApplicationContext
    {
        public string return_url { get; set; }
        public string cancel_url { get; set; }
    }
    public class ItemInput
    {
        public string name { get; set; }
        public string description { get; set; }
        public string quantity { get; set; }

        public UnitAmountInput unit_amount { get; set; }
    }
    public class AmountInput
    {
        public string currency_code { get; set; }
        public string value { get; set; }
        public BreakdownInput breakdown { get; set; }

    }
    public class UnitAmountInput
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }
    public class BreakdownInput
    {

        public ItemTotalInput item_total { get; set; }
    }
    public class ItemTotalInput

    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }
}
