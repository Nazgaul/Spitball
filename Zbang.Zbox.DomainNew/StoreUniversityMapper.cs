namespace Zbang.Zbox.Domain
{
    internal class StoreUniversityMapper
    {
        public int Id { get; set; }
        public int StoreUniversityId { get; set; }
        public University University { get; set; }

        public int CouponCode { get; set; }
    }
}
