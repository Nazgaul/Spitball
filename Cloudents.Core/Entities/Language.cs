﻿using System.Collections.Generic;
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


        public static IEnumerable<Language> SystemSupportLanguage()
        {
            yield return English;// new CultureInfo(English.Id);
            yield return Hebrew;// new CultureInfo(Hebrew.Id);

        }



        public static implicit operator CultureInfo(Language tb)
        {
            return tb.Info;
        }

        //public static implicit operator Language(string tb)
        //{
        //    if (SystemSupportLanguage()
        //    {
        //        return tb;
        //    }

        //    return English;
        //}
        public static implicit operator Language(CultureInfo info)
        {
            var result = SystemSupportLanguage().FirstOrDefault(f => Equals(f.Info, info));
            if (result != null)
            {
                return result;
            }

            return English;
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
    }


    //public sealed class Country
    //{
    //    public RegionInfo RegionInfo { get; }

    //    public CountryGroup Tier { get; }

    //    private Country(string info, CountryGroup tier)
    //    {

    //        RegionInfo = new RegionInfo(info.ToUpperInvariant());
    //        Tier = tier;
    //    }

    //    public static Country Israel = new Country("IL", CountryGroup.Israel);
    //    public static Country Palestine = new Country("PS", CountryGroup.Israel);

    //    public static Country Afghanistan = new Country("AF", CountryGroup.Tier3);
    //    public static Country Bangladesh = new Country("BD", CountryGroup.Tier3);
    //    public static Country Bahrain = new Country("BH", CountryGroup.Tier3);
    //    public static Country BruneiDarussalam = new Country("BN", CountryGroup.Tier3);
    //    public static Country Bhutan = new Country("BT", CountryGroup.Tier3);
    //    public static Country Cocos = new Country("CC", CountryGroup.Tier3);
    //    public static Country ChristmasIsland = new Country("CX", CountryGroup.Tier3);
    //    public static Country Indonesia = new Country("ID", CountryGroup.Tier3);
    //    public static Country India = new Country("IN", CountryGroup.Tier3);
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
    //}

    //public enum CountryGroup
    //{
    //    All,
    //    Tier3,
    //    Israel

    //}

}