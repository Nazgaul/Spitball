﻿using System.Collections.Generic;
using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class ReputationData : DomainProcess
    {
        protected ReputationData()
        {
        }

        
        public ReputationData(long userId)
        {
            UserIds = new[] { userId};
        }

        public ReputationData(IEnumerable<long> userIds)
        {
            UserIds = userIds;
        }

        [ProtoMember(1)]
        public IEnumerable<long> UserIds { get; private set; }
        public override string ProcessResolver
        {
            get { return ReputationResolver; }
        }
    }

    [ProtoContract]
    public class QuotaData : DomainProcess
    {
        protected QuotaData()
        {
        }


        public QuotaData(long userId)
        {
            UserIds = new[] { userId };
        }

        public QuotaData(IEnumerable<long> userIds)
        {
            UserIds = userIds;
        }

        [ProtoMember(1)]
        public IEnumerable<long> UserIds { get; private set; }
        public override string ProcessResolver
        {
            get { return ReputationResolver; }
        }
    }
}
