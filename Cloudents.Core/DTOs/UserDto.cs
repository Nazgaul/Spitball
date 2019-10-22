using System;
using System.Globalization;
using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;

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
        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int Score { get; set; }
        public string UniversityName { get; set; }
        public string Description { get; set; }
        public bool Online { get; set; }
        public bool CalendarShared { get; set; }
        public UserTutorProfileDto Tutor { get; set; }

        //If the user is a tutor and then delete then the first name and the last name stays
        public bool ShouldSerializeTutor()
        {
            // don't serialize the Manager property if an employee is their own manager
            return (Tutor?.TutorCountry != null);
        }
    }

    public class UserTutorProfileDto
    {

        public const string TutorPriceVariableName = nameof(TutorPrice);
        public const string TutorCountryVariableName = nameof(TutorCountry);

        public string Price => TutorPrice.ToString("C0", CultureInfo.CurrentUICulture.ChangeCultureBaseOnCountry(TutorCountry));


       
        private decimal TutorPrice { get; set; }
       
        internal string TutorCountry { get; set; }

        public double Rate { get; set; }
        public int ReviewCount { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class UserAccountDto
    {
        [EntityBind(nameof(User.Balance))]

        public decimal Balance { get; set; }

        [EntityBind(nameof(User.Email))]
        public string Email { get; set; }
        [EntityBind(nameof(User.PhoneNumber))]
        public string PhoneNumber { get; set; }

        [EntityBind(nameof(User.University.Id))]
        public bool UniversityExists { get; set; }

        [EntityBind(nameof(User.Id))]
        public long Id { get; set; }
        [EntityBind(nameof(User.Name))]
        public string Name { get; set; }
        [EntityBind(nameof(User.Image))]
        public string Image { get; set; }
        [EntityBind(nameof(User.Score))]
        public int Score { get; set; }
        public ItemState? IsTutor { get; set; }

        [EntityBind(nameof(User.PaymentExists),nameof(User.Country))]
        public bool NeedPayment { get; set; }

        private string  Country { get; set; }
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
        [EntityBind(nameof(BaseUser.Image))]
        public string Image { get; set; }

        [EntityBind(nameof(ChatUser.Unread))]
        public int Unread { get; set; }

        [EntityBind(nameof(User.Online))]
        public bool Online { get; set; }

        [EntityBind(nameof(ChatRoom.Identifier))]
        public string ConversationId { get; set; }

        [EntityBind(nameof(ChatRoom.UpdateTime))]
        public DateTime DateTime { get; set; }

        [EntityBind(nameof(StudyRoom.Id))]

        public Guid? StudyRoomId { get; set; }

        public string LastMessage { get; set; }
    }
}