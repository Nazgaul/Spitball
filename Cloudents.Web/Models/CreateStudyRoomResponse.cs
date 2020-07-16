using System;

namespace Cloudents.Web.Models
{
    public class CreateStudyRoomResponse
    {
        public CreateStudyRoomResponse(Guid studyRoomId, string identifier)
        {
            StudyRoomId = studyRoomId;
            Identifier = identifier;
        }

        public Guid StudyRoomId { get; }

        public string Identifier { get; }

    }
}