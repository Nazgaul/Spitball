using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Query.Stuff
{
    public interface IJsonArrayToEnumerableTransforme<TEntity> : IResultTransformer
    {
    }
}
