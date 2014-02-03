using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.ViewModel.Queries
{
    public abstract class QueryPrefixBase : QueryPagedBase
    {
        protected QueryPrefixBase(string prefix, long userid, int pageNumber)
            : base(userid, pageNumber)
        {
            prefix = ChangePrefixForQuery(prefix);
        }

        protected QueryPrefixBase(string prefix, long userid)
            : this(prefix, userid, 0)
        {
        }

        private string ChangePrefixForQuery(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                prefix = string.Empty;
            }
            prefix = prefix.Replace("%", string.Empty);
            Prefix = prefix.Replace(' ', '%');
            return prefix;
        }
        public string Prefix { get; private set; }
    }

}
