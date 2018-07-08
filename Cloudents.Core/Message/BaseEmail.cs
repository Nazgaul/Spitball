using System;

namespace Cloudents.Core.Message
{
    [Serializable]
    public abstract class BaseEmail
    {
        protected BaseEmail(string to, string template, string subject)
        {
            To = to;
            Template = template;
            Subject = subject;
        }

        public string To { get; private set; }

        public string Template { get; private set; }
        public string Subject { get; private set; }
    }

    [Serializable]
    public class BlockChainInitialBalance
    {
        public BlockChainInitialBalance(string publicAddress)
        {
            PublicAddress = publicAddress;
        }

        public string PublicAddress { get; set; }
    }
}