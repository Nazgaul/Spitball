using System;
using System.Collections.Generic;
using System.Linq;

namespace Zbang.Zbox.ViewModel.Dto.Qna
{
    public class FeedToUpdateSearchDto
    {
        public IEnumerable<FeedSearchDto> Updates { get; set; }

        public IEnumerable<FeedSearchDeleteDto> Deletes { get; set; }

        public long NextVersion
        {
            get
            {
                var versions = Updates.Select(s => s.Version);
                var versionsDelete = Deletes.Select(s => s.Version);
                return versions.Union(versionsDelete).Max();
            }
        }
    }

    public class FeedSearchDeleteDto
    {
        public Guid Id { get; set; }
        public long Version { get; set; }
    }
}