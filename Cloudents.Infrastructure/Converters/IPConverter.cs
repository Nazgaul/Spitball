﻿using AutoMapper;
using Cloudents.Core.DTOs;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure.Converters
{
    public class IpConverter :  ITypeConverter<string, IpDto>
    {
        public IpDto Convert(string source, IpDto destination, ResolutionContext context)
        {
            return JsonConvert.DeserializeObject<IpDto>(source,
                new JsonSerializerSettings()
                {
                    ContractResolver = new UnderscorePropertyNamesContractResolver()
                });
            //return new IpDto
            //{
            //    City = source["city"].Value<string>(),
            //    CountryCode = source["country_code"].Value<string>(),
            //    CountryName = source["country_name"].Value<string>()

            //}
            //throw new NotImplementedException();
        }
    }
}
