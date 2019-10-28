using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class TutorInfoDto
    {
        [EntityBind(nameof(User.Id))]
        public long Id { get; set; }
        [EntityBind(nameof(User.Name))]
        public string Name { get; set; }
        [EntityBind(nameof(User.PhoneNumber))]
        public string PhoneNumber { get; set; }
        [EntityBind(nameof(User.Email))]
        public string Email { get; set; }
        public int TotalHours { get; set; }
        public int TotalStudents { get; set; }
        [EntityBind(nameof(Tutor.Price))]
        public decimal Price { get; set; }
        public bool NeedToPay { get; set; }
    }

    public class SessionBillDto
    {
        [EntityBind(nameof(User.Name))]
        public string Name { get; set; }
        [EntityBind(nameof(User.PhoneNumber))]
        public string PhoneNumber { get; set; }
        [EntityBind(nameof(User.Email))]
        public string Email { get; set; }
        [EntityBind(nameof(StudyRoomSession.Created))]
        public DateTime Created { get; set; }
        [EntityBind(nameof(StudyRoomSession.Ended))]
        public DateTime Ended { get; set; }
        public int Minutes { get; set; }
        public decimal Cost { get; set; }
        public bool IsPayed { get; set; }
    }
}
