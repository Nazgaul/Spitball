using System;
using System.Collections.Specialized;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateGoogleUserCommand : CreateUserCommand
    {
        public const string ResolveName = "Google";
        public CreateGoogleUserCommand(string email, string googleId, string image, string firstName, string lastName, string culture, Sex sex, NameValueCollection parameters, Guid? inviteId = null, long? boxId = null, long? universityId = null)
            : base(email, universityId, firstName, lastName, culture, inviteId, boxId, sex, parameters)
        {
            GoogleId = googleId;
            Image = image;
        }

        public string GoogleId { get; private set; }
        public string Image { get; private set; }
        public override string CommandResolveName => ResolveName;
    }
}