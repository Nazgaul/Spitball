using System;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    public class CreateUrlStatsCommand : ICommand
    {
        public string Host { get; }
        public DateTime DateTime { get; }
        public string UrlTarget { get; }
        public string UrlSource { get; }
        public int? SourceLocation { get; }
        public string Ip { get; private set; }

        public CreateUrlStatsCommand(string host, DateTime dateTime, string urlTarget, string urlSource,
            int? sourceLocation, string ip)
        {
            Host = host;
            DateTime = dateTime;
            UrlTarget = urlTarget;
            UrlSource = urlSource;
            SourceLocation = sourceLocation;
            Ip = ip;
        }
    }
}
