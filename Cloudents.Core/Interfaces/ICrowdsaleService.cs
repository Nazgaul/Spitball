using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface ICrowdsaleService: IBlockChainProvider
    {
        Task<string> BuyTokens(string senderPK, int amount, CancellationToken token);
    }
}
