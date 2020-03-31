using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class SessionParticipantDisconnectMap : ClassMap<SessionParticipantDisconnect>
    {
        public SessionParticipantDisconnectMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(r => r.StudyRoomSession).Column("SessionId").ForeignKey("Session_Disconnect")
                .Not.Nullable().UniqueKey("Session_Disconnect");
        }
    }
}
