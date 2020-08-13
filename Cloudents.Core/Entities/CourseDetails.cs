using System.Collections.Generic;

namespace Cloudents.Core.Entities
{
    public class CourseDetails : ValueObject
    {
        public string HeroButton { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            throw new System.NotImplementedException();
        }
    }
}