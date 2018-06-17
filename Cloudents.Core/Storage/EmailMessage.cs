
namespace Cloudents.Core.Storage
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

        public static readonly TopicSubscription Email = new TopicSubscription(Communication, nameof(Email));
        public static readonly TopicSubscription Sms = new TopicSubscription(Communication, nameof(Sms));
        public static readonly TopicSubscription TalkJs = new TopicSubscription(Background, nameof(TalkJs));
    }
}