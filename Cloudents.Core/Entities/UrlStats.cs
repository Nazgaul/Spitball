//using System;
//using System.Diagnostics.CodeAnalysis;

//namespace Cloudents.Core.Entities
//{
//    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
//    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
//    public class UrlStats
//    {
//        // ReSharper disable once MemberCanBePrivate.Global nhibernate proxy
//        protected UrlStats()
//        {
//            AggregateCount = 1;
//        }

//        public UrlStats(string host, DateTime dateTime,
//            string urlTarget, string urlSource, int? sourceLocation, string ip)
//            :this()
//        {
//            Host = host;
//            DateTime = dateTime;
//            UrlTarget = urlTarget;
//            UrlSource = urlSource;
//            SourceLocation = sourceLocation;
//            Ip = ip;
//        }

//        public virtual Guid Id { get;  protected set; }
//        public virtual string Host { get;  protected set; }
//        public virtual DateTime DateTime { get;  protected set; }
//        public virtual string UrlTarget { get;  protected set; }
//        public virtual string UrlSource { get;  protected set; }
//        public virtual int? SourceLocation { get;  protected set; }
//        public virtual int AggregateCount { get;  protected set; }

//        public virtual string Ip { get;  protected set; }
//    }
//}
