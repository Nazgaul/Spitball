using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Core.Entities
{
    public class CourseSubject : Entity<long>, IAggregateRoot
    {
        public CourseSubject(string name, Country country)
        {
            Name = name;
            Country = country;
        }
        protected CourseSubject()
        {
        }
        public virtual string Name { get; protected set; }

        public virtual Country Country { get; protected set; }
        //public virtual string EnglishName { get; set; }
        // private readonly ISet<CourseSubjectTranslation> _translations = new HashSet<CourseSubjectTranslation>();
        //public virtual IEnumerable<CourseSubjectTranslation> Translations => _translations.ToList();

        public virtual byte[] Version { get; protected set; }

        //public virtual void AddTranslation(CourseSubjectTranslation translation)
        //{
        //    _translations.Add(translation);
        //}

        public virtual void ChangeName(string newName)
        {
            Name = newName;
        }

        //public virtual void DeleteTranslation(CourseSubjectTranslation translation)
        //{
        //    _translations.Remove(translation);
        //}
    }
}
