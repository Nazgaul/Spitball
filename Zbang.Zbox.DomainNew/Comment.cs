using System;
using System.Collections.Generic;
using System.Linq;

namespace Zbang.Zbox.Domain
{
    public class Comment
    {
        protected Comment()
        {

        }
        public Comment(User user, string text, Box box, Guid id, IList<Item> items, bool isSystemGenerated)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (box == null)
            {
                throw new ArgumentNullException("box");
            }
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
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
            IsSystemGenerated = isSystemGenerated;
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
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

        public virtual bool IsSystemGenerated { get; set; }

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
            return Items.Count == 0 && (string.IsNullOrEmpty(Text) || IsSystemGenerated);
        }
    }
}
