﻿using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class ChangeEmailData : BaseMailData
    {
        protected ChangeEmailData()
        {

        }
        public ChangeEmailData(string code, string emailAddress, string culture)
            : base(emailAddress, culture)
        {
            Code = code;
        }
        [ProtoMember(1)]
        public string Code { get; private set; }

        public override string MailResover => ChangeEmailResolver;
    }
}
