using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain
{
    public class CourseTag : IDirty
    {
        protected CourseTag()
        {
            
        }

        public CourseTag(Guid id, string name, string code, string professor)
        {
            Id = id;
            Name = name;
            Code = code;
            Professor = professor;
            Create = DateTime.UtcNow;
        }

        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Code { get; set; }
        public virtual string Professor { get; set; }

        public ICollection<Item> Items { get; set; }
        public ICollection<Quiz> Quizzes { get; set; }
        public ICollection<FlashcardMeta> Flashcards { get; set; }
        public ICollection<Comment> Comments { get; set; }

        public virtual bool IsDeleted { get; set; }
        public virtual void DeleteAssociation()
        {
            //throw new NotImplementedException();
        }

        public virtual DirtyState IsDirty { get; set; }

        public DateTime Create { get; set; }
        public virtual Func<bool> ShouldMakeDirty { get; }
    }
}
