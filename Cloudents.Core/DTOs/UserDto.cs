using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Cloudents.Core.DTOs
{
    public class UserDto
    {
        public UserDto(long id, string name, int score, string image)
        {
            Id = id;
            Name = name;
            Score = score;
            Image = image;
        }

        // ReSharper disable once MemberCanBeProtected.Global need that for mark answer as correct.
        public UserDto()
        {

        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int Score { get; set; }
    }

    public class UserProfileDto
    {
        [EntityBind(nameof(User.Id))]
        public long Id { get; set; }
        [EntityBind(nameof(User.ImageName))]
        public string Name { get; set; }
        [EntityBind(nameof(User.ImageName))]
        public string Image { get; set; }
        public string UniversityName { get; set; }
        [EntityBind(nameof(User.Description))]
        public string Description { get; set; }
        [EntityBind(nameof(User.Online))]
        public bool Online { get; set; }
        [EntityBind(nameof(GoogleTokens))]
        public bool CalendarShared { get; set; }
        [EntityBind(nameof(User.Tutor))]
        public UserTutorProfileDto Tutor { get; set; }

        [EntityBind(nameof(User.FirstName))]
        public string FirstName { get; set; }
        [EntityBind(nameof(User.LastName))]
        public string LastName { get; set; }

        [EntityBind(nameof(User.Followers))]
        public int Followers { get; set; }

        public bool IsFollowing { get; set; }

        public IEnumerable<string> Courses { get; set; }

        //If the user is a tutor and then delete then the first name and the last name stays
        public bool ShouldSerializeTutor()
        {
            // don't serialize the Manager property if an employee is their own manager
            return Tutor?.TutorCountry != null;
        }
    }

    public class UserTutorProfileDto
    {
        public decimal Price { get; set; }

        public string Currency => new RegionInfo(TutorCountry).ISOCurrencySymbol;

        [EntityBind(nameof(ReadTutor.Country))]
        internal string TutorCountry { get; set; }

        public decimal? DiscountPrice { get; set; }

        [EntityBind(nameof(ReadTutor.Rate))]
        public double Rate { get; set; }
        [EntityBind(nameof(ReadTutor.RateCount))]
        public int ReviewCount { get; set; }
       

        public bool HasCoupon { get; set; }

        public decimal? CouponValue { get; set; }
        public CouponType? CouponType { get; set; }

        [EntityBind(nameof(ReadTutor.Bio))] 
        public string Bio { get; set; }

        [EntityBind(nameof(ReadTutor.AllSubjects))]
        public IEnumerable<string> Subjects { get; set; }

        [EntityBind(nameof(ReadTutor.Lessons))]
        public int Lessons { get; set; }

        public int ContentCount { get; set; }
        public int Students { get; set; }

        // public int ResponseTime { get; set; }

    }

    public class UserAccountDto
    {
        [EntityBind(nameof(User.Balance))]

        public decimal Balance { get; set; }

        [EntityBind(nameof(User.Email))]
        public string Email { get; set; }
        [EntityBind(nameof(User.PhoneNumber))]
        public string PhoneNumber { get; set; }
        [EntityBind(nameof(User.Id))]
        public long Id { get; set; }
        [EntityBind(nameof(User.Name))]
        public string Name { get; set; }
        [EntityBind(nameof(User.ImageName))]
        public string Image { get; set; }
        
        public ItemState? IsTutor { get; set; }

        [EntityBind(nameof(User.PaymentExists), nameof(User.Country))]
        public bool NeedPayment { get; set; }

        public bool HaveDocs { get; set; }
        public UserType? UserType { get; set; }
        public string Country { get; set; }

        public IEnumerable<CourseDto> Courses { get; set; }
        public UniversityDto University { get; set; }

        public string CurrencySymbol
        {
            get
            {
                var regionInfo = new RegionInfo(Country);
                return regionInfo.CurrencySymbol;
            }
        }
    }




    public class ChatUserDto
    {
        [EntityBind(nameof(BaseUser.Id))]
        public long UserId { get; set; }
        [EntityBind(nameof(BaseUser.Name))]
        public string Name { get; set; }
        [EntityBind(nameof(BaseUser.ImageName))]
        public string Image { get; set; }

        [EntityBind(nameof(ChatUser.Unread))]
        public int Unread { get; set; }

        [EntityBind(nameof(User.Online))]
        public bool Online { get; set; }

        [EntityBind(nameof(ChatRoom.Identifier))]
        public string ConversationId { get; set; }

        [EntityBind(nameof(ChatRoom.TimeStamp.UpdateTime))]
        public DateTime DateTime { get; set; }

        [EntityBind(nameof(StudyRoom.Id))]

        public Guid? StudyRoomId { get; set; }

        public string LastMessage { get; set; }
    }
}