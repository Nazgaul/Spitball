using System;
using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.IdGenerator;
using System.Text;
using Zbang.Zbox.Infrastructure;

namespace Zbang.Zbox.Domain
{
    public class User //: EqualityAndHashCodeProvider<User,long>
    {
        protected User()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            UserBoxRel = new HashSet<UserBoxRel>();
            UserLibraryRel = new HashSet<UserLibraryRel>();
            //Invites = new HashSet<Invite>();
            Quota = new Quota();
            UserTime = new UserTimeDetails(0);
            UserType = UserType.Regular;
            
        }



       
        public User(string email, string image,
            string firstName, string lastName, string culture, Sex sex
            )
            : this()
        {
            if (firstName == null) throw new ArgumentNullException(nameof(firstName));
            if (lastName == null) throw new ArgumentNullException(nameof(lastName));
            Email = email;
            ImageLarge = image;

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            CreateName();
            UpdateLanguage(culture);
            Sex = sex;
            //AddReputation(ReputationAction.Register);
        }
        // ReSharper restore DoNotCallOverridableMethodsInConstructor
        public void CreateName()
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(FirstName))
            {
                FirstName = TextManipulation.SpaceReg.Replace(FirstName, " ");
                sb.Append(FirstName.Trim());
            }
            sb.Append(" ");
            if (!string.IsNullOrEmpty(LastName))
            {
                LastName = TextManipulation.SpaceReg.Replace(LastName, " ");
                sb.Append(LastName.Trim());
            }
           
            Name = sb.ToString().Trim();
            Name = TextManipulation.SpaceReg.Replace(Name, " ");
            GenerateUrl();
        }

        public void GenerateUrl()
        {
            if (Id == 0)
            {
                return;
            }
            Url = UrlConst.BuildUserUrl(Id, Name);
        }

        public virtual long Id { get; protected set; }

        public virtual string Email { get; protected set; }
        public virtual string Culture { get; private set; }

        public UserType UserType { get; set; }
        public virtual bool IsRegisterUser { get; set; }
        public virtual string Name { get; protected set; }
       // public virtual Theme Theme { get; set; }

        public virtual string FirstName { get; set; }
        //public virtual string MiddleName { get; set; }
        public virtual string LastName { get; set; }

        public virtual Sex Sex { get; set; }
        public virtual string Url { get; set; }

        public virtual Quota Quota { get; set; }
        public virtual UserTimeDetails UserTime { get; set; }
        public virtual ICollection<UserBoxRel> UserBoxRel { get; protected set; }
        public virtual ICollection<UserLibraryRel> UserLibraryRel { get; protected set; }
        public virtual ICollection<Connection> Connections { get; protected set; }
        public virtual Guid? MembershipId { get; set; }
        public virtual long? FacebookId { get; set; }
        public virtual string GoogleId { get; set; }

        public virtual string ImageLarge { get; set; }


        public virtual University University { get; protected set; }
        public virtual string StudentId { get; set; }
        public virtual DateTime? LastAccessTime { get; set; }

        public virtual int Reputation { get; set; }

        //public virtual RussianDepartment RussianDepartment { get; set; }


        //public virtual string GroupNumber { get; set; }
        //public virtual string RegisterNumber { get; set; }

        public virtual bool Online { get; set; }


        public virtual EmailSend EmailSendSettings { get; set; }
        

        public virtual MobileOperatingSystem MobileDevice { get; set; }
        public virtual PushNotificationSettings PushNotificationSetting { get; set; }

        

        //public Reputation AddReputation(ReputationAction action)
        //{
        //    var reputation = new Reputation(GuidIdGenerator.GetGuid(), this, action);

        //    Reputation += reputation.Score;
        //    return reputation;
        //}

        public virtual void AddConnection(string connectionId)
        {
            Connections.Add(new Connection(connectionId, this));
        }

        public void ChangeUserRelationShipToBoxType(Box box, UserRelationshipType newUserType)
        {
            var userType = UserBoxRel.FirstOrDefault(w => w.BoxId == box.Id);
            if (userType == null)
            {
                userType = new UserBoxRel(this, box, UserRelationshipType.Subscribe);
                UserBoxRel.Add(userType);
                box.UserBoxRelationship.Add(userType);
            }
            userType.UserRelationshipType = newUserType;
            userType.UserTime.UpdateUserTime(Id);
        }

       



        public IEnumerable<Box> GetUserOwnedBoxes()
        {
            return UserBoxRel.Where(w => w.UserRelationshipType == UserRelationshipType.Owner)
                .Select(s => s.Box).Where(w => w.IsDeleted == false);
        }



        public void UpdateUserProfile(string firstName, string lastName)
        {
            if (firstName == null) throw new ArgumentNullException(nameof(firstName));
            if (lastName == null) throw new ArgumentNullException(nameof(lastName));
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            CreateName();
        }

        public virtual void UpdateLanguage(string culture)
        {
            Culture = Languages.GetCultureBaseOnCulture(culture);
        }

        public void UpdateUniversity(University university,string studentId)
        {
            University = university;
            StudentId = studentId;
            University.UserTime.UpdateUserTime(Id);
            UserTime.UpdateUserTime(Id);
        }

        public virtual bool IsAdmin()
        {
            return UserType == UserType.TooHighScore ||
                   Reputation >= University.AdminScore;
        }

        public virtual void UpdateEmail(string email)
        {
            Email = email;
        }

       

        //#region Nhibernate
        //public override bool Equals(object other)
        //{
        //    if (this == other) return true;
        //
        //    var user = other as User;
        //    if (user == null) return false;
        //
        //    return string.Equals(Email.Trim(), user.Email.Trim(), StringComparison.CurrentCultureIgnoreCase);
        //}
        //
        //public override int GetHashCode()
        //{
        //    var result = Email.GetHashCode();
        //    return result;
        //}
        //#endregion

    }


}
