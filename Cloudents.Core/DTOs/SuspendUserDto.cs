using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Cloudents.Core.DTOs
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Dto")]
    public class SuspendUserDto
    {
        public IEnumerable<long> Questions { get; set; }
        public IEnumerable<Guid> Answers{ get; set; }
      
    }
}
