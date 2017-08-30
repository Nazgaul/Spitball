using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Domain
{
    public class Comment : ITag
    {
        protected Comment()
        {
        }

        public Comment(User user, string text, Box box, Guid id, IList<Item> items, FeedType feedType, bool anonymous)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (box == null)
            {
                throw new ArgumentNullException(nameof(box));
            }
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            Id = id;
            Items = items ?? new List<Item>();
            foreach (var item in Items)
            {
                item.CommentReply = null;
            }
            User = user;
            Box = box;
            Text = ExtractCommentText(feedType, text, user);
            DateTimeUser = new UserTimeDetails(user.Id);
            Box.UserTime.UpdateUserTime(user.Id);
            FeedType = feedType;
            Anonymous = anonymous;
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        private string ExtractCommentText(FeedType feedType, string text, User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (feedType == Infrastructure.Enums.FeedType.None) return string.IsNullOrEmpty(text) ? null : text.Trim();
            return feedType.GetEnumDescription(Languages.GetCultureBaseOnCountry(user.University.Country));
        }

        public virtual Guid Id { get; set; }
        public virtual User User { get; set; }
        public virtual string Text { get; set; }
        public virtual Box Box { get; set; }
        protected virtual ICollection<Item> Items { get; set; }



        protected virtual ICollection<Quiz> Quizzes { get; set; }
        protected virtual ICollection<FlashcardMeta> Flashcards { get; set; }

        protected virtual ICollection<CommentReply> Replies { get; set; }
        protected virtual ICollection<Updates> Updates { get; set; }
        protected virtual ICollection<CommentLike> Likes { get; set; }
        public ICollection<CommentReply> RepliesReadOnly => Replies.ToList().AsReadOnly();

        public virtual bool Anonymous { get; set; }

        public virtual FeedType? FeedType { get; set; }

        public virtual int ReplyCount { get; set; }

        public virtual int LikeCount { get; set; }


        public virtual UserTimeDetails DateTimeUser { get; set; }

        public void AddItem(Item item)
        {
            Items.Add(item);
        }

        public virtual int ItemsCount()
        {
            return Items.Count;
        }

        public virtual void AddQuiz(Quiz quiz)
        {
            if (Quizzes == null)
            {
                Quizzes = new List<Quiz>();
            }
            Quizzes.Add(quiz);
        }

        public virtual void AddFlashcard(FlashcardMeta flashcard)
        {
            if (Flashcards == null)
            {
                Flashcards = new List<FlashcardMeta>();
            }
            Flashcards.Add(flashcard);
        }

        public bool RemoveItem(Item item)
        {
            Items.Remove(item);
            return NeedToRemoveComment();
        }



        private bool NeedToRemoveComment()
        {
            return Items.Count == 0 && Quizzes.Count == 0
                && (string.IsNullOrEmpty(Text)
                || FeedType == Infrastructure.Enums.FeedType.AddedItems);
        }

        public bool RemoveQuiz(Quiz quiz)
        {
            Quizzes.Remove(quiz);
            return NeedToRemoveComment();
        }

        public virtual ISet<CommentTag> CommentTags { get; set; }


        public virtual  Task AddTagAsync(Tag tag, TagType type, IJaredPushNotification jaredPush)
        {
            var newExists = CommentTags.FirstOrDefault(w => w.Tag.Id == tag.Id);
            if (newExists != null) return Task.CompletedTask;
            newExists = new CommentTag(tag, this, type);
            CommentTags.Add(newExists);
            tag.CommentTags.Add(newExists);
            if (type != TagType.Watson)
            {
                //TODO: think about this
                DateTimeUser.UpdateTime = DateTime.UtcNow;
            }
            if (DateTimeUser.CreationTime.AddDays(1) > DateTime.UtcNow)
            {
                return jaredPush.SendAddPostNotificationAsync(User.Name, Text, Box.Id, Id, tag.Name);
            }
            return Task.CompletedTask;
        }

        public void RemoveTag(string tag)
        {
            var tagToRemove = CommentTags.FirstOrDefault(w => w.Tag.Name == tag);
            if (tagToRemove == null) return;
            CommentTags.Remove(tagToRemove);
            //TODO: think about this
            DateTimeUser.UpdateTime = DateTime.UtcNow;
        }
    }
}
