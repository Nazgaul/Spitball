using System;
using System.Linq;
using System.Data;
using System.IO;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain
{
    public class Box : ISoftDeletable
    {
        public const int NameLength = 120;
        protected Box()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            UserBoxRelationship = new HashSet<UserBoxRel>();
            Items = new List<Item>();

            //Comments = new List<Comment>();
            Comments = new List<Comment>();
            PrivacySettings = new PrivacySettings();
        }

        public Box(string boxName, User user, BoxPrivacySettings privacySettings)
            : this()
        {
            if (boxName == null)
            {
                throw new ArgumentNullException("boxName");
            }
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            Name = boxName.Trim();
            PrivacySettings.PrivacySetting = privacySettings;
            UserTime = new UserTimeDetails(user.Email);
            Owner = user;
            UserBoxRelationship.Add(new UserBoxRel(user, this, UserRelationshipType.Owner));
            RemovePicture();
            CalculateMembers();
        }
        // ReSharper restore DoNotCallOverridableMethodsInConstructor

        public virtual long Id { get; protected set; }
        public virtual string Name { get; protected set; }
        public virtual UserTimeDetails UserTime { get; set; }
        public virtual PrivacySettings PrivacySettings { get; set; }
        public virtual bool IsDeleted { get; set; }
        public virtual User Owner { get; private set; }
        public virtual string Picture { get; protected set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public virtual string PictureUrl { get; private set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public virtual string Url { get; protected set; }

        public virtual ICollection<UserBoxRel> UserBoxRelationship { get; protected set; }
        public virtual ICollection<Item> Items { get; private set; }
        protected virtual ICollection<Quiz> Quizzes { get; set; }

        // public IQueryable<Item> Items2 { get; set; }


        //protected virtual ICollection<Comment> Comments { get; set; }
        protected virtual ICollection<Comment> Comments { get;  set; }

        protected virtual ICollection<ItemTab> ItemTabs { get; set; }

        protected virtual ICollection<Updates> Updates { get; set; }
        protected virtual ICollection<InviteToBox> Invites { get; set; }

        public virtual int MembersCount { get; private set; }
        public virtual int ItemCount { get; private set; }
        public virtual int QuizCount { get; private set; }

        public virtual int CommentCount { get; protected set; }


        public void ChangeBoxName(string newBoxName)
        {
            if (newBoxName == null)
            {
                throw new ArgumentNullException("newBoxName");
            }
            Name = newBoxName.Trim();
            GenerateUrl();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "Db call will save as string")]
        public void AddPicture(string boxPicture, string pictureUrl)
        {
            Picture = boxPicture;
            PictureUrl = pictureUrl;
        }
        public void RemovePicture()
        {
            Picture = null;
            PictureUrl = "/images/emptyState/my_default3.png";
        }

        public virtual void GenerateUrl()
        {
            if (Id == 0)
            {
                return;
            }
            Url = UrlConsts.BuildBoxUrl(Id, Name, string.Empty);
        }



        public File AddFile(string fileName,
            User user,
            long length,
            string blobAddressName,
            string thumbnailBlobAddressName,
            string thumbnailUrl)
        {
            var file = new File(fileName, user, length, blobAddressName, thumbnailBlobAddressName, this, thumbnailUrl);
            return AddItem(file) as File;
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

        public virtual Comment AddComment(User user, string text, Guid id, IList<Item> items, bool isSystemGenerated)
        {
            var comment = new Comment(user, text, this, id, items, isSystemGenerated);
            Comments.Add(comment);
            UpdateCommentsCount();
            return comment;
        }

        public virtual List<Reputation> DeleteComment(Comment comment)
        {
            var reputation = comment.AnswersReadOnly.Select(answer => answer.User.AddReputation(ReputationAction.DeleteAnswer)).ToList();
            reputation.Add(comment.User.AddReputation
                (ReputationAction.DeleteQuestion));
            Comments.Remove(comment);
            UpdateCommentsCount();
            return reputation;
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
            MembersCount = UserBoxRelationship.Count();
        }

        #endregion
        #region itemCount

        public virtual void UpdateItemCount()
        {
            ItemCount = Items.Count(file => !file.IsDeleted);
            QuizCount = Quizzes.Count(quiz => quiz.Publish);

        }
        #endregion
        #region commentCount
       
        public virtual void UpdateCommentsCount()
        {
            CommentCount = Comments.Count;
        }

        #endregion

        #region Nhibernate
        public override bool Equals(object obj)
        {
            if (this == obj) return true;

            var box = obj as Box;
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
                int result = 13 * Name.GetHashCode();
                result += 11 * UserTime.CreationTime.GetHashCode();
                result += 19 * UserTime.CreatedUser.GetHashCode();
                result += 17 * IsDeleted.GetHashCode();
                return result;
            }
        }
        #endregion




        public void ChangePrivacySettings(BoxPrivacySettings boxPrivacySettings, User user)
        {
            if (PrivacySettings.PrivacySetting == boxPrivacySettings)
            {
                return;
            }
            if (this is AcademicBox)
            {
                throw new ArgumentException("cannot change academic box privacy settings");
            }
            if (!Equals(Owner, user))
            {
                throw new UnauthorizedAccessException("only owner can change privacy settings");
            }

            PrivacySettings.PrivacySetting = boxPrivacySettings;

        }
    }
}
