namespace Cloudents.Infrastructure
{
    public class SearchServiceCredentials
    {
        public SearchServiceCredentials(string name, string key)
        {
            Name = name;
            Key = key;
        }

        public string Name { get; }
        public string Key { get; }
    }
}