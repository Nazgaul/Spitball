using System.Runtime.Serialization;

using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    [DataContract]
    public class ChangeBoxPrivacySettingsCommandResult: ICommandResult
    {
        //Ctor
        public ChangeBoxPrivacySettingsCommandResult(Box box ,bool privacyChanged)
        {
            ChangedBox = box;
            PrivacyChanged = privacyChanged;

        }

        public Box ChangedBox { get; private set; }
        public bool PrivacyChanged { get; set; }
    }
}
