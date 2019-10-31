using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Infrastructure
{
    public class FeedSort : IFeedSort
    {

        public IEnumerable<FeedDto> SortFeed(IList<FeedDto> itemsFeed, IList<TutorCardDto> tutorsFeed, int page)
        {
            if (itemsFeed == null)
            {
                return tutorsFeed;
            }
            if (tutorsFeed == null)
            {
                return itemsFeed;
            }
           
            var tutorlocationPageZero = new[] { 2, 12, 19 };
            var tutorlocationPage = new[] { 6, 13, 20 };

            var locations = new[] { tutorlocationPageZero, tutorlocationPage };

            var pageLocations = locations.ElementAtOrDefault(page) ?? tutorlocationPage;
            

            foreach (var item in pageLocations)
            {
                var tutor = tutorsFeed.FirstOrDefault();
                if (tutor is null)
                {
                    break;
                }
                itemsFeed.Insert(Math.Min(itemsFeed.Count, item), tutor);
                tutorsFeed.RemoveAt(0);
            }

            return itemsFeed;
        }
    }
}
