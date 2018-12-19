﻿
namespace Cloudents.Application.Storage
{
    public sealed class TopicSubscription
    {
        public string Topic { get; }
        public string Subscription { get; }

        public const string Communication = nameof(Communication);
        public const string Background = nameof(Background);

        private TopicSubscription(string topic, string subscription)
        {
            Topic = topic;
            Subscription = subscription;
        }

        //public static readonly TopicSubscription Email = new TopicSubscription(Communication, nameof(Email));
        //public static readonly TopicSubscription Sms = new TopicSubscription(Communication, nameof(Sms));


        public static readonly TopicSubscription BlockChainInitialBalance = new TopicSubscription(Background, nameof(BlockChainInitialBalance));
        public static readonly TopicSubscription BlockChainQnA = new TopicSubscription(Background, nameof(BlockChainQnA));
        public static readonly TopicSubscription UrlRedirect = new TopicSubscription(Background, nameof(UrlRedirect));
    }
}