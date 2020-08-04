using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Cloudents.Core.Entities
{
    public sealed class Country : Enumeration
    {

        public RegionInfo RegionInfo { get; }
        public decimal ConversationRate { get; }
        public Language MainLanguage { get; }

        public override string ToString()
        {
            return RegionInfo.TwoLetterISORegionName;
        }


        private Country(int id, string info, decimal conversationRate, Language language) : base(id, info)
        {
            ConversationRate = conversationRate;
            // TwilioMediaRegion = twilioMediaRegion;
            RegionInfo = new RegionInfo(info.ToUpperInvariant());
            MainLanguage = language;
        }

        public static Country FromCountry(string? tb)

        //public static implicit operator Country(string? tb)
        {
            if (tb is null)
            {
                return UnitedStates;
            }
            var result = FromDisplayName<Country>(tb);
            if (result is null)
            {
                return UnitedStates;
            }
            return result;
        }

        //public static explicit operator Country(int s)
        //{
        //    return FromValue<Country>(s);
        //}

        public const string IsraelStr = "IL";
        public const string IndiaStr = "IN";
        public const string UsStr = "US";
        public static readonly Country Israel = new Country(1, IsraelStr, 1 / 25M, Language.Hebrew);
        public static readonly Country India = new Country(2, IndiaStr, 1, Language.EnglishIndia);
        public static readonly Country UnitedStates = new Country(3, UsStr, 1 / 100M, Language.EnglishUsa);

        private bool Equals(Country other)
        {
            return RegionInfo.Equals(other.RegionInfo);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is Country other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RegionInfo);
        }

        public static bool operator ==(Country? left, Country? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Country? left, Country? right)
        {
            return !Equals(left, right);
        }


        //public static readonly HashSet<string> CountriesNotSupported = new HashSet<string>()
        //{
        //    //"DZ", "AO", "BJ", "BW", "BF", "BI", "CM",
        //    //"CV", "CF", "KM", "CD", "DJ", "EG", "GQ", "ER", "ET", "GA", "GM", "GH", "GN", "GW", "CI", "KE",
        //    //"LS", "LR", "LY",
        //    //"MG", "MW", "ML", "MR", "MU", "MA", "MZ", "NA", "NE", "NG", "CG", "RE", "RW", "SH", "ST", "SN",
        //    //"SC", "SL", "SO",
        //    //"SS", "SD", "SZ", "TZ", "TG", "TN", "UG", "EH", "ZM", "ZW", "AF", "AM", "AZ", "BH", "BD", "BT",
        //    //"BN", "KH",
        //    //"ID", "IR", "IQ", "JO", "KZ", "KW", "KG", "LA", "LB", "MO", "MY", "MV", "MN", "MM", "NP",
        //    //"KP", "OM", "PK",
        //    //"PH", "QA", "SA", "LK", "SY", "TW", "TJ", "TH", "TR", "TM", "AE", "UZ", "VN", "YE"
        //};
        //    public static Country Palestine = new Country("PS", CountryGroup.Israel);

        //    public static Country Afghanistan = new Country("AF", CountryGroup.Tier3);
        //    public static Country Bangladesh = new Country("BD", CountryGroup.Tier3);
        //    public static Country Bahrain = new Country("BH", CountryGroup.Tier3);
        //    public static Country BruneiDarussalam = new Country("BN", CountryGroup.Tier3);
        //    public static Country Bhutan = new Country("BT", CountryGroup.Tier3);
        //    public static Country Cocos = new Country("CC", CountryGroup.Tier3);
        //    public static Country ChristmasIsland = new Country("CX", CountryGroup.Tier3);
        //    public static Country Indonesia = new Country("ID", CountryGroup.Tier3);

        //    public static Country TheBritishIndianOceanTerritory = new Country("IO", CountryGroup.Tier3);
        //    public static Country Iraq = new Country("IQ", CountryGroup.Tier3);
        //    public static Country Iran = new Country("IR", CountryGroup.Tier3);
        //    public static Country Japan = new Country("JP", CountryGroup.Tier3);
        //    public static Country Cambodia = new Country("KH", CountryGroup.Tier3);
        //    public static Country Lao = new Country("LA", CountryGroup.Tier3);
        //    public static Country Lanka = new Country("LK", CountryGroup.Tier3);
        //    public static Country Myanmar = new Country("MM", CountryGroup.Tier3);
        //    public static Country Maldives = new Country("MV", CountryGroup.Tier3);
        //    public static Country Malaysia = new Country("MY", CountryGroup.Tier3);
        //    public static Country Nepal = new Country("NP", CountryGroup.Tier3);
        //    public static Country Philippines = new Country("PH", CountryGroup.Tier3);
        //    public static Country Pakistan = new Country("PK", CountryGroup.Tier3);
        //    public static Country Singapore = new Country("SG", CountryGroup.Tier3);
        //    public static Country Thailand = new Country("TH", CountryGroup.Tier3);
        //    public static Country Turkmenistan = new Country("TM", CountryGroup.Tier3);
        //    public static Country Taiwan = new Country("TW", CountryGroup.Tier3);
        //    public static Country VietNam = new Country("VN", CountryGroup.Tier3);



        //    static IEnumerable<Country> GetCountries()
        //    {
        //        var z = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
        //            .Select(s => new RegionInfo(s.LCID))
        //            .GroupBy(g => g.TwoLetterISORegionName)






        //        return from ri in
        //                from ci in CultureInfo.GetCultures(CultureTypes.SpecificCultures)
        //                select new RegionInfo(ci.LCID)
        //            group ri by ri.TwoLetterISORegionName into g
        //                //where g.Key.Length == 2
        //            select new Country
        //            {
        //                //CountryId = g.Key,
        //                //Title = g.First().DisplayName
        //            };
        //    }
    }


    [DataContract]
    public struct Money

    {
        public Money(decimal amount, string currency) : this((double)amount, currency)
        {

        }


        public Money(double amount, string currency)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Value cannot be negative");
            }
            Amount = RoundToNearestPenny(amount);
            Cents = ToPennies(Amount);
            Currency = currency;
        }

        private static int ToPennies(double amount)
        {
            return Convert.ToInt32(amount * 100);
        }

        private static double RoundToNearestPenny(double amount)
        {
            double quotient = amount / .01;
            int wholePart = (int)quotient;
            decimal mantissa = ((decimal)quotient) - wholePart;
            return mantissa >= .5m ? .01 * (wholePart + 1) : .01 * wholePart;
        }
        [DataMember]
        public double Amount { get; }

        [DataMember]
        public string Currency { get; }
        public int Cents { get; }


        public bool Equals(Money other)
        {
            return Currency == other.Currency && Cents == other.Cents;
        }

        public override bool Equals(object? obj)
        {
            return obj is Money other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Currency, Cents);
        }

        public static bool operator ==(Money left, Money right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Money left, Money right)
        {
            return !left.Equals(right);
        }

        public static Money operator +(Money left, Money right)
        {
            if (right.Cents == 0)
            {
                return left;
            }

            if (left.Cents == 0)
            {
                return right;
            }
            if (left.Currency != right.Currency)
            {
                throw new ArgumentException();
            }
            return new Money(left.Amount + right.Amount, left.Currency);
            // return !left.Equals(right);
        }

        public Money ChangePrice(double newMoney)
        {
            return new Money(newMoney, Currency);
        }

        public override string ToString()
        {
            return $"{nameof(Amount)}: {Amount}, {nameof(Currency)}: {Currency}";
        }
        //public Money Add(Money other)

        //{
        //    return new Money(this.Amount + other.Amount);
        //}

        //public Money MultiplyBy(double multiplicationFactor)

        //{

        //    return new Money(this.Amount * multiplicationFactor);

        //}

    }
}