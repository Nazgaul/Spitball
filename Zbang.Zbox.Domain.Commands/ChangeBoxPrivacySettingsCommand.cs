using System.Runtime.Serialization;

using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.Commands
{    
    [DataContract]
    public class ChangeBoxPrivacySettingsCommand : ICommand
    {
        public ChangeBoxPrivacySettingsCommand(long userId, long boxId, BoxPrivacySettings newSettings, string password)
        {
            NewSettings = newSettings;
            BoxId = boxId;
            UserId = userId;
            SharePassword = password;
        }

        //Properties
        [DataMember]
        public long BoxId { get; set; }

        [DataMember]
        public long UserId { get; set; }

        [DataMember]
        public BoxPrivacySettings NewSettings { get; set; }

        [DataMember]
        public string SharePassword { get; set; }      
    }
}
