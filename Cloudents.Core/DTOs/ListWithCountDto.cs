using System.Collections.Generic;

namespace Cloudents.Core.DTOs
{
    public class ListWithCountDto<T>
    {
        public IEnumerable<T> Result { get; set; }
        public long? Count { get; set; }
    }
}
