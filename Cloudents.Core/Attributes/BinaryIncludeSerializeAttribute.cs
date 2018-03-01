using System;

namespace Cloudents.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class BinaryIncludeSerializeAttribute : Attribute
    {
        public BinaryIncludeSerializeAttribute(Type knownType, int tag)
        {
            KnownType = knownType;
            Tag = tag;
        }

        public Type KnownType { get; }

        public int Tag { get; private set; }
    }
}
