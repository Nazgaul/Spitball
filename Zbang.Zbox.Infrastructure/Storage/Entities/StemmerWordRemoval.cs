using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Storage.Entities
{
    public class StemmerWordRemoval : TableEntity
    {
        public StemmerWordRemoval()
        {

        }

        public string Word { get; set; }


    }
}
