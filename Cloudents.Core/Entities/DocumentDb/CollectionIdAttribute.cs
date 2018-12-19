using System;

namespace Cloudents.Application.Entities.DocumentDb
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