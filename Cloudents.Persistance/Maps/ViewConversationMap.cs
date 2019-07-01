using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class ViewConversationMap :ClassMap<ViewConversation>
    {
        public ViewConversationMap()
        {
            Id(x => x.Id);
            Map(x => x.LastMessage);
            Map(x => x.UserName);
            Map(x => x.UserPhoneNumber);
            Map(x => x.UserEmail);
            Map(x => x.UserId);
            Map(x => x.TutorName);
            Map(x => x.TutorPhoneNumber);
            Map(x => x.TutorEmail);
            Map(x => x.TutorId);
            Map(x => x.Status);
            Map(x => x.AssignTo);
            Map(x => x.RequestFor);
            Map(x => x.ConversationStatus);
            Map(x => x.StudyRoomExists);
            Map(x => x.HoursFromLastMessage);
            SchemaAction.Validate();
            Table("vAdminConversation");
            ReadOnly();
        }
    }
}