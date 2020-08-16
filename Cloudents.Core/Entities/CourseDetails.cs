using System.Collections.Generic;

namespace Cloudents.Core.Entities
{
    public class CourseDetails : ValueObject
    {
        public string? HeroButton { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return HeroButton;
            //throw new System.NotImplementedException();
        }
    }
}