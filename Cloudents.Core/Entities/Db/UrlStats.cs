using System;

namespace Cloudents.Core.Entities.Db
{
    public class UrlStats
    {
        protected UrlStats()
        {
            AggregateCount = 1;
        }

        public UrlStats(string host, DateTime dateTime,
            string urlTarget, string urlSource, int sourceLocation, string ip)
            :this()
        {
            Host = host;
            DateTime = dateTime;
            UrlTarget = urlTarget;
            UrlSource = urlSource;
            SourceLocation = sourceLocation;
            Ip = ip;
        }

        public virtual Guid Id { get; set; }
        public virtual string Host { get; set; }
        public virtual DateTime DateTime { get; set; }
        public virtual string UrlTarget { get; set; }
        public virtual string UrlSource { get; set; }
        public virtual int SourceLocation { get; set; }
        public virtual int AggregateCount { get; protected set; }

        public virtual string Ip { get; set; }
    }
}
