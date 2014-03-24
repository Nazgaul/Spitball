using System;
using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.IdGenerator;

namespace Zbang.Zbox.Domain
{
    public class User
    {


        protected User()
        {
            UserBoxRel = new Iesi.Collections.Generic.HashedSet<UserBoxRel>();
            Invites = new Iesi.Collections.Generic.HashedSet<Invite>();
            Quota = new Quota();
            UserTime = new UserTimeDetails("Sys");
            FirstTime = new UserFirstTime();
            Culture = System.Globalization.CultureInfo.CurrentCulture.Name;
        }

        //for university pupose only
        protected User(string email, string userName, string smallImage, string largeImage)
            : this()
        {
            Email = email;
            Name = userName.Trim();
            IsRegisterUser = false;
            Image = smallImage;
            ImageLarge = largeImage;
        }
        public User(string email, string smallImage, string largeImage)
        {
            Name  = Email = email.Trim();
            IsRegisterUser = false;
            Image = smallImage;
            ImageLarge = largeImage;
        }
        
        public User(string email, string smallImage, string largeImage, string firstName, string middleName, string lastName, bool sex)
            : this()
        {
            Email = email;
            IsRegisterUser = true;
            Image = smallImage;
            ImageLarge = largeImage;

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            MiddleName = middleName.Trim();

            Name = FirstName + " " + MiddleName + " " + LastName;
            Sex = sex;

        }

        public virtual long Id { get; protected set; }
        public virtual string Email { get; set; }
        public virtual string Culture { get; set; }
        public virtual string Country { get; set; }
        public virtual bool IsRegisterUser { get; set; }
        public virtual string Name { get; set; }


        public virtual string FirstName { get; set; }
        public virtual string MiddleName { get; set; }
        public virtual string LastName { get; set; }
        public virtual bool Sex { get; set; }

        public virtual Quota Quota { get; set; }
        public virtual UserTimeDetails UserTime { get; set; }
        public virtual ICollection<UserBoxRel> UserBoxRel { get; private set; }
        protected virtual ICollection<Invite> Invites { get; set; }
        public virtual Guid? MembershipId { get; set; }
        public virtual long? FacebookId { get; set; }
        public virtual string Image { get; set; }
        public virtual string ImageLarge { get; set; }
        public virtual University University { get; private set; }
        public virtual string Code { get; set; }
        public virtual DateTime? LastAccessTime { get; set; }

        public virtual UserFirstTime FirstTime { get; set; }
        public virtual int Reputation { get; set; }

        public virtual Department Department { get; set; }


        public void RemoveInviteState(Box box)
        {
            Invites.Where(w => w.Box == box).ToList().ForEach(f => f.IsActive = false);
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
            var userType = UserBoxRel.FirstOrDefault(w => w.Box == box);
            if (userType == null)
            {
                userType = new UserBoxRel(this, box, UserRelationshipType.Subscribe);
                this.UserBoxRel.Add(userType);
                box.UserBoxRel.Add(userType);
                //throw new ArgumentException("user is not connected to box");
            }
            userType.UserRelationshipType = newUserType;
            userType.UserTime.UpdateUserTime(this.Email);
        }



        public IEnumerable<Box> GetUserOwnedBoxes()
        {
            return UserBoxRel.Where(w => w.UserRelationshipType == UserRelationshipType.Owner).Select(s => s.Box).Where(w => w.IsDeleted == false);
        }



        public void UpdateUserProfile(string userName, Uri picture, Uri largePicture)
        {
            Name = userName.Trim();
            if (picture != null)
            {
                Image = picture.AbsoluteUri;
            }
            if (largePicture != null)
            {
                ImageLarge = largePicture.AbsoluteUri;
            }

        }
        public void UpdateUserLanguage(string culture)
        {
            if (!Languages.CheckIfLanguageIsSupported(culture))
            {
                throw new ArgumentException("Language is not supported");
            }
            Culture = culture;
        }

        public void UpdateUserUniversity(University university, string userCode, Department department)
        {
            University = university;
            this.Code = userCode;
            this.Department = department;

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

            return Email.Trim().ToLower() == user.Email.Trim().ToLower();
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
