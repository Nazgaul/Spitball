using System;

namespace Zbang.Zbox.Infrastructure.Repositories
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DocumentDbModelAttribute : Attribute
    {
        public DocumentDbModelAttribute(string collectionId)
        {
            CollectionId = collectionId;
        }

        public string CollectionId { get; private set; }
    }
}