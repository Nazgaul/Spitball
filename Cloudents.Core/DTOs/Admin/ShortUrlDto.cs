using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.DTOs.Admin
{
    public class ShortUrlDto
    {
        public ShortUrlDto(string identifier, string destination, DateTime? expiration)
        {
            Identifier = identifier;
            Destination = destination;
            Expiration = expiration;
        }
        [EntityBind(nameof(ShortUrl.Identifier))]
        public string Identifier { get; set; }
        [EntityBind(nameof(ShortUrl.Destination))]
        public string Destination { get; set; }
        [EntityBind(nameof(ShortUrl.Expiration))]
        public DateTime? Expiration { get; set; }
    }
}
