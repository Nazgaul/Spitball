using System;
using System.Linq;
using System.Data;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using System.Collections.Generic;
using Zbang.Zbox.Domain.Resources;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain
{
    public abstract class Box : IDirty
    {
        public const int NameLength = 120;
        protected Box()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            UserBoxRelationship = new HashSet<UserBoxRel>();
            Items = new List<Item>();
            Quizzes = new List<Quiz>();
            Comments = new List<Comment>();
            PrivacySettings = new PrivacySettings();
            ItemTabs = new List<ItemTab>();
            ShouldMakeDirty = () => false;
        }

        protected Box(string boxName, User user, BoxPrivacySetting privacySetting, Guid newCommentId)
            : this(boxName, user, privacySetting)
        {
            Comments.Add(new Comment(user, null,
               this, newCommentId, null, FeedType.CreatedBox));
            CommentCount = 1;
        }

        protected Box(string boxName, User user, BoxPrivacySetting privacySetting)
            : this()
        {
            if (boxName == null)
            {
                throw new ArgumentNullException(nameof(boxName));
            }
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            Name = boxName.Trim();
            PrivacySettings.PrivacySetting = privacySetting;
            UserTime = new UserTimeDetails(user.Id);
            Owner = user;
            UserBoxRelationship.Add(new UserBoxRel(user, this, UserRelationshipType.Owner));

            ItemTabs.Add(new ItemTab(GuidIdGenerator.GetGuid(), DomainResource.TabName1, this));
            ItemTabs.Add(new ItemTab(GuidIdGenerator.GetGuid(), DomainResource.TabName2, this));
            ItemTabs.Add(new ItemTab(GuidIdGenerator.GetGuid(), DomainResource.TabName3, this));
            ItemTabs.Add(new ItemTab(GuidIdGenerator.GetGuid(), DomainResource.TabName4, this));
            ItemTabs.Add(new ItemTab(GuidIdGenerator.GetGuid(), DomainResource.TabName5, this));

            CalculateMembers();
        }
        // ReSharper restore DoNotCallOverridableMethodsInConstructor

        public virtual long Id { get; protected set; }
        public virtual string Name { get; protected set; }
        public virtual UserTimeDetails UserTime { get; set; }
        public virtual PrivacySettings PrivacySettings { get; set; }
        public virtual bool IsDeleted { get; set; }
        public virtual User Owner { get; protected set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public virtual string Url { get; protected set; }

        public virtual ICollection<UserBoxRel> UserBoxRelationship { get; protected set; }
        public virtual ICollection<Item> Items { get; protected set; }
        public virtual ICollection<Quiz> Quizzes { get; protected set; }
        public virtual ICollection<FlashcardMeta> Flashcards { get; protected set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<CommentReply> Replies { get; set; }
        protected virtual ICollection<ItemTab> ItemTabs { get; set; }

        protected virtual ICollection<Updates> Updates { get; set; }
        public virtual ICollection<InviteToBox> Invites { get; set; }


        public virtual int MembersCount { get; protected set; }
        public virtual int ItemCount { get; protected set; }
        public virtual int QuizCount { get; protected set; }
        public virtual int FlashcardCount { get; protected set; }

        public virtual int CommentCount { get; protected set; }


        public void ChangeBoxName(string newBoxName, User user)
        {
            if (newBoxName == null)
            {
                throw new ArgumentNullException(nameof(newBoxName));
            }
            if (string.Equals(Name, newBoxName, StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            Name = newBoxName.Trim();
            UserTime.UpdateUserTime(user.Id);
            GenerateUrl();
        }



        public virtual void GenerateUrl()
        {
            if (Id == 0)
            {
                return;
            }
            Url = UrlConst.BuildBoxUrl(Id, Name, string.Empty);
        }

        public virtual string GetUniversityName()
        {
            return null;
        }

        public virtual File AddFile(string fileName,
            User user,
            long length,
            string blobAddressName
           )
        {
            var file = new File(fileName, user, length, blobAddressName, this);
            return AddItem(file) as File;
        }

        public virtual Item AddLink(string url, User user, int linkStorageSize, string linkTitle)
        {
            var linkExist = Items.Any(s => s.ItemContentUrl == url && !s.IsDeleted);
            if (linkExist)
            {
                throw new DuplicateNameException("This link already exists in the box");
            }
            var link = new Link(url, user, linkStorageSize, this, linkTitle);
            return AddItem(link);
        }

        protected virtual Item AddItem(Item item)
        {
            //Need to put in here unique item name check per box
            Items.Add(item);
            UpdateItemCount();
            UserTime.UpdateUserTime(item.User.Id);
            return item;
        }

        public virtual Comment AddComment(User user, string text, Guid id, IList<Item> items, FeedType feedType)
        {
            var comment = new Comment(user, text, this, id, items, feedType);
            Comments.Add(comment);
            UpdateCommentsCount();
            UserTime.UpdateUserTime(user.Id);
            return comment;
        }

        /// <summary>
        /// Deletes a comment
        /// </summary>
        /// <param name="comment">The comment to delete</param>
        /// <returns>Users list who their post are deleted</returns>
        public virtual IEnumerable<long> DeleteComment(Comment comment)
        {
            var usersAffectedByDeleteComment = comment.RepliesReadOnly.Where(w => w.LikeCount > 0).Select(s => s.User.Id).Union(new[] { comment.User.Id });
            Comments.Remove(comment);
            UpdateCommentsCount();
            return usersAffectedByDeleteComment;
        }


        #region membersCount
        public virtual void CalculateMembers()
        {
            MembersCount = UserBoxRelationship.Count;
        }

        #endregion
        #region itemCount

        public virtual void UpdateItemCount()
        {
            ShouldMakeDirty = () => true;
            ItemCount = Items.Count(file => !file.IsDeleted);
            QuizCount = Quizzes.Count(quiz => quiz.Publish && !quiz.IsDeleted);
        }

        public virtual void UpdateFlashcardCount()
        {
            ShouldMakeDirty = () => true;
            FlashcardCount = Flashcards.Count(f => f.Publish && !f.IsDeleted);
        }
        #endregion
        #region commentCount

        public virtual void UpdateCommentsCount()
        {
            CommentCount = Comments.Count;
        }

        #endregion

        




        public void ChangePrivacySettings(BoxPrivacySetting boxPrivacySetting, User user)
        {
            if (PrivacySettings.PrivacySetting == boxPrivacySetting)
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

            PrivacySettings.PrivacySetting = boxPrivacySetting;

        }


        public virtual void DeleteAssociation()
        {
            Invites.Clear();
            UserBoxRelationship.Clear();
            Comments.Clear();
            ItemTabs.Clear();
            Updates.Clear();
        }

        public virtual void UnFollowBox(long userId)
        {
            var userBoxRel = UserBoxRelationship.FirstOrDefault(w => w.User.Id == userId);
            if (userBoxRel == null) //this happen when user decline invite of a box that is public
            {
                throw new InvalidOperationException("User does not have an active invite");
            }
            UserBoxRelationship.Remove(userBoxRel);
            CalculateMembers();
        }

        public DirtyState IsDirty { get; set; }


        public virtual Func<bool> ShouldMakeDirty { get; set; }

        public virtual object Actual => this;
    }
}
