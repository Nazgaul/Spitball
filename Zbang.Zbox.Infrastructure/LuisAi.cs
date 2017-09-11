using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Cognitive.LUIS;

namespace Zbang.Zbox.Infrastructure
{
    public class LuisAi : IAi
    {
        //https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/a1a0245f-4cb3-42d6-8bb2-62b6cfe7d5a3?subscription-key=6effb3962e284a9ba73dfb57fa1cfe40&timezoneOffset=0&verbose=true&q=
        private readonly LuisClient m_Client = new LuisClient("a1a0245f-4cb3-42d6-8bb2-62b6cfe7d5a3", "6effb3962e284a9ba73dfb57fa1cfe40");
        public LuisAi()
        {

        }
        public async Task InterpetString(string sentence)
        {
            var result = await m_Client.Predict(sentence);

        }
    }

    public interface IAi
    {
        Task InterpetString(string sentence);
    }
}
