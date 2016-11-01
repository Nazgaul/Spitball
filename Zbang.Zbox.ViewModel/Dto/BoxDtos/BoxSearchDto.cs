using System;
using System.Collections.Generic;


namespace Zbang.Zbox.ViewModel.Dto.BoxDtos
{
    /// <summary>
    /// Used to upload data to azure search
    /// </summary>
    public class BoxSearchDto
    {

        public long Id { get; set; }
        public string Name { get; set; }
        public string Professor { get; set; }
        public string CourseCode { get; set; }

        public string Url { get; set; }

        public long UniversityId { get; set; }


        public Infrastructure.Enums.BoxType Type { get; set; }

        public IEnumerable<string> Department { get; set; }
        public Guid DepartmentId { get; set; }

        public IEnumerable<string> Feed { get; set; }

        public IEnumerable<long> UserIds { get; set; }
    }

    public class UsersInBoxSearchDto
    {
        public long UserId { get; set; }
        public long BoxId { get; set; }
    }

    public class DepartmentOfBoxSearchDto
    {
        public long BoxId { get; set; }
        public string Name { get; set; }
    }
    public class FeedOfBoxSearchDto
    {
        public long BoxId { get; set; }
        public string Text { get; set; }
    }

    public class BoxToUpdateSearchDto
    {
        public IEnumerable<BoxSearchDto> BoxesToUpdate { get; set; }

        public IEnumerable<long> BoxesToDelete { get; set; }
    }
}
