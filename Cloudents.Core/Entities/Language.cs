using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Cloudents.Core.Entities
{
    public sealed class Language
    {
        private Language(CultureInfo info)
        {
            Info = info;
        }

        public CultureInfo Info { get; private set; }

        public static readonly Language English = new Language(new CultureInfo("en"));
        public static readonly Language Hebrew = new Language(new CultureInfo("he"));
        public static readonly Language EnglishIndia = new Language(new CultureInfo("en-IN"));
        public static readonly Language EnglishIsrael = new Language(new CultureInfo("en-IL"));


        public static IEnumerable<Language> SystemSupportLanguage()
        {
            yield return English;
            yield return Hebrew;
            yield return EnglishIndia;
            yield return EnglishIsrael;

        }



        public static implicit operator CultureInfo(Language tb)
        {
            return tb.Info;
        }

        public override string ToString()
        {
            return $"{nameof(Info)}: {Info}";
        }


        public static implicit operator Language(CultureInfo info)
        {
            return AssignLanguage(info);
        }

        public static implicit operator Language(string tb)
        {
            var result = SystemSupportLanguage().FirstOrDefault(f => Equals(f.Info, new CultureInfo(tb)));
            if (result != null)
            {
                return result;
            }

            return English;
        }


        private static Language AssignLanguage(CultureInfo info)
        {
            if (info == null)
            {
                return English;
            }
            while (true)
            {
                if (Equals(info, info.Parent))
                {
                    break;
                }

                var result = SystemSupportLanguage().FirstOrDefault(f => Equals(f.Info, info));
                if (result == null)
                {
                    return AssignLanguage(info.Parent);
                }
                return result;
                //info = info.Parent;
            }

            return English;
        }
    }


    public sealed class Country : Enumeration
    {

        public RegionInfo RegionInfo { get; }
        public decimal ConversationRate { get; }

        /// <summary>
        /// Taken from https://www.twilio.com/blog/2017/12/introducing-gll-for-group-rooms.html
        /// </summary>
        //public string TwilioMediaRegion { get; }

        public override string ToString()
        {
            return RegionInfo.TwoLetterISORegionName;
        }


        private Country(string info, decimal conversationRate, int id) : base(id, info)
        {
            ConversationRate = conversationRate;
           // TwilioMediaRegion = twilioMediaRegion;
            RegionInfo = new RegionInfo(info.ToUpperInvariant());
        }

        public static implicit operator Country(string tb)
        {
            var result = FromDisplayName<Country>(tb);
            if (result is null)
            {
                return UnitedStates;
            }
            return result;
        }

        public static readonly Country Israel = new Country("IL", 1 / 25M, 1);
        public static readonly Country India = new Country("IN", 1, 2);
        public static readonly Country UnitedStates = new Country("US",1/100M,3);

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

    //public enum CountryGroup
    //{
    //    All,
    //    Tier3,
    //    Israel

    //}

}