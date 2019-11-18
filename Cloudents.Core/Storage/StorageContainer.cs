
using System;

namespace Cloudents.Core.Storage
{
    [Serializable]
    public sealed class StorageContainer : IEquatable<StorageContainer>
    {
        private StorageContainer(string name, string relativePath)
        {
            Name = name.ToLowerInvariant();
            RelativePath = relativePath;
        }

        public string Name { get; }
        public string RelativePath { get; }

        //public static readonly StorageContainer QuestionsAndAnswers = new StorageContainer("spitball-files","question");
        public static readonly StorageContainer Chat = new StorageContainer("spitball-user", "chat");
        public static readonly StorageContainer File = new StorageContainer("spitball-files", "files");
        public static readonly StorageContainer User = new StorageContainer("spitball-user", "profile");

        public bool Equals(StorageContainer other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) && string.Equals(RelativePath, other.RelativePath, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is StorageContainer other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (StringComparer.OrdinalIgnoreCase.GetHashCode(Name) * 397) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(RelativePath);
            }
        }

        public static bool operator ==(StorageContainer left, StorageContainer right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StorageContainer left, StorageContainer right)
        {
            return !Equals(left, right);
        }
    }
}
