using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Enums
{
    public enum ReputationAction
    {
        None = 0,
        AddItem = 1,
        AddAnswer = 2,
        AddQuestion = 3,
        DeleteItem = 100,
        DeleteQuestion = 101,
        DeleteAnswer = 102,
        ShareFacebook = 7,
        Invite =8,
        InviteToBox = 9,
        Rate3Stars = 10,
        Rate4Stars = 11,
        Rate5Stars = 12,
        UnRate3Stars = 103,
        UnRate4Stars = 104,
        UnRate5Stars = 105
        


    }
}
