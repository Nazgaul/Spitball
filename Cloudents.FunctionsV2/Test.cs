using Microsoft.Azure.WebJobs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cloudents.FunctionsV2
{
    public static class Test
    {
        [FunctionName("Test"),Disable]
        public static async Task Run(
            [TimerTrigger("0 */1 * * * *", RunOnStartup = true)]TimerInfo timer,
           [Blob("spitball-files/files/1/text.txt")] string text, IDictionary<string, string> metadata
            //[BlobTrigger("spitball-files/files/{id}/{name}")] Stream myBlob,string id, string name
            )
        {


        }
    }
}
