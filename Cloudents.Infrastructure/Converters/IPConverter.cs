using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.DTOs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
