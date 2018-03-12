using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Cloudents.Core.Entities.Db
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class UrlStats
    {
        // ReSharper disable once MemberCanBePrivate.Global nhibernate proxy
        protected UrlStats()
        {
            AggregateCount = 1;
        }

        public UrlStats(string host, DateTime dateTime,
            string urlTarget, string urlSource, int? sourceLocation, string ip)
            :this()
        {
            Host = host;
            DateTime = dateTime;
            UrlTarget = urlTarget;
            UrlSource = urlSource;
            SourceLocation = sourceLocation;
            Ip = ip;
        }

        public virtual Guid Id { get; [UsedImplicitly] protected set; }
        public virtual string Host { get; [UsedImplicitly] protected set; }
        public virtual DateTime DateTime { get; [UsedImplicitly] protected set; }
        public virtual string UrlTarget { get; [UsedImplicitly] protected set; }
        public virtual string UrlSource { get; [UsedImplicitly] protected set; }
        public virtual int? SourceLocation { get; [UsedImplicitly] protected set; }
        public virtual int AggregateCount { get; [UsedImplicitly] protected set; }

        public virtual string Ip { get; [UsedImplicitly] protected set; }
    }
}
