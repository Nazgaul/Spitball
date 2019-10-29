using Cloudents.Core.DTOs;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cloudents.Infrastructure.Test
{
    public class FeedSortTests
    {
        [Fact]
        public void SourtFeed_first_page()
        {

            var feedSort = new FeedSort();
            //Page-0
            IList<FeedDto> itemsFeed = Enumerable.Range(0, 18).Select(s => new DocumentFeedDto()).ToList<FeedDto>();
            IList<TutorCardDto> tutorsFeed = Enumerable.Range(0, 3).Select(s => new TutorCardDto()).ToList();

            var res = feedSort.SortFeed(itemsFeed, tutorsFeed, 0).ToList();
            res[2].GetType().Should().Be(typeof(TutorCardDto));
            res[12].GetType().Should().Be(typeof(TutorCardDto));
            res[19].GetType().Should().Be(typeof(TutorCardDto));

        }
        
        [Fact]
        public void SourtFeed_second_page()
        {
            //Page-1
            var feedSort = new FeedSort();
            IList<FeedDto>  itemsFeed = Enumerable.Range(0, 18).Select(s => new DocumentFeedDto()).ToList<FeedDto>();
            IList<TutorCardDto>  tutorsFeed = Enumerable.Range(0, 3).Select(s => new TutorCardDto()).ToList();

            var res = feedSort.SortFeed(itemsFeed, tutorsFeed, 1).ToList();
            res[6].GetType().Should().Be(typeof(TutorCardDto));
            res[13].GetType().Should().Be(typeof(TutorCardDto));
            res[20].GetType().Should().Be(typeof(TutorCardDto));
        }

        [Fact]
        public void SourtFeed_first_page_12_items()
        {
            //12-docs Page-0
            var feedSort = new FeedSort();
            IList<FeedDto>  itemsFeed = Enumerable.Range(0, 12).Select(s => new DocumentFeedDto()).ToList<FeedDto>();
            IList<TutorCardDto>  tutorsFeed = Enumerable.Range(0, 3).Select(s => new TutorCardDto()).ToList();
            var res = feedSort.SortFeed(itemsFeed, tutorsFeed, 0).ToList();
            res[2].GetType().Should().Be(typeof(TutorCardDto));
            res[12].GetType().Should().Be(typeof(TutorCardDto));
            res[res.Count-1].GetType().Should().Be(typeof(TutorCardDto));
            res.Contains(null).Should().BeFalse();
        }

        [Fact]
        public void SourtFeed_first_page_no_items()
        {
            var feedSort = new FeedSort();
            IList<FeedDto> itemsFeed = new List<FeedDto>();
            IList<TutorCardDto> tutorsFeed = Enumerable.Range(0, 3).Select(s => new TutorCardDto()).ToList();
            var res = feedSort.SortFeed(itemsFeed, tutorsFeed, 0).ToList();
            res[0].GetType().Should().Be(typeof(TutorCardDto));
            res[1].GetType().Should().Be(typeof(TutorCardDto));
            res[res.Count - 1].GetType().Should().Be(typeof(TutorCardDto));
            res.Contains(null).Should().BeFalse();
        }


        [Fact]
        public void SourtFeed_first_page_null_items()
        {
            var feedSort = new FeedSort();
            IList<TutorCardDto> tutorsFeed = Enumerable.Range(0, 3).Select(s => new TutorCardDto()).ToList();
            var res = feedSort.SortFeed(null, tutorsFeed, 0).ToList();
            res[0].GetType().Should().Be(typeof(TutorCardDto));
            res[1].GetType().Should().Be(typeof(TutorCardDto));
            res[res.Count - 1].GetType().Should().Be(typeof(TutorCardDto));
            res.Contains(null).Should().BeFalse();
        }

        [Fact]
        public void SourtFeed_first_page_1_tutor()
        {
            //1-tutor
            var feedSort = new FeedSort();
            IList<FeedDto>  itemsFeed = Enumerable.Range(0, 18).Select(s => new DocumentFeedDto()).ToList<FeedDto>();
            IList<TutorCardDto> tutorsFeed = Enumerable.Range(0, 1).Select(s => new TutorCardDto()).ToList();
            var res = feedSort.SortFeed(itemsFeed, tutorsFeed, 0).ToList();
            res[2].GetType().Should().Be(typeof(TutorCardDto));
            res.Contains(null).Should().BeFalse();
        }

        [Fact]
        public void SourtFeed_first_page_no_tutor()
        {
            //1-tutor
            var feedSort = new FeedSort();
            IList<FeedDto> itemsFeed = Enumerable.Range(0, 18).Select(s => new DocumentFeedDto()).ToList<FeedDto>();
            IList<TutorCardDto> tutorsFeed = new List<TutorCardDto>();
            var res = feedSort.SortFeed(itemsFeed, tutorsFeed, 0).ToList();
            res.Count.Should().Equals(18);
            res.Contains(null).Should().BeFalse();
        }


        [Fact]
        public void SourtFeed_first_page_null_tutor()
        {
            //1-tutor
            var feedSort = new FeedSort();
            IList<FeedDto> itemsFeed = Enumerable.Range(0, 18).Select(s => new DocumentFeedDto()).ToList<FeedDto>();
            var res = feedSort.SortFeed(itemsFeed, null, 0).ToList();
            res.Count.Should().Equals(18);
            res.Contains(null).Should().BeFalse();
        }
    }
}
