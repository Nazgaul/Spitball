using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Extensions;
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
            Code = code.NullIfWhiteSpace();
            Professor = professor.NullIfWhiteSpace();

            Create = DateTime.UtcNow;
        }

        public virtual Guid Id { get; set; }
        public virtual string Name { get; protected set; }
        public virtual string Code { get; protected set; }
        public virtual string Professor { get; protected set; }

        //public ICollection<Item> Items { get; set; }
        //public ICollection<Quiz> Quizzes { get; set; }
        //public ICollection<FlashcardMeta> Flashcards { get; set; }
        //public ICollection<Comment> Comments { get; set; }

        public virtual bool IsDeleted { get; set; }
        public virtual void DeleteAssociation()
        {
            //throw new NotImplementedException();
        }

        public virtual bool IsDirty { get; set; }

        public DateTime Create { get; set; }
        public virtual Func<bool> ShouldMakeDirty { get; }

        //public override bool Equals(object obj)
        //{
        //    var item = obj as CourseTag;
        //    if (item == null)
        //    {
        //        return false;
        //    }
        //    string code = null;
        //    if (!string.IsNullOrWhiteSpace(item.Code))
        //    {
        //        code = item.Code;
        //    }
        //    string professor = null;
        //    if (!string.IsNullOrWhiteSpace(item.Professor))
        //    {
        //        professor = item.Professor;
        //    }
        //    return this.Name.Equals(item.Name) &&
        //           this.Code.Equals(code) &&
        //           this.Professor.Equals(professor);
        //}

        //public override int GetHashCode()
        //{
        //    return this.Name.GetHashCode() + 3 * this.Code.GetHashCode() + 5 * this.Professor.GetHashCode();
        //}
    }
}
