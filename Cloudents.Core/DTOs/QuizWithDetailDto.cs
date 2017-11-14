using System;
using System.Collections.Generic;

namespace Cloudents.Core.DTOs
{
    public class QuizWithDetailDto
    {
        private DateTime _date;
        public string Name { get; set; }

        public string Owner { get; set; }

        public long Id { get; set; }
        public DateTime Date
        {
            get => _date;
            set => _date = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public int NumberOfViews { get; set; }

        public IEnumerable<QuizQuestionWithDetailDto> Questions { get; set; }

    }
}