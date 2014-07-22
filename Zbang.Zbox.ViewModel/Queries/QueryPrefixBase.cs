namespace Zbang.Zbox.ViewModel.Queries
{
    public abstract class QueryPrefixBase : QueryPagedBase
    {
        protected QueryPrefixBase(string prefix, long userid, int pageNumber = 0)
            : base(userid, pageNumber)
        {
            ChangePrefixForQuery(prefix);
        }

        private void ChangePrefixForQuery(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                prefix = string.Empty;
            }
            prefix = prefix.Replace("%", string.Empty);
            Prefix = prefix.Replace(' ', '%');
           // return prefix;
        }
        public string Prefix { get; private set; }
    }

}
