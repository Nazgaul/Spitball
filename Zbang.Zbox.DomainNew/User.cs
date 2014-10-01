using System;
using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.IdGenerator;
using System.Text;

namespace Zbang.Zbox.Domain
{
    public class User
    {
        protected User()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            UserBoxRel = new HashSet<UserBoxRel>();

            Invites = new HashSet<Invite>();
            Quota = new Quota();
            UserTime = new UserTimeDetails("Sys");
            FirstTime = new UserFirstTime();

        }

        public User(string email, string smallImage, string largeImage)
            : this()
        {
            if (email == null) throw new ArgumentNullException("email");
            Name = Email = email.Trim();
            IsRegisterUser = false;
            Image = smallImage;
            ImageLarge = largeImage;
            Culture = System.Globalization.CultureInfo.CurrentCulture.Name;
        }

        public User(string email, string smallImage, string largeImage,
            string firstName, string middleName, string lastName, bool sex, bool marketEmail, string culture)
            : this()
        {
            if (firstName == null) throw new ArgumentNullException("firstName");
            if (lastName == null) throw new ArgumentNullException("lastName");
            Email = email;
            Image = smallImage;
            ImageLarge = largeImage;

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            if (!string.IsNullOrEmpty(middleName))
            {
                MiddleName = middleName.Trim();
            }

            CreateName();
            Sex = sex;
            MarketEmail = marketEmail;
            UpdateLanguage(culture);
        }
        // ReSharper restore DoNotCallOverridableMethodsInConstructor
        public void CreateName()
        {
            var sb = new StringBuilder();
            sb.Append(FirstName);
            sb.Append(" ");
            if (!string.IsNullOrEmpty(MiddleName))
            {
                sb.Append(MiddleName);
                sb.Append(" ");
            }
            sb.Append(LastName);
            //return sb.ToString();
            Name = sb.ToString();
            GenerateUrl();
        }

        public void GenerateUrl()
        {
            if (Id == 0)
            {
                return;
            }
            Url = UrlConsts.BuildUserUrl(Id, Name);
        }

        public virtual long Id { get; protected set; }

        public virtual string Email { get; set; }
        public virtual string Culture { get; private set; }

        public virtual bool IsRegisterUser { get; set; }
        public virtual string Name { get; protected set; }


        public virtual string FirstName { get; set; }
        public virtual string MiddleName { get; set; }
        public virtual string LastName { get; set; }
        public virtual bool Sex { get; set; }

        public virtual string Url { get; set; }

        public virtual Quota Quota { get; set; }
        public virtual UserTimeDetails UserTime { get; set; }
        public virtual ICollection<UserBoxRel> UserBoxRel { get; protected set; }
        protected virtual ICollection<Invite> Invites { get; set; }
        public virtual Guid? MembershipId { get; set; }
        public virtual long? FacebookId { get; set; }
        public virtual string Image { get; set; }
        public virtual string ImageLarge { get; set; }


        public virtual University University2 { get; protected set; }
        public virtual string Code { get; set; }
        public virtual DateTime? LastAccessTime { get; set; }

        public virtual UserFirstTime FirstTime { get; set; }
        public virtual int Reputation { get; set; }

        public virtual RussianDepartment RussianDepartment { get; set; }

        public virtual Department Department { get; set; }

        public virtual string GroupNumber { get; set; }
        public virtual string RegisterNumber { get; set; }

        public virtual bool MarketEmail { get; set; }


        public virtual string GetUniversityName()
        {
            return null;
        }

        public void RemoveInviteState(Box box)
        {
            var invites = Invites.Where(w => Equals(w.Box, box)).ToList();
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse - this is nhibernate issue
            if (invites != null)
            {
                invites.ForEach(f => f.IsActive = false);
            }
        }

        public Reputation AddReputation(ReputationAction action)
        {
            var reputation = new Reputation(IdGenerator.GetGuid(), this, action);

            Reputation += reputation.Score;
            return reputation;
        }

        public void ChangeUserRelationShipToBoxType(Box box, UserRelationshipType newUserType)
        {
            RemoveInviteState(box);
            var userType = UserBoxRel.FirstOrDefault(w => Equals(w.Box, box));
            if (userType == null)
            {
                userType = new UserBoxRel(this, box, UserRelationshipType.Subscribe);
                UserBoxRel.Add(userType);
                box.UserBoxRelationship.Add(userType);
                //throw new ArgumentException("user is not connected to box");
            }
            userType.UserRelationshipType = newUserType;
            userType.UserTime.UpdateUserTime(Email);
        }



        public IEnumerable<Box> GetUserOwnedBoxes()
        {
            return UserBoxRel.Where(w => w.UserRelationshipType == UserRelationshipType.Owner)
                .Select(s => s.Box).Where(w => w.IsDeleted == false);
        }



        public void UpdateUserProfile(string firstName, string middleName, string lastName, Uri picture, Uri largePicture)
        {
            if (firstName == null) throw new ArgumentNullException("firstName");
            if (lastName == null) throw new ArgumentNullException("lastName");
            // Name = userName.Trim();
            if (picture != null)
            {
                Image = picture.AbsoluteUri;
            }
            if (largePicture != null)
            {
                ImageLarge = largePicture.AbsoluteUri;
            }
            FirstName = firstName.Trim();
            MiddleName = !string.IsNullOrEmpty(middleName) ? middleName.Trim() : null;
            LastName = lastName.Trim();
            CreateName();
        }

        public void UpdateLanguage(string culture)
        {
            if (!Languages.CheckIfLanguageIsSupported(culture))
            {
                culture = System.Globalization.CultureInfo.CurrentCulture.Name;
            }
            Culture = culture;
        }

        public void UpdateUniversity(University university,
            string userCode, RussianDepartment department, string groupNumber,
            string registerNumber)
        {
            University2 = university;
            Code = userCode;
            RussianDepartment = department;
            GroupNumber = groupNumber;
            RegisterNumber = registerNumber;
            Department = null;
        }

        #region firstTime
        public void UpdateDashboardFirstTime()
        {
            FirstTime.DashboardFirstTimeShow();
        }
        public void UpdateLibraryFirstTime()
        {
            FirstTime.LibraryFirstTimeShow();
        }
        public void UpdateItemFirstTime()
        {
            FirstTime.ItemFirstTimeShow();
        }
        public void UpdateBoxFirstTime()
        {
            FirstTime.BoxFirstTimeShow();
        }
        #endregion

        #region Nhibernate
        public override bool Equals(object other)
        {
            if (this == other) return true;

            var user = other as User;
            if (user == null) return false;

            return String.Equals(Email.Trim(), user.Email.Trim(), StringComparison.CurrentCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = Email.GetHashCode();
                return result;
            }
        }
        #endregion

    }
}
