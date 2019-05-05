using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Cloudents.Core
{
    public class PrioritySource
    {
        public string Source { get; }
        public float Priority { get; }

        public IEnumerable<string> Domains { get; }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "We need that for serialization")]
        protected PrioritySource()
        {

        }

        //TODO: only public because of unit test
        public PrioritySource(string source, float priority)

        {
            Source = source;
            Priority = priority;
        }

        private PrioritySource(string source, float priority, IEnumerable<string> domains)

        {
            Source = source;
            Priority = priority;
            Domains = domains;
        }

        public static readonly PrioritySource TutorWyzant = new PrioritySource("Wyzant", 1);
        public static readonly PrioritySource TutorMe = new PrioritySource("TutorMe", 1.33f);
    }
}