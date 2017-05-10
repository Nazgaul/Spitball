using System;
using System.Net;
using Zbang.Zbox.Infrastructure;

namespace Zbang.Zbox.ViewModel.Dto.JaredDtos
{
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
}
