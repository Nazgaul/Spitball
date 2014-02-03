using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Infrastructure.Culture
{
    public interface IHebrewStemmer
    {
        string StemAHebrewWord(string phrase);
    }
}
