using System;
using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Infrastructure.Exceptions;

namespace Zbang.Zbox.Domain
{
    public class Comment
    {
        protected Comment()
        {

        }
        public Comment(User user, string text, Box box, Guid id, IList<Item> items)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (box == null)
            {
                throw new ArgumentNullException("box");
            }
            Id = id;
            Items = items ?? new List<Item>();
            User = user;
            Box = box;
            if (text != null)
            {
                text = text.Trim();
            }
            if (text == string.Empty)
            {
                text = null;
            }
            Text = text;
            DateTimeUser = new UserTimeDetails(user.Email);
            Box.UserTime.UpdateUserTime(user.Email);
        }
        public virtual Guid Id { get; set; }
        public virtual User User { get; set; }
        public virtual string Text { get; set; }
        public virtual Box Box { get; set; }
        protected virtual ICollection<Item> Items { get; private set; }

        protected virtual ICollection<Quiz> Quizes { get; set; }

        protected virtual ICollection<CommentReplies> Answers { get; set; }
        protected virtual ICollection<Updates> Updates { get; set; }

        public ICollection<CommentReplies> AnswersReadOnly { get { return Answers.ToList().AsReadOnly(); } }


        public virtual UserTimeDetails DateTimeUser { get; set; }

        public void AddItem(Item item)
        {
            Items.Add(item);
        }

        public virtual void AddQuiz(Quiz quiz)
        {
            if (Quizes == null)
            {
                Quizes = new List<Quiz>();
            }
            Quizes.Add(quiz);
        }

        public void RemoveItem(Item item)
        {
            Items.Remove(item);
        }
    }
}
