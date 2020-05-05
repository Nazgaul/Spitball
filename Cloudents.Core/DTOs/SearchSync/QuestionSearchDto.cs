using System.Collections.Generic;

namespace Cloudents.Core.DTOs.SearchSync
{
    public class SearchWrapperDto<T>
    {
        public IEnumerable<T> Update { get; set; }
        public IEnumerable<string> Delete { get; set; }
        public long Version { get; set; }
    }
    
}