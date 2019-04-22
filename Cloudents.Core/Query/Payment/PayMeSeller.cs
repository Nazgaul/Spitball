namespace Cloudents.Core.Query.Payment
{
    public class PayMeSeller
    {
        public string PaymeClientKey = "MPL15546-31186SKB-53ES24ZG-WGVCBKO2";
        public string SellerId { get; set; }
        public string SellerFirstName { get; set; }
        public string SellerLastName { get; set; }
        public string SellerSocialId { get; set; }
        public string SellerBirthdate { get; set; }
        public string SellerSocialIdIssued { get; set; }
        public int SellerGender { get; set; }
        public string SellerEmail { get; set; }
        public string SellerPhone { get; set; }
        public BankCode SellerBankCode { get; set; }
        public int SellerBankBranch { get; set; }
        public string SellerBankAccountNumber { get; set; }
        public string SellerDescription { get; set; }
        public string SellerSiteUrl { get; set; }
        public int SellerPersonBusinessType { get; set; }
        public int SellerInc => 0;
        public string SellerAddressCity { get; set; }
        public string SellerAddressStreet { get; set; }
        public string SellerAddressStreetNumber { get; set; }
        public string SellerAddressCountry => "IL";
        //public float MarketFee { get; set; }


    }

    public class PayMeSellerResponse
    {
        public int StatusCode { get; set; }
        public string SellerPaymeId { get; set; }
        public string SellerPaymeSecret { get; set; }
        public object SellerId { get; set; }
        public string SellerDashboardSignupLink { get; set; }
    }

    public enum BankCode
    {
        Yahav = 4,
        PostOffice = 9,
        Leumi = 10,
        Discount = 11,
        Hapoalim = 12,
        Igud = 13,
        Hahayal = 14,
        Mecantil = 17,
        Tfahot = 20,
        CitiBank = 22,
        Hsbc = 24,
        Ubank = 26,
        Harishon = 31,
        Aravi = 34,
        India = 39,
        Masad = 46,
        Fagi = 52,
        Yerushalaim = 54,
        Dexia = 68,
        LeumiLemshcantaot = 77,
        DiscountLemshcantaot = 90,
        MishkanLemshcantaot = 91,
        HabenLeumiLemshcantaot = 92
    }
}