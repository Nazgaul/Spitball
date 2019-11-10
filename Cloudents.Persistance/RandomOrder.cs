using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;

namespace Cloudents.Persistence
{
    public class RandomOrder : Order
    {
        public RandomOrder() : base(string.Empty, true)
        {

        }
        //public RandomOrder(IProjection projection, bool @ascending) : base(projection, @ascending)
        //{
        //}

        //public RandomOrder(string propertyName, bool @ascending) : base(propertyName, @ascending)
        //{
        //}

        public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
        {
            return SqlString.Parse("NewId()");
        }
    }
}