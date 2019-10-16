using Cloudents.Core.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cloudents.Core.Enum;
using Cloudents.Core.Exceptions;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nhibernate proxy")]
    public class User : BaseUser
    {
        public User(string email, string firstName, string lastName, Language language) : this()
        {
            Email = email;
            ChangeName(firstName, lastName);
            TwoFactorEnabled = true;
            Language = language;
            Created = DateTime.UtcNow;
        }

        public User(string email,  Language language) : this(email,null,null,language)
        {
          
        }

        protected User()
        {
            UserLogins = new List<UserLogin>();
            Transactions = Transactions ?? new UserTransactions();

        }

        public virtual string PhoneNumber { get; set; }

        public virtual bool PhoneNumberConfirmed { get; set; }
        public virtual string PasswordHash { get; set; }
       

        public virtual int AccessFailedCount { get; set; }

       

        // ReSharper disable once CollectionNeverUpdated.Local Nhiberate
        private readonly IList<Answer> _answers = new List<Answer>();

        public virtual IReadOnlyList<Answer> Answers => _answers.ToList();
        protected internal virtual IList<UserLogin> UserLogins { get; protected set; }

        //protected internal virtual ISet<UserCourse> UserCourses { get; protected set; }
        private readonly ISet<UserCourse> _userCourses = new HashSet<UserCourse>();



        public virtual IEnumerable<UserCourse> UserCourses => _userCourses.ToList();

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

        public virtual void RemoveCourse(Course course)
        {
            var p = new UserCourse(this,course);
            if (_userCourses.Remove(p))
            {
                course.Count--;
            }
            AddEvent(new RemoveCourseEvent(Id));
        }

        public virtual void CanTeachCourse(string courseName)
        {
            var course = _userCourses.AsQueryable().First(w => w.Course.Id == courseName);
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

        public virtual void BecomeTutor(string bio, decimal price,string description, string firstName, string lastName )
        {
            Tutor = new Tutor(bio, this, price);
            Description = description;
            ChangeName(firstName, lastName);
            foreach (var userCourse in _userCourses)
            {
                userCourse.CanTeach = true;
            }
        }

        private readonly ICollection<StudyRoomUser> _studyRooms = new List<StudyRoomUser>();

        public virtual IReadOnlyCollection<StudyRoomUser> StudyRooms => _studyRooms.ToList();


        public virtual DateTime LastOnline { get; protected set; }

        public override bool Online { get; protected set; }

        public virtual UserTransactions Transactions { get; protected set; }

       
        public virtual Tutor Tutor { get; set; }

        public virtual BuyerPayment BuyerPayment { get; protected set; }

        public virtual Gender Gender { get; protected set; }
        public virtual PaymentStatus PaymentExists { get; protected set; }

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

        public virtual void ChangeName(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
           
            Name = $"{FirstName} {LastName}".Trim();
            if (string.IsNullOrWhiteSpace(Name))
            {
                Name = $"{Email.Split(new[] { '.', '@' }, StringSplitOptions.RemoveEmptyEntries)[0]}";
            }
        }

        public virtual void SuspendUser(DateTimeOffset lockTime, string reason)
        {
            LockoutEnd = lockTime;
            LockoutReason = reason;
            AddEvent(new UserSuspendEvent(this));
        }

        public virtual bool IsSuspended()
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

        public virtual void UpdateImage(string image)
        {
            Image = image;
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

        public override int Score { get; protected set; } 
        public override decimal Balance => Transactions.Balance;


        public virtual void DeleteUserPayment()
        {
            BuyerPayment = null;
            PaymentExists = PaymentStatus.None;
        }
    }
}