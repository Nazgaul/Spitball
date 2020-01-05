using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]

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