using NHibernate;

namespace Cloudents.Infrastructure.Data.Query
{
    public class ReadonlySession
    {
        public ISession Session { get; }

        public ReadonlySession(ISession session)
        {
            Session = session;
            Session.DefaultReadOnly = true;
            Session.FlushMode = FlushMode.Manual;
        }
    }
}