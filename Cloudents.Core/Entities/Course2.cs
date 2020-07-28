using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    public class Course :Entity<long>
    {
        public Course(string name, Tutor tutor)
        {
            Name = name;
            Tutor = tutor;
        }

        protected Course()
        {
            
        }
        public virtual string Name { get; set; }

        public virtual Tutor Tutor { get; set; }

        public virtual int Position { get; }

        public virtual Money Price { get; }
        public virtual Money SubscriptionPrice { get; }


        [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
        private readonly ICollection<Document> _documents = new List<Document>();

        public virtual IEnumerable<Document> Documents => _documents;


        [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
        private readonly ICollection<StudyRoom> _studyRooms = new List<StudyRoom>();

        public virtual IEnumerable<StudyRoom> StudyRooms => _studyRooms;

    }
}