using Zbang.Zbox.Infrastructure.Query;
using NHibernate;

namespace Zbang.Zbox.Infrastructure.Data.Query
{
    public class NHibernateQueryAdapter : IQueryAdapter
    {
        private readonly IQuery m_Query;

        public NHibernateQueryAdapter(IQuery query)
        {
            m_Query = query;
        }

        public void SetNamedParameter(string name, object value)
        {
            m_Query.SetParameter(name, value);
        }
    }
}
