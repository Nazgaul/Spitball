using System;

using System.Globalization;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using Zbang.Zbox.Infrastructure.Enums;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.IdGenerator;

namespace Zbang.Zbox.Domain
{
    public class Box
    {
        public const int NameLength = 120;
        protected Box()
        {
            UserBoxRel = new Iesi.Collections.Generic.HashedSet<UserBoxRel>();
            Items = new List<Item>();
            //Comments = new List<Comment>();
            Questions = new List<Comment>();
            PrivacySettings = new PrivacySettings();
        }

        public Box(string boxName, User user, BoxPrivacySettings privacySettings)
            : this()
        {
            Name = boxName.Trim();
            PrivacySettings.PrivacySetting = privacySettings;
            UserTime = new UserTimeDetails(user.Email);
            Owner = user;
            UserBoxRel.Add(new UserBoxRel(user, this, UserRelationshipType.Owner));
            RemovePicture();
            this.CalculateMembers();
        }


        public virtual long Id { get; protected set; }
        public virtual string Name { get; protected set; }
        public virtual UserTimeDetails UserTime { get; set; }
        public virtual PrivacySettings PrivacySettings { get; set; }
        public virtual bool IsDeleted { get; set; }
        public virtual User Owner { get; private set; }
        public virtual string Picture { get; protected set; }

        public virtual string PictureUrl { get; set; }

        public virtual ICollection<UserBoxRel> UserBoxRel { get; private set; }
        public virtual ICollection<Item> Items { get; protected set; }
        public virtual ICollection<Quiz> Quizes { get; protected set; }

        public IQueryable<Item> Items2 { get; set; }


        //protected virtual ICollection<Comment> Comments { get; set; }
        protected virtual ICollection<Comment> Questions { get; set; }

        public virtual int MembersCount { get; private set; }
        public virtual int ItemCount { get; private set; }
        public virtual int CommentCount { get; protected set; }


        public void ChangeBoxName(string newBoxName)
        {
            Name = newBoxName.Trim();
        }

        public void AddPicture(string boxPicture, string picutreUrl)
        {
            Picture = boxPicture;
            PictureUrl = picutreUrl;
        }
        public void RemovePicture()
        {
            Picture = null;
            PictureUrl = "/images/emptyState/my_default3.png";
        }

        //public Comment AddBoxComment(User user, string commentText)
        //{
        //    var comment = new Comment(user, commentText, this, null);
        //    Comments.Add(comment);
        //    return comment;
        //}


        public File AddFile(string fileName, User user, long length, 
            string blobAddressName, string thumbnailBlobAddressName, string thumbnailUrl)
        {
            if (Items.OfType<File>().FirstOrDefault(f => f.ItemContentUrl == blobAddressName) != null)
            {
                throw new ArgumentException("Only one file can be connected to one blob");
            }
            var file = new File(GetUniqueFileName(fileName), user, length, blobAddressName, thumbnailBlobAddressName, this, thumbnailUrl);
            return AddItem(file) as File;
        }

        public string GetUniqueFileName(string fileName)
        {
            //Find exact macth
            var foundMatch = (from file in Items
                              where file.Name.ToLower() == fileName.ToLower() && !file.IsDeleted
                              select file).Count();
            if (foundMatch > 0)
            {
                var index = 0;
                //Find next available index
                do
                {
                    index++;
                    foundMatch = (from file in Items
                                  where file.Name.ToLower() == string.Format("{0}({1}){2}", Path.GetFileNameWithoutExtension(fileName), index, Path.GetExtension(fileName)).ToLower()
                                  select file).Count();
                } while (foundMatch > 0);
                fileName = string.Format("{0}({1}){2}", Path.GetFileNameWithoutExtension(fileName), index, Path.GetExtension(fileName));
            }

            return fileName;
        }

        public Item AddLink(string url, User user, int linkStorageSize, string linkTitle, string thumbnail, string thumbnailUrl)
        {
            var linkExist = Items.Any(s => s.ItemContentUrl == url && !s.IsDeleted);
            if (linkExist)
            {
                throw new DuplicateNameException("This link already exists in the box");
            }
            var link = new Link(url, user, linkStorageSize, this, linkTitle, thumbnail, thumbnailUrl);
            return AddItem(link);
        }

        private Item AddItem(Item item)
        {
            //Need to put in here unique item name check per box
            Items.Add(item);
            UpdateItemCount();
            return item;
        }

        public Item GetItem(long itemId)
        {
            var item = Items.SingleOrDefault(w => w.Id == itemId);
            return item;
        }


        #region dbiTemp
      
        //public void UpdateMembersDbi(int count)
        //{
        //    MembersCount = count;
        //}
        //public void UpdateItemDbi(int count)
        //{
        //    ItemCount = count;
        //}
        //public void UpdateCommentDbi(int count)
        //{
        //    CommentCount = count;
        //}

        
        #endregion


        #region membersCount
        public virtual void CalculateMembers()
        {
            MembersCount = UserBoxRel.Count();
        }

        #endregion
        #region itemCount

        public virtual void UpdateItemCount()
        {
            ItemCount = this.Items.Count(file => !file.IsDeleted) + this.Quizes.Count(quiz => quiz.Publish);

        }
        #endregion
        #region commentCount
        public virtual void UpdateQnACount(int number)
        {
            CommentCount = number;
        }

        #endregion

        #region Nhibernate
        public override bool Equals(object other)
        {
            if (this == other) return true;

            Box box = other as Box;
            if (box == null) return false;

            if (Name != box.Name) return false;
            if (UserTime.CreationTime != box.UserTime.CreationTime) return false;
            if (UserTime.CreatedUser != box.UserTime.CreatedUser) return false;
            if (IsDeleted != box.IsDeleted) return false;


            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result;
                result = 13 * Name.GetHashCode();
                result += 11 * UserTime.CreationTime.GetHashCode();
                result += 19 * UserTime.CreatedUser.GetHashCode();
                result += 17 * IsDeleted.GetHashCode();
                return result;
            }
        }
        #endregion




        public void ChangePrivacySettings(BoxPrivacySettings boxPrivacySettings, User user)
        {
            if (this is AcademicBox)
            {
                throw new ArgumentException("cannot change academic box privacy settings");
            }
            if (Owner != user)
            {
                throw new UnauthorizedAccessException("only owner can change privacy settings");
            }
            this.PrivacySettings.PrivacySetting = boxPrivacySettings;

        }
    }
}
