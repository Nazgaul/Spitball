//using Cloudents.Core.Attributes;
//using Cloudents.Core.DTOs.Users;
//using System;

//namespace Cloudents.Core.Entities
//{
//    public class ViewQuestionWithFirstAnswer
//    {
//        protected ViewQuestionWithFirstAnswer()
//        {

//        }

//        [EntityBind(nameof(Question.Id))]
//        public virtual long Id { get; set; }
//        [EntityBind(nameof(Question.Text))]
//        public virtual string Text { get; set; }
//        [EntityBind(nameof(Question.Course.Id))]
//        public virtual string Course { get; set; }
//        [EntityBind(nameof(Question.Answers))]
//        public virtual int Answers { get; set; }
//        [EntityBind(nameof(Question.Updated))]
//        public virtual DateTime DateTime { get; set; }
//        [EntityBind(nameof(Question.Answers))]
//        public virtual ViewQuestionAnswer Answer { get; set; }
//        [EntityBind(nameof(Question.User))]
//        public virtual UserDto User { get; set; }
//    }

//    public class ViewQuestionAnswer
//    {
//        [EntityBind(nameof(Answer.User.Id))]
//        public long? UserId { get; set; }
//        [EntityBind(nameof(Answer.User.Image))]
//        public string UserImage { get; set; }
//        [EntityBind(nameof(Answer.User.Name))]
//        public string UserName { get; set; }
//        [EntityBind(nameof(Answer.Text))]
//        public string Text { get; set; }
//        [EntityBind(nameof(Answer.Created))]
//        public DateTime DateTime { get; set; }
//    }
//}