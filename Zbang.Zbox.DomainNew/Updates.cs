using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Domain
{
    public class Updates
    {
        protected Updates()
        {

        }
        public Updates(User user, Box box,
            Question question = null, Answer answer = null, Item item = null,
            ItemComment annotation = null)
        {
            User = user;
            Box = box;
            Question = question;
            Answer = answer;
            Item = item;
            Annotation = annotation;
            CreationTime = DateTime.UtcNow;
        }
        public virtual Guid Id { get; set; }
        public virtual User User { get; set; }
        public virtual Box Box { get; set; }
        public virtual Question Question { get; set; }
        public virtual Answer Answer { get; set; }
        public virtual Item Item { get; set; }
        public virtual ItemComment Annotation { get; set; }
        public virtual DateTime CreationTime { get; set; }
    }
}
