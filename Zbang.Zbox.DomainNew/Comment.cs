﻿using System;
using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain
{
    public class Comment //: ISoftDeletable
    {
        protected Comment()
        {
        }
        public Comment(User user, string text, Box box, Guid id, IList<Item> items, FeedType feedType)
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
            User = user;
            Box = box;
            Text = ExtractCommentText(feedType, text, user);
            DateTimeUser = new UserTimeDetails(user.Id);
            Box.UserTime.UpdateUserTime(user.Id);
            FeedType = feedType;
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



        protected virtual ICollection<Quiz> Quizes { get; set; }

        protected virtual ICollection<CommentReplies> Replies { get; set; }
        protected virtual ICollection<Updates> Updates { get; set; }
        protected virtual ICollection<CommentLike> Likes { get; set; }
        public ICollection<CommentReplies> RepliesReadOnly => Replies.ToList().AsReadOnly();
        public ICollection<Item> ItemsReadOnly => Items.ToList().AsReadOnly();


        public virtual FeedType? FeedType { get; set; }

        public virtual int ReplyCount { get; set; }

        public virtual int LikeCount { get; set; }

        public virtual Guid? LastReplyId { get; set; }

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
            if (Quizes == null)
            {
                Quizes = new List<Quiz>();
            }
            Quizes.Add(quiz);
        }

        public bool RemoveItem(Item item)
        {
            Items.Remove(item);
            return NeedToRemoveComment();
        }



        private bool NeedToRemoveComment()
        {
            return Items.Count == 0 && Quizes.Count == 0
                && (string.IsNullOrEmpty(Text)
                || FeedType == Infrastructure.Enums.FeedType.AddedItems);
        }

        public bool RemoveQuiz(Quiz quiz)
        {
            Quizes.Remove(quiz);
            return NeedToRemoveComment();
        }
    }
}
