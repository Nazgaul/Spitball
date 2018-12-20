using System;

namespace Cloudents.Core.Entities.DocumentDb
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CollectionIdAttribute : Attribute
    {
        public CollectionIdAttribute(string collectionId)
        {
            CollectionId = collectionId;
        }

        public string CollectionId { get; }
    }
}