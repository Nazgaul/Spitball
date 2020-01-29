using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;

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
            //TwoFactorEnabled = true;
            Language = language;
            Created = DateTime.UtcNow;
            Country = country;
            Gender = gender;
        }

        //public User(string email, Language language) : this(email, null, null, language)
        //{

        //}

        protected User()
        {
            UserLogins = new List<UserLogin>();
            Transactions = Transactions ?? new UserTransactions();

        }

        public virtual string PhoneNumber { get; set; }

        public virtual bool PhoneNumberConfirmed { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual DateTimeOffset? LockoutEnd { get; set; }

        public virtual int AccessFailedCount { get; set; }

        public virtual bool LockoutEnabled { get; set; }

        // public virtual bool TwoFactorEnabled { get; set; }

        public virtual string LockoutReason { get; set; }

        // ReSharper disable once CollectionNeverUpdated.Local Nhiberate
        //private readonly IList<Answer> _answers = new List<Answer>();

        protected internal virtual ICollection<Answer> Answers { get; set; }
        protected internal virtual IList<UserLogin> UserLogins { get; protected set; }

        //protected internal virtual ISet<UserCourse> UserCourses { get; protected set; }



        //private readonly ISet<UserCourse> _userCourses = new HashSet<UserCourse>();
        private readonly ISet<UserCourse> _userCourses = new HashSet<UserCourse>();

        public virtual IEnumerable<UserCourse> UserCourses => _userCourses.ToList();


        private readonly ISet<UserCoupon> _userCoupon = new HashSet<UserCoupon>();
        public virtual IEnumerable<UserCoupon> UserCoupon => _userCoupon;

        public virtual void AssignCourses(IEnumerable<Course> courses)
        {
            var isTutor = Tutor != null;
            foreach (var course in courses)
            {

                var p = new UserCourse(this, course)
                {
                    CanTeach = isTutor
                };
                if (_userCourses.Add(p))
                {
                    course.Count++;
                }
            }
        }

        public virtual void UseCoupon(Tutor tutor)
        {
            var userCoupon = UserCoupon.SingleOrDefault(w => w.Tutor.Id == tutor.Id && w.UsedAmount < w.Coupon.AmountOfUsePerUser);
            if (userCoupon is null) // we do not check before if user have coupon on that user
            {
                return;
            }
            userCoupon.UsedAmount++;
            AddEvent(new UseCouponEvent(userCoupon));
        }

        public virtual void ApplyCoupon(Coupon coupon, Tutor tutor)
        {
            if (coupon.CanApplyCoupon())
            {
                var userCoupon = UserCoupon.SingleOrDefault(w => w.Tutor.Id == tutor.Id && w.UsedAmount < w.Coupon.AmountOfUsePerUser);
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
                //{
                //    _userCoupon.Remove(p);
                //    _userCoupon.Add(p);
                //}

            }
            //if (coupon.Expiration.GetValueOrDefault(DateTime.MaxValue) < DateTime.UtcNow)
            //{
            //    throw new ArgumentException("invalid coupon");
            //}

            //if (AmountOfUsers.HasValue && AmountOfUsers.Value <= _userCoupon.Count)
            //{
            //    throw new OverflowException();
            //}
            
        }

        public virtual void ChangeCountry(string country)
        {

            if (Country?.Equals(country) == true)
            {
                return;
            }
            Country = country;
            University = null;
            AddEvent(new ChangeCountryEvent(Id));
        }


        public virtual void ChangeCountryAdmin(string country)
        {

            if (Country?.Equals(country) == true)
            {
                return;
            }
            Country = country;
            University = null;
            ChangeLanguage(Entities.Language.English);
            AddEvent(new ChangeCountryEvent(Id));
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
            course.CanTeach = !course.CanTeach;
            LastOnline = DateTime.UtcNow; // this is for trigger the event
            AddEvent(new CanTeachCourseEvent(course));
        }

        public virtual void SetUniversity(University university)
        {
            University = university;
            University.UsersCount++;
            AddEvent(new SetUniversityEvent(Id));
        }

        public virtual void BecomeTutor(string bio, decimal? price, string description, string firstName, string lastName)
        {

            Tutor = new Tutor(bio, this, price);
            Description = description;
            ChangeName(firstName, lastName);
            foreach (var userCourse in UserCourses)
            {
                userCourse.CanTeach = true;
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
        public virtual Tutor Tutor { get; set; }

        public virtual BuyerPayment BuyerPayment { get; protected set; }

        public virtual Gender Gender { get; protected set; }
        public virtual PaymentStatus PaymentExists { get; protected set; }

        public virtual UserType? UserType { get; set; }
        public virtual string ChildFirstName { get; set; }
        public virtual string ChildLastName { get; set; }
        public virtual short Grade { get; set; }
        public virtual void CreditCardReceived()
        {
            PaymentExists = PaymentStatus.Done;
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

        public virtual void DeleteFirstAndLastName()
        {
            FirstName = null;
            LastName = null;
        }

        public virtual void ChangeName(string firstName, [CanBeNull] string lastName)
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

        public virtual void UpdateUserBalance(decimal balance, int score)
        {
            Transactions.UpdateBalance(balance, score);
        }

        public virtual void UpdateUserImage(string image, string imageName)
        {
            Image = image;
            ImageName = imageName;
            AddEvent(new UpdateImageEvent(Id));
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
            MakeTransaction(new ReferUserTransaction(user));
        }

        public virtual void FinishRegistration()
        {
            MakeTransaction(AwardMoneyTransaction.FinishRegistration(this));
        }

        public virtual void ConfirmPhoneNumber()
        {
            if (PhoneNumberConfirmed == false)
            {
                PhoneNumberConfirmed = true;
            }
        }

        public override int Score { get; protected set; }  //=> Transactions.Score;
        public override decimal Balance => Transactions.Balance;


        public virtual void DeleteUserPayment()
        {
            BuyerPayment = null;
            PaymentExists = PaymentStatus.None;
        }

        public virtual void SetUserType(UserType userType)
        {
            UserType = userType;
        }

        public virtual void SetUserGrade(short grade)
        {
            Grade = grade;
        }

        public virtual void SetChildName(string firstName, string lastName)
        {
            ChildFirstName = firstName;
            ChildLastName = lastName;
        }

    }
}