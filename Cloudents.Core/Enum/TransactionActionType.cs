using Cloudents.Core.Attributes;
using Cloudents.Core.Enum.Resources;

namespace Cloudents.Core.Enum
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
        [ResourceDescription(typeof(EnumResources), "ActionTypeAwarded")]
        FirstCourse,
        [ResourceDescription(typeof(EnumResources), "ActionTypePurchaseDocument")]
        PurchaseDocument,
        [ResourceDescription(typeof(EnumResources), "ActionTypeSoldDocument")]
        SoldDocument,
        [ResourceDescription(typeof(EnumResources), "ActionTypeCommission")]
        Commission,

        [ResourceDescription(typeof(EnumResources), "ActionTypeBuy")]
        Buy,
        [ResourceDescription(typeof(EnumResources), "ActionTypePurchaseSession")]
        PurchaseSession,
        [ResourceDescription(typeof(EnumResources), "ActionTypeSoldSession")]
        SoldSession
    }


}