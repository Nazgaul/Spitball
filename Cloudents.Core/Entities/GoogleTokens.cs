namespace Cloudents.Core.Entities
{
    public class GoogleTokens
    {
        public GoogleTokens(string id, string value)
        {
            Id = id;
            Value = value;
        }

        protected GoogleTokens()
        {

        }

        public virtual string Id { get; protected set; }

        public virtual string Value { get; set; }
    }
}