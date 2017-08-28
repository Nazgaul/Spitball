using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;

namespace Zbang.Zbox.ViewModel.Dto.Qna
{
    public class FeedSearchDto
    {
        protected FeedSearchDto()
        {
            Tags = new List<FeedSearchTag>();
        }
        public Guid Id { get; set; }
        public string Text { get; set; }
        public ItemUniversitySearchDto University { get; set; }
        public ItemCourseSearchDto Course { get; set; }
        public DateTime Date { get; set; }

        public int LikeCount { get; set; }
        public int ReplyCount { get; set; }

        public IEnumerable<string> Replies { get; set; }

        public long Version { get; set; }

        public string Content
        {
            get
            {
                if (Replies != null)
                {
                    return Text + string.Join(" ", Replies);
                }
                return Text;
            }
        }

        public Language? Language { get; set; }
        public List<FeedSearchTag> Tags { get; set; }

        public int ItemCount { get; set; }

        public string UserName { get; set; }
        public string UserImage { get; set; }
    }

    public class RepliesSearchDto
    {
        public Guid QuestionId { get; set; }
        public string Text { get; set; }
    }
}
