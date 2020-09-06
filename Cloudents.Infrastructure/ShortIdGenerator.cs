using Cloudents.Core.Interfaces;
using shortid;
using shortid.Configuration;

namespace Cloudents.Infrastructure
{
    public class ShortIdGenerator : IShortIdGenerator
    {
        public string GenerateShortId(int length)
        {
            var identifierChat = ShortId.Generate(new GenerationOptions
            {
                UseNumbers = true,
                UseSpecialCharacters = false,
                Length = length
            });
            return identifierChat;
        }
    }
}