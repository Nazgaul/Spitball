using Cloudents.Core.Attributes;
using Cloudents.Core.DTOs.Users;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using System;

namespace Cloudents.Core.DTOs.Feed
{
    public abstract class FeedDto
    {

        public abstract FeedType Type { get; }
    }
    public class QuestionFeedDto : FeedDto
    {
        [EntityBind(nameof(Question.Id))]
        public long Id { get; set; }
        [EntityBind(nameof(Question.Text))]
        public string Text { get; set; }
        //public int Files { get; set; }
        [EntityBind(nameof(Answer.Question))]

        public int Answers { get; set; }


        public QuestionUserDto User { get; set; }

        [EntityBind(nameof(Question.Updated))]
        public DateTime DateTime { get; set; }

        [EntityBind(nameof(Question.Course2.CardDisplay))]
        public string Course { get; set; }

        [EntityBind(nameof(Question.User.Id))]
      
        public override FeedType Type => FeedType.Question;

        [EntityBind(nameof(Question.Answers))]

        public AnswerFeedDto FirstAnswer { get; set; }
    }

    public class AnswerFeedDto
    {
        [EntityBind(nameof(Answer.User))]
        public UserDto User { get; set; }

        [EntityBind(nameof(Answer.Text))]
        public string Text { get; set; }

        [EntityBind(nameof(Answer.Created))]
        public DateTime DateTime { get; set; }
    }

    public class QuestionUserDto
    {
        [EntityBind(nameof(Question.User.Id))]
        public long Id { get; set; }
        [EntityBind(nameof(Question.User.Name))]
        public string Name { get; set; }
        [EntityBind(nameof(Question.User.ImageName))]
        public string? Image { get; set; }
    }



}