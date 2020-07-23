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
        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="email">the user email</param>
        /// <param name="firstName">first name</param>
        /// <param name="lastName">last name - note google can sometime doesn't return</param>
        /// <param name="language">the user culture</param>
        /// <param name="country">hte user country</param>
        /// <param name="isTutor">if the user want to be tutor</param>
        public User(string email, string firstName, string? lastName,
            Language language, string country, bool isTutor = false)
        {
            if (firstName == null) throw new ArgumentNullException(nameof(firstName));
            //if (lastName == null) throw new ArgumentNullException(nameof(lastName));
            if (country == null) throw new ArgumentNullException(nameof(country));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            ChangeName(firstName, lastName);
            FirstName = firstName;
            LastName = lastName;
            Language = language;
            Created = DateTime.UtcNow;
            FinishRegistrationDate = DateTime.UtcNow;
            Country = country;
            SbCountry = Entities.Country.FromCountry(country);
            UserLogins = new List<UserLogin>();
            Transactions = new UserTransactions();
            if (isTutor)
            {
                Tutor = new Tutor(this);
            }
        }



        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Nhibernate proxy")]
        [SuppressMessage("ReSharper", "CS8618", Justification = "Nhibernate proxy")]
        protected User()
        {
        }

        public virtual string? PhoneNumber { get; set; }

        public virtual bool PhoneNumberConfirmed { get; set; }
        public virtual string? PasswordHash { get; set; }
        public virtual DateTimeOffset? LockoutEnd { get; set; }

        public virtual int AccessFailedCount { get; set; }

        public virtual bool LockoutEnabled { get; set; }


        public virtual string? LockoutReason { get; set; }


        //[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Mapping")]
        //protected internal virtual ICollection<Answer> Answers { get; protected set; }
        protected internal virtual ICollection<UserLogin> UserLogins { get; protected set; }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Mapping")]
        protected internal virtual ICollection<UserLocation> UserLocations { get; protected set; }


        [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
        private readonly ICollection<ChatUser> _chatUsers = new List<ChatUser>();

        public virtual IEnumerable<ChatUser> ChatUsers => _chatUsers;

        private readonly ISet<UserCourse> _userCourses = new HashSet<UserCourse>();

        public virtual IEnumerable<UserCourse> UserCourses => _userCourses.ToList();


        private readonly ISet<UserCoupon> _userCoupon = new HashSet<UserCoupon>();
        public virtual IEnumerable<UserCoupon> UserCoupon => _userCoupon;

        public virtual void AssignCourse(OldCourse course)
        {
            var p = new UserCourse(this, course);
            if (_userCourses.Add(p))
            {
                course.Count++;
                AddEvent(new CanTeachCourseEvent(p));
            }

        }

        public virtual void BecomeTutor()
        {
            Tutor = new Tutor(this);
        }




        public virtual void ApplyCoupon(Coupon coupon, Tutor tutor)
        {
            if (coupon.CanApplyCoupon())
            {
                var userCoupon = UserCoupon.SingleOrDefault(w => w.Tutor.Id == tutor.Id && w.IsNotUsed());
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
            if (Tutor != null)
            {
                if (Tutor.HasSubscription())
                {
                    throw new UnauthorizedAccessException("Cannot change country of tutor with subscription");
                }
            }
            PaymentExists = PaymentStatus.None;
            ChangeCountry(country);
            ChangeLanguage(Entities.Language.English);

        }


        //public virtual void RemoveCourse(Course course)
        //{
        //    var p = new UserCourse(this, course);
        //    if (_userCourses.Remove(p))
        //    {
        //        course.Count--;
        //    }
        //    AddEvent(new RemoveCourseEvent(Id));
        //}

        //public virtual void CanTeachCourse(string courseName)
        //{
        //    var course = UserCourses.AsQueryable().First(w => w.Course.Id == courseName);
        //    course.ToggleCanTeach();

        //}


        [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
        private readonly ICollection<StudyRoomUser> _studyRooms = new List<StudyRoomUser>();

        public virtual IEnumerable<StudyRoomUser> StudyRooms => _studyRooms;


        public virtual DateTime? LastOnline { get; protected set; }
        public virtual bool? Online { get; protected set; }

        public virtual UserTransactions Transactions { get; protected set; }

        public virtual string FirstName { get; protected set; }
        public virtual string? LastName { get; protected set; }
        //[Obsolete]
        //public virtual string Description { get; set; }

        public virtual string CoverImage { get; protected set; }
        public virtual Tutor? Tutor { get; protected set; }

        //[Obsolete]
        //public virtual BuyerPayment? BuyerPayment { get; protected set; }
        public virtual IPayment2? Payment { get; protected set; }

        //public virtual Gender? Gender { get; protected set; }
        public virtual PaymentStatus? PaymentExists { get; protected set; }

        public virtual DateTime? FinishRegistrationDate { get; set; }

        public virtual void CreditCardReceived()
        {
            PaymentExists = PaymentStatus.Done;
            AddEvent(new StudentPaymentReceivedEvent(this));
        }

        public virtual void AddPayment(IPayment2 payment)
        {
            PaymentExists = PaymentStatus.Done;
            Payment = payment;
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
            AddEvent(new UserChangeNameEvent(this));
        }

        public virtual void ChangeEmail(string email)
        {
            if (UserLogins.Count > 0)
            {
                throw new ArgumentException("User is registered though 3rd party - cant change email");
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

        public virtual void UpdateUserImage(string imageName)
        {
            //Image = image;
            ImageName = imageName;
            AddEvent(new UpdateImageEvent(Id));
        }

        public virtual void UpdateCoverImage(string image)
        {
            CoverImage = image;
        }

        //public override string ToString()
        //{
        //    return $"{nameof(Id)}: {Id}, " +
        //           $"{nameof(EmailConfirmed)}: {EmailConfirmed}," +
        //           $" {nameof(PhoneNumberConfirmed)}: {PhoneNumberConfirmed}" +
        //           $" {nameof(SbCountry)}: {SbCountry}";
        //}

        public override void MakeTransaction(Transaction transaction)
        {
            Transactions.Add(transaction, this);
            AddEvent(new TransactionEvent(transaction, this));

        }


        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Mapping")]

        protected internal virtual IEnumerable<Follow> Following { get; protected set; }
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Mapping")]
        protected internal virtual IEnumerable<Lead> Leads { get; protected set; }

        private readonly ISet<Follow> _followers = new HashSet<Follow>();
        public virtual IEnumerable<Follow> Followers => _followers;

        public override void AddFollower(User follower)
        {
            if (this.Id == follower.Id)
            {
                return;
            }

            if (Tutor == null)
            {
                return;
            }


            var follow = new Follow(this, follower);
            _followers.Add(follow);
        }

        public virtual void AddSubscriber(User follower)
        {
            if (this.Id == follower.Id)
            {
                return;
            }

            if (Tutor == null)
            {
                return;
            }

            //if (!Equals(follower))
            //{
            var existingFollow = _followers.FirstOrDefault(f => f.Follower.Id == follower.Id);
            if (existingFollow != null)
            {
                existingFollow.Subscriber = true;
                return;
            }
            var follow = new Follow(this, follower, true);
            _followers.Add(follow);
            AddEvent(new SubscribeToTutorEvent(this));
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

        public virtual void RemoveFollower(User follower)
        {

            var follow = new Follow(this, follower);
            _followers.Remove(follow);

        }


        public virtual void AwardMoney(decimal price)
        {
            var t = new AwardMoneyTransaction(price);
            MakeTransaction(t);
        }



        public virtual void CashOutMoney(/*decimal price*/)
        {
            var t = CashOutTransaction.CashOut();
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

        //public virtual void FinishRegistration()
        //{
        //    FinishRegistrationDate = DateTime.UtcNow;
        //    //MakeTransaction(AwardMoneyTransaction.FinishRegistration(this));
        //}

        public virtual void ConfirmPhoneNumber()
        {
            if (PhoneNumberConfirmed == false)
            {
                PhoneNumberConfirmed = true;
            }
        }

        // public override int Score { get; protected set; }  //=> Transactions.Score;
        public override decimal Balance => Transactions.Balance;

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Mapping")]
        protected internal virtual ICollection<UserDownloadDocument> DocumentDownloads { get; protected set; }
        protected internal virtual ICollection<StudyRoomPayment> SessionPayments { get; protected set; }
        protected internal virtual ICollection<StudyRoomSessionUser> StudyRoomSessionUsers { get; protected set; }

        public virtual void DeleteUserPayment()
        {
            //BuyerPayment = null;
            Payment = null;
            PaymentExists = PaymentStatus.None;
        }

        public virtual void Delete()
        {
            AddEvent(new DeleteUserEvent(this));
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