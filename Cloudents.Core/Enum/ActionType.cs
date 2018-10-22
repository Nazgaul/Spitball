using Cloudents.Core.Attributes;
using Cloudents.Core.Enum.Resources;

namespace Cloudents.Core.Enum
{
    public enum ActionType
    {
        [ResourceDescription(typeof(EnumResources), "ActionTypeNone")]
        None,
        [ResourceDescription(typeof(EnumResources), "ActionTypeSignUp")]
        SignUp,
        [ResourceDescription(typeof(EnumResources), "ActionTypeQuestion")]
        Question,
        [ResourceDescription(typeof(EnumResources), "ActionTypeDeleteQuestion")]
        DeleteQuestion,
        [ResourceDescription(typeof(EnumResources), "ActionTypeAnswerCorrect")]
        AnswerCorrect,
        [ResourceDescription(typeof(EnumResources), "ActionTypeCashOut")]
        CashOut,
        [ResourceDescription(typeof(EnumResources), "ActionTypeReferringUser")]
        ReferringUser
    }
}