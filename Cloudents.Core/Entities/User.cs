using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nhibernate proxy")]
    public class User : BaseUser
    {
        public User(string email, string firstName, string lastName,
            Language language, string country, Gender gender = Gender.None) : this()
        {
            Email = email;
            ChangeName(firstName, lastName);
            FirstName = firstName;
            LastName = lastName;
            Language = language;
            Created = DateTime.UtcNow;
            Country = country;
            SbCountry = Entities.Country.FromCountry(country);
            Gender = gender;
        }



        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Nhibernate proxy")]
        [SuppressMessage("ReSharper", "CS8618", Justification = "Nhibernate proxy")]
        protected User()
        {
            UserLogins = new List<UserLogin>();
            Transactions ??= new UserTransactions();

        }

        public virtual string PhoneNumber { get; set; }

        public virtual bool PhoneNumberConfirmed { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual DateTimeOffset? LockoutEnd { get; set; }

        public virtual int AccessFailedCount { get; set; }

        public virtual bool LockoutEnabled { get; set; }


        public virtual string LockoutReason { get; set; }


        protected internal virtual ICollection<Answer> Answers { get; protected set; }
        protected internal virtual ICollection<UserLogin> UserLogins { get; protected set; }

        protected internal virtual ICollection<UserLocation> UserLocations { get; protected set; }


        private readonly ICollection<ChatUser> _chatUsers = new List<ChatUser>();

        public virtual IEnumerable<ChatUser> ChatUsers => _chatUsers;

        private readonly ISet<UserCourse> _userCourses = new HashSet<UserCourse>();

        public virtual IEnumerable<UserCourse> UserCourses => _userCourses.ToList();


        private readonly ISet<UserCoupon> _userCoupon = new HashSet<UserCoupon>();
        public virtual IEnumerable<UserCoupon> UserCoupon => _userCoupon;

        public virtual void AssignCourses(IEnumerable<Course> courses)
        {
            foreach (var course in courses)
            {
                var p = new UserCourse(this, course);
                if (_userCourses.Add(p))
                {
                    course.Count++;
                }
            }
        }

        //public virtual void UseCoupon(Tutor tutor)
        //{
        //    var userCoupon = UserCoupon.SingleOrDefault(w => w.Tutor.Id == tutor.Id 
        //                                            && w.UsedAmount < w.Coupon.AmountOfUsePerUser);
        //    if (userCoupon is null) // we do not check before if user have coupon on that user
        //    {
        //        return;
        //    }
        //    userCoupon.UsedAmount++;
        //    AddEvent(new UseCouponEvent(userCoupon));
        //}


        public virtual void ApplyCoupon(Coupon coupon, Tutor tutor)
        {
            if (coupon.CanApplyCoupon())
            {
                var userCoupon = UserCoupon.SingleOrDefault(w => w.Tutor.Id == tutor.Id && w.IsUsed());
                if (userCoupon != null)
                {
                    throw new DuplicateRowException();
                }
                var p = new UserCoupon(this, coupon, tutor);
                if (!_userCoupon.Add(p))
                {
                    throw new DuplicateRowException();
                }
                AddEvent(new ApplyCouponEvent(p));
               

            }

        }

        public virtual void ChangeCountry(string country)
        {
            if (Entities.Country.CountriesNotSupported.Contains(country))
            {
                throw new NotSupportedException();
            }
            if (Country?.Equals(country) == true)
            {
                return;
            }
            Country = country;

            SbCountry = Entities.Country.FromCountry(country);
           

            AddEvent(new ChangeCountryEvent(Id));
        }


        public virtual void ChangeCountryAdmin(string country)
        {
            ChangeCountry(country);
            ChangeLanguage(Entities.Language.English);
          
        }


        public virtual void RemoveCourse(Course course)
        {
            var p = new UserCourse(this, course);
            if (_userCourses.Remove(p))
            {
                course.Count--;
            }
            AddEvent(new RemoveCourseEvent(Id));
        }

        public virtual void CanTeachCourse(string courseName)
        {
            var course = UserCourses.AsQueryable().First(w => w.Course.Id == courseName);
            course.ToggleCanTeach();
          
        }



        public virtual void BecomeTutor(string bio, decimal? price, string description, string firstName, string lastName)
        {

            Tutor = new Tutor(bio, this, price);
            Description = description;
            //SetUserType(UserType.Teacher);
            ChangeName(firstName, lastName);
            foreach (var userCourse in UserCourses)
            {
                userCourse.CanTeach();
            }
        }




        private readonly ICollection<StudyRoomUser> _studyRooms = new List<StudyRoomUser>();

        public virtual IEnumerable<StudyRoomUser> StudyRooms => _studyRooms;


        public virtual DateTime? LastOnline { get; protected set; }
        public virtual bool Online { get; protected set; }

        public virtual UserTransactions Transactions { get; protected set; }

        public virtual string FirstName { get; protected set; }
        public virtual string LastName { get; protected set; }
        public virtual string Description { get; set; }

        public virtual string CoverImage { get; protected set; }
        public virtual Tutor? Tutor { get; protected set; }

        public virtual BuyerPayment? BuyerPayment { get; protected set; }

        public virtual Gender Gender { get; protected set; }
        public virtual PaymentStatus PaymentExists { get; protected set; }

        // public virtual UserType? UserType2 { get; protected set; }
        private readonly ICollection<UserPaymentToken> _userTokens = new List<UserPaymentToken>();
        public virtual IEnumerable<UserPaymentToken> UserTokens => _userTokens;


        public virtual DateTime? FinishRegistrationDate { get; set; }


        public virtual void CreditCardReceived()
        {
            PaymentExists = PaymentStatus.Done;
            AddEvent(new StudentPaymentReceivedEvent(this));
        }

        //public virtual void UseToken(StudyRoom studyRoom)
        //{


        //    if (SbCountry != Entities.Country.UnitedStates)
        //    {
        //        return;
        //    }

        //    var userToken = UserTokens
        //        .FirstOrDefault(w => w.State == PaymentTokenState.NotUsed && w.StudyRoom.Id == studyRoom.Id);
        //    if (userToken != null)
        //    {
        //        userToken.ChangeToUsedState();
        //    }

        //    UseCoupon(studyRoom.Tutor);
        //}

        public virtual void AddPaymentToken(string orderId, string authorizationId, decimal amount, StudyRoom studyRoom)
        {
            if (orderId == null) throw new ArgumentNullException(nameof(orderId));
            if (authorizationId == null) throw new ArgumentNullException(nameof(authorizationId));
            if (studyRoom == null) throw new ArgumentNullException(nameof(studyRoom));

            if (SbCountry != Entities.Country.UnitedStates)
            {
                throw new ArgumentException("Only usa country can use paypal");
            }
            _userTokens.Add(new UserPaymentToken(orderId, authorizationId, amount, studyRoom));
            AddEvent(new StudentPaymentReceivedEvent(this));
        }

        public virtual void AddPayment(string token, DateTime expiration, string buyerCardMask)
        {
            PaymentExists = PaymentStatus.Done;
            BuyerPayment = new BuyerPayment(token, expiration, buyerCardMask);
        }



        public virtual void ChangeOnlineStatus(bool isOnline)
        {
            if (IsSuspended())
            {
                throw new UserLockoutException();
            }
            Online = isOnline;
            LastOnline = DateTime.UtcNow;
        }



        public virtual void ChangeName(string firstName, string? lastName)
        {
            FirstName = firstName;
            LastName = lastName;

            Name = $"{FirstName} {LastName}".Trim();
            if (string.IsNullOrWhiteSpace(Name))
            {
                Name = $"{Email.Split(new[] { '.', '@' }, StringSplitOptions.RemoveEmptyEntries)[0]}";
            }
        }

        public virtual void ChangeEmail(string email)
        {
            if (UserLogins.Count > 0)
            {
                throw new ArgumentException();
            }
            Email = email;
        }

        public virtual void SuspendUser(DateTimeOffset lockTime, string reason)
        {
            LockoutEnd = lockTime;
            LockoutReason = reason;
            AddEvent(new UserSuspendEvent(this));
        }

        protected virtual bool IsSuspended()
        {
            if (LockoutEnd.HasValue && LockoutEnd.Value > DateTimeOffset.UtcNow)
            {
                return true;
            }

            return false;
        }

        public virtual void UnSuspendUser()
        {
            LockoutEnd = DateTime.UtcNow.Add(TimeSpan.FromSeconds(-1));
            AddEvent(new UserUnSuspendEvent(this));
        }

        public virtual void UpdateUserBalance()
        {
            var balance = Transactions.Balance;
            Transactions.UpdateBalance(balance);
        }

        public virtual void UpdateUserImage(string image, string imageName)
        {
            Image = image;
            ImageName = imageName;
            AddEvent(new UpdateImageEvent(Id));
        }

        public virtual void UpdateCoverImage(string image)
        {
            CoverImage = image;
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(EmailConfirmed)}: {EmailConfirmed}, {nameof(PhoneNumberConfirmed)}: {PhoneNumberConfirmed}";
        }

        public override void MakeTransaction(Transaction transaction)
        {
            Transactions.Add(transaction, this);
            AddEvent(new TransactionEvent(transaction, this));

        }


        protected internal virtual IEnumerable<Follow> Followed { get; set; }
        protected internal virtual IEnumerable<Lead> Leads { get; set; }

        private readonly ISet<Follow> _followers = new HashSet<Follow>();
        public virtual IEnumerable<Follow> Followers => _followers;

        public override void AddFollower(User follower)
        {
            if (this == follower)
            {
                return;
            }

            if (Tutor == null)
            {
                return;
            }

            //if (!Equals(follower))
            //{
            var follow = new Follow(this, follower);
            _followers.Add(follow);
        }

        public virtual void AddFollowers(IEnumerable<User> followers)
        {
            foreach (var follower in followers)
            {
                AddFollower(follower);
            }
            //if (!Equals(follower))
            //{

        }

        public override void RemoveFollower(BaseUser follower)
        {
            if (follower is User u)
            {
                var follow = new Follow(this, u);
                _followers.Remove(follow);
            }
        }


        public virtual void AwardMoney(decimal price)
        {
            var t = new AwardMoneyTransaction(price);
            MakeTransaction(t);
        }



        public virtual void CashOutMoney(/*decimal price*/)
        {
            var t = new CashOutTransaction();
            MakeTransaction(t);
        }

        public virtual void ReferUser(User user)
        {
            const int maxRefer = 5;

            //Country country = user.Country;

            var referCount = Transactions.TransactionsReadOnly.AsQueryable().Count(w => w is ReferUserTransaction);
            var price = referCount > maxRefer || SbCountry == Entities.Country.India ? 0 : 10;
            MakeTransaction(new ReferUserTransaction(user, price));
        }

        public virtual void FinishRegistration()
        {
            FinishRegistrationDate = DateTime.UtcNow;
            //MakeTransaction(AwardMoneyTransaction.FinishRegistration(this));
        }

        public virtual void ConfirmPhoneNumber()
        {
            if (PhoneNumberConfirmed == false)
            {
                PhoneNumberConfirmed = true;
            }
        }

        // public override int Score { get; protected set; }  //=> Transactions.Score;
        public override decimal Balance => Transactions.Balance;


        public virtual void DeleteUserPayment()
        {
            BuyerPayment = null;
            PaymentExists = PaymentStatus.None;
        }

        //public virtual void SetUserType(UserType userType)
        //{

        //    //(userType,Tutor) switch
        //    //{
        //    //    (UserType.UniversityStudent, _) => Extend = new CollegeStudent(this),
        //    //    (UserType.HighSchoolStudent, _) => Extend = new HighSchoolStudent(this),
        //    //    (UserType.Parent, _) => Extend = new Parent(this),
        //    //    (UserType.Teacher, null) => {Extend = new CollegeStudent(this)}
        //    //    (UserType.Teacher, _) => Extend = new Teacher(this);,
        //    //}

        //    switch (userType)
        //    {
        //        case UserType.UniversityStudent:
        //            Extend = new CollegeStudent(this);
        //            break;
        //        case UserType.HighSchoolStudent:
        //            Extend = new HighSchoolStudent(this);

        //            break;
        //        case UserType.Parent:
        //            Extend = new Parent(this);

        //            break;
        //        case UserType.Teacher:
        //            if (Tutor == null)
        //            {
        //                Extend = new CollegeStudent(this);
        //                UserType2 = UserType.HighSchoolStudent;
        //                return;
        //            }
        //            else
        //            {
        //                Extend = new Teacher(this);
        //            }

        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException(nameof(userType), userType, null);
        //    }

        //    UserType2 = userType;
        //}

        //[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Nhibernate")]
        //protected internal virtual ICollection<UserComponent> UserComponents { get; set; }

        //public virtual UserComponent Extend
        //{
        //    get => UserComponents.SingleOrDefault();
        //    set
        //    {
        //        UserComponents.Clear();
        //        UserComponents.Add(value);

        //    }
        //}
    }

    //public class Parent : UserComponent
    //{
    //    public Parent(User user)
    //        : base(UserType.Parent, user)
    //    {

    //    }
    //    public Parent(User user, string name, short grade)
    //        : base(UserType.Parent, user)
    //    {
    //        Name = name;
    //        Grade = grade;
    //    }

    //    protected Parent()
    //    {

    //    }
    //    public virtual string Name { get; protected set; }
    //    public virtual short Grade { get; protected set; }




    //    public virtual void SetChildData(string name, short grade)
    //    {
    //        Name = name;
    //        Grade = grade;
    //    }

    //    //public override UserType Type { get; protected set; }
    //}


    //public class HighSchoolStudent : UserComponent
    //{
    //    public HighSchoolStudent(User user) : base(UserType.HighSchoolStudent, user)
    //    {

    //    }

    //    protected HighSchoolStudent()
    //    {

    //    }
    //    public override UserType Type { get; protected set; }

    //    public virtual void SetGrade(short grade)
    //    {
    //        Grade = grade;
    //    }

    //    public virtual short Grade { get; protected set; }
    //}

    //public class CollegeStudent : UserComponent
    //{
    //    public CollegeStudent(User user) : base(UserType.UniversityStudent, user)
    //    {
    //    }

    //    protected CollegeStudent()
    //    {

    //    }
    //    public override UserType Type { get; protected set; }
    //}

    //public class Teacher : UserComponent
    //{
    //    public Teacher(User user) : base(UserType.Teacher, user)
    //    {
    //    }

    //    protected Teacher()
    //    {

    //    }
    //    public override UserType Type { get; protected set; }
    //}


}