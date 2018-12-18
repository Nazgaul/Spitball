using Cloudents.Common.Attributes;
using Cloudents.Common.Resources;

namespace Cloudents.Common.Enum
{
    public enum TransactionActionType
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
        ReferringUser,
        [ResourceDescription(typeof(EnumResources), "ActionTypeAwarded")]
        Awarded,
        [ResourceDescription(typeof(EnumResources), "ActionTypeFirstCourse")]
        FirstCourse,
    }
}