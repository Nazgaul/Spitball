namespace Cloudents.Core.Query.Payment
{



    public class GenerateSaleResponse
    {
        public int StatusCode { get; set; }

        public string StatusErrorDetails { get; set; }
        public string SaleUrl { get; set; }
        public string PaymeSaleId { get; set; }
        public int PaymeSaleCode { get; set; }
        public int Price { get; set; }
        public string TransactionId { get; set; }
        public string Currency { get; set; }
        public string SalePaymentMethod { get; set; }
        public string TransactionCcAuthNumber { get; set; }
    }



}