using System;
using System.Collections.Generic;
using System.Net;
using Zbang.Zbox.Infrastructure;

namespace Zbang.Zbox.ViewModel.Dto.JaredDtos
{
    public class JaredFavoriteDto
    {
        public IEnumerable<JaredFavoriteDocumentDto> Documents { get; set; }
        public IEnumerable<JaredFavoriteQuiz> Quizzes { get; set; }
        public IEnumerable<JaredFavoriteFlashcardDto> Flashcards { get; set; }
        public IEnumerable<JaredFavoriteCommentDto> Comments { get; set; }
    }
    public class JaredFavoriteDocumentDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
        public string Meta { get; set; }
        public int Likes { get; set; }
        public int Views { get; set; }
        public DateTime Date { get; set; }
    }

    public class JaredFavoriteQuiz
    {
        private string m_Source;
        public long Id { get; set; }
        public string Name { get; set; }

        public string Source
        {
            get { return m_Source; }
            set { m_Source = TextManipulation.RemoveHtmlTags.Replace(WebUtility.HtmlDecode(value), string.Empty); }
        }

        public int NumOfQuestion { get; set; }
        public int Likes { get; set; }
        public int Views { get; set; }
        public DateTime Date { get; set; }
    }

    public class JaredFavoriteFlashcardDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Likes { get; set; }
        public int Views { get; set; }
        public DateTime Date { get; set; }

        public int CardCount { get; set; }
        public string CardFront { get; set; }
        public string CardBack { get; set; }
    }

    public class JaredFavoriteCommentDto
    {
        public Guid Id { get; set; }
        public long CourseId { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public int Replies { get; set; }
        public int Items { get; set; }
    }
}
