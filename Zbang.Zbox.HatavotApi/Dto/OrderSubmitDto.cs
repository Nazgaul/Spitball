using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Store.Dto
{
    public class OrderSubmitDto
    {
        public long ProdcutId { get; private set; }
        public object studentnum { get; private set; }
        public object dfname { get; private set; }
        public object dlname { get; private set; }
        public object daddress1 { get; private set; }
        public object HouseNumber { get; private set; }
        public float coupon { get; private set; }
        public object ids { get; private set; }
        public object Notes { get; private set; }
        public object city { get; private set; }
        public object zip { get; private set; }
        public object ccname { get; private set; }
        public object ccNumber { get; private set; }
        public DateTime ccExpire { get; private set; }
        public object cvv { get; private set; }
        public object mosadName { get; private set; }

// should be 0 if empty
    }
}
