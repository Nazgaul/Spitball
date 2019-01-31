using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using System;
using System.Collections.Generic;


namespace Cloudents.Core.DTOs.Admin
{

    public class UserInfoDto
    {
        public UserDetailsDto User { get; set; }
       
        public IEnumerable<UserQuestionsDto> Questions { get; set; }
       
        public IEnumerable<UserAnswersDto> Answers { get; set; }
    
        public IEnumerable<UserDocumentsDto> Documents { get; set; }
    }
    
    public class UserAnswersDto
    {
        [DtoToEntityConnection(nameof(Answer.Id))]
        public Guid Id { get; set; }
        [DtoToEntityConnection(nameof(Answer.Text))]
        public string Text { get; set; }
        [DtoToEntityConnection(nameof(Answer.Created))]
        public DateTime Created { get; set; }
        [DtoToEntityConnection(nameof(Answer.Question.Id))]
        public long QuestionId { get; set; }
        [DtoToEntityConnection(nameof(Answer.Question.Text))]
        public string QuestionText { get; set; }
        [DtoToEntityConnection(nameof(Answer.Status.State))]
        public string State { get; set; }

    }

    public class UserQuestionsDto
    {
        [DtoToEntityConnection(nameof(Question.Id))]
        public long Id { get; set; }
        [DtoToEntityConnection(nameof(Question.Text))]
        public string Text { get; set; }
        [DtoToEntityConnection(nameof(Question.Created))]
        public DateTime Created { get; set; }
        [DtoToEntityConnection(nameof(Question.Status.State))]
        public string State { get; set; }
    }
    public class UserDocumentsDto
    {
        [DtoToEntityConnection(nameof(Document.Id))]
        public long Id { get; set; }
        [DtoToEntityConnection(nameof(Document.Name))]
        public string Name { get; set; }
        [DtoToEntityConnection(nameof(Document.TimeStamp.CreationTime))]
        public DateTime Created { get; set; }
        [DtoToEntityConnection(nameof(Document.University.Name))]
        public string University { get; set; }
        [DtoToEntityConnection(nameof(Document.Course.Name))]
        public string Course { get; set; }
        [DtoToEntityConnection(nameof(Document.Price))]
        public decimal? Price { get; set; }
        [DtoToEntityConnection(nameof(Document.Status.State))]
        public string State { get; set; }
        
        public Uri Preview { get; set; }
        public string SiteLink { get; set; }
    }
}
