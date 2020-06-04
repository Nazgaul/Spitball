//using System.Collections.Generic;

//namespace Cloudents.Core.Entities
//{
//    public class TutorPrice : ValueObject
//    {
//        public TutorPrice(decimal price)
//        {
//            Price = price;
//            SubsidizedPrice = null;
//        }
//        public TutorPrice(decimal price, decimal subsidizedPrice)
//        {
//            Price = price;
//            SubsidizedPrice = subsidizedPrice;
//        }

//        protected TutorPrice()
//        {

//        }

//        public virtual decimal? Price { get; protected set; }
//        public virtual decimal? SubsidizedPrice { get; protected set; }

//        public virtual decimal GetPrice() => SubsidizedPrice ?? Price ?? 0M;



//        protected override IEnumerable<object> GetEqualityComponents()
//        {
//            yield return Price.GetValueOrDefault();
//            yield return SubsidizedPrice.GetValueOrDefault();
//        }
//    }
//}