using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IDocumentDbRepository<T> where T : class
    {
        Task<T> GetItemAsync(string id);
    }
}
