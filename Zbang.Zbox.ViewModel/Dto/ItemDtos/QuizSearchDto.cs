﻿using System;
using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class QuizSearchDto : ItemSearchDto
    {
        public override IEnumerable<ItemType> Type => new[] { ItemType.Quiz };
        public override string SearchContentId => "quiz_" + Id;

        public IEnumerable<string> Questions { get; set; }
        public IEnumerable<string> Answers { get; set; }
        public IEnumerable<long> UserIds { get; set; }

        public long BoxId { get; set; }
        public long? UniversityId { get; set; }
        public string Url { get; set; }
        public override string Content => TextManipulation.RemoveHtmlTags.Replace(string.Join(" ", Questions) + string.Join(" ", Answers), string.Empty);
        public override string[] MetaContent => new[] { Questions.FirstOrDefault(), Answers.FirstOrDefault() };
        public override int? ContentCount => Questions.Count();
    }

    public class QuizQuestionAndAnswersSearchDto
    {
        public string Text { get; set; }
        public long QuizId { get; set; }
        public Guid Questionid { get; set; }
    }

    public class QuizToUpdateSearchDto
    {
        public IEnumerable<QuizSearchDto> QuizzesToUpdate { get; set; }

        public IEnumerable<QuizToDeleteSearchDto> QuizzesToDelete { get; set; }
    }

    public class QuizToDeleteSearchDto : ItemToDeleteSearchDto
    {
        public override string SearchContentId => "quiz_" + Id;
    }
}
