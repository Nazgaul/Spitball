using Microsoft.WindowsAzure.Storage.Table;


namespace Zbang.Zbox.Infrastructure.Storage.Entities
{
    public class StemmerWordRemoval : TableEntity
    {

        public string Word { get; set; }


    }
}
