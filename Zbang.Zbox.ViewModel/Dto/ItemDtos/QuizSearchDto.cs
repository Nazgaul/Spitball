using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class QuizSearchDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string BoxName { get; set; }

        public IEnumerable<string> Questions { get; set; }
        public IEnumerable<string> Answers { get; set; }
        public IEnumerable<long> UserIds { get; set; }
        public long BoxId { get; set; }
        public long? UniversityId { get; set; }
        public string Url { get; set; }
        public string UniversityName { get; set; }

    }

    public class QuizQuestionAndAnswersSearchDto
    {
        public string Text { get; set; }
        public long QuizId { get; set; }
    }

    public class QuizToUpdateSearchDto
    {
        public IEnumerable<QuizSearchDto> QuizzesToUpdate { get; set; }

        public IEnumerable<long> QuizzesToDelete { get; set; }
    }
}
