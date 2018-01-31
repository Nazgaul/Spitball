using AutoMapper;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure.Converters
{
    public class IpConverter : ITypeConverter<string, Core.Models.Location>
    {
        public Core.Models.Location Convert(string source, Core.Models.Location destination, ResolutionContext context)
        {
            var ipDto =  JsonConvert.DeserializeObject<IpDto>(source,
                new JsonSerializerSettings
                {
                    ContractResolver = new UnderscorePropertyNamesContractResolver()
                });

            return context.Mapper.Map<Core.Models.Location>(ipDto);
        }
    }
}
