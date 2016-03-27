using System;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateGoogleUserCommand : CreateUserCommand
    {
        public const string ResolveName = "Google";
        public CreateGoogleUserCommand(string email, string googleId, string image, long? universityId, string firstName, string lastName, string culture,Sex sex, Guid? inviteId = null, long? boxId = null)
            : base(email, universityId, firstName, lastName, culture, inviteId, boxId, sex)
        {
            GoogleId = googleId;
            Image = image;
        }

        public string GoogleId { get; private set; }
        public string Image { get; private set; }
        public override string CommandResolveName
        {
            get { return ResolveName; }
        }
    }
}