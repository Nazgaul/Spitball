using System.Collections.Generic;

namespace Zbang.Zbox.Infrastructure.File
{
    public class PreviewResult
    {
        public PreviewResult()
        {
            Content = new List<string>();
        }
        public PreviewResult(string content)
        {
            Content = new List<string> { content };
        }
        public IEnumerable<string> Content { get; set; }
        public string ViewName { get; set; }
    }
}
