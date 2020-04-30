//using Cloudents.Core.Enum;
//using Cloudents.Core.Extension;
//using System;

//namespace Cloudents.Core.DTOs
//{
//    public class TransactionDto
//    {
//        private TransactionActionType _action;
//        private TransactionType _type;
//        public DateTime Date { get; set; }

//        public string Action
//        {
//            get => _action.GetEnumLocalization();
//            set
//            {
//                if (System.Enum.TryParse(value, out TransactionActionType p))
//                {
//                    _action = p;
//                }
//            }
//        }

//        public string Type
//        {
//            get => _type.GetEnumLocalization();
//            set
//            {
//                if (System.Enum.TryParse(value, out TransactionType p))
//                {
//                    _type = p;
//                }
//            }
//        }
//        public decimal Amount { get; set; }


//        //public string Action => ActionType.GetEnumLocalization();
//        //public string Type => TypeEnum.GetEnumLocalization();



//        public decimal Balance { get; set; }
//    }
//}