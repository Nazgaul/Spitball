namespace Cloudents.Core.Request
{
    public class AiQuery : IQuery
    {
        public AiQuery(string sentence)
        {
            Sentence = sentence;
        }

        public string Sentence { get; }
        public string CacheKey => Sentence;
    }
}
