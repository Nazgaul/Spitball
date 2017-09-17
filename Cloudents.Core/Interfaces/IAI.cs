using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;

namespace Cloudents.Core.Interfaces
{
    public interface IAI
    {
        Task<AIDto> InterpetStringAsync(string sentence);
    }

    public interface IDesicions
    {
        (AIResult result, AIDto data) MakeDesicision(AIDto aiResult);
    }
}
