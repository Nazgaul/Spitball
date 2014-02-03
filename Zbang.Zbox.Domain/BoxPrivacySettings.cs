using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Domain
{
    public enum BoxPrivacySettings
    {
        NotShared = 0,
        Public = 1,
        InvitationOnly = 2,
        PasswordProtected = 3
    }
}
