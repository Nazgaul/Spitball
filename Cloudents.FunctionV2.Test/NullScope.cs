using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.FunctionsV2.Test
{
    public class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new NullScope();

        private NullScope() { }

        public void Dispose() { }
    }
}
