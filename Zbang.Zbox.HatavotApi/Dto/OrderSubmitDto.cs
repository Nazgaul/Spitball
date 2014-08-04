using System;

namespace Zbang.Zbox.Store.Dto
{
    public class OrderSubmitDto
    {
        public OrderSubmitDto(long prodcutId, string identificationNumber, string firstName, string lastName, string address, float couponValue, string cardHolderIdentificationNumber, string notes, string city, string creditCardNameHolder, string creditCardNumber, DateTime creditCardExpiration, string cvv, object deliveryPrice, int universityId, string v1, string p2, string v2, string p3, string v3, string p4, string v4, string p5, string v5, string p6, string v6, string p1, object email, object phone, object phone2, float totalPrice, float extraFeaturePrice, int numberOfPayment)
        {
            ProdcutId = prodcutId;
            IdentificationNumber = identificationNumber;
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            CouponValue = couponValue;
            CardHolderIdentificationNumber = cardHolderIdentificationNumber;
            Notes = notes;
            City = city;
            CreditCardNameHolder = creditCardNameHolder;
            CreditCardNumber = creditCardNumber;
            CreditCardExpiration = creditCardExpiration;
            Cvv = cvv;
            DeliveryPrice = deliveryPrice;
            UniversityId = universityId;
            V1 = v1;
            P2 = p2;
            V2 = v2;
            P3 = p3;
            V3 = v3;
            P4 = p4;
            V4 = v4;
            P5 = p5;
            V5 = v5;
            P6 = p6;
            V6 = v6;
            P1 = p1;
            Email = email;
            Phone = phone;
            Phone2 = phone2;
            TotalPrice = totalPrice;
            ExtraFeaturePrice = extraFeaturePrice;
            NumberOfPayment = numberOfPayment;
        }

        public long ProdcutId { get; private set; }
        public string IdentificationNumber { get; private set; } //form
        public string FirstName { get; private set; }//form
        public string LastName { get; private set; }//form
        public string Address { get; private set; }//form
        public float CouponValue { get; private set; }
        public string CardHolderIdentificationNumber { get; private set; }//form
        public string Notes { get; private set; }//form
        public string City { get; private set; }//form
        public string CreditCardNameHolder { get; private set; }//form
        public string CreditCardNumber { get; private set; }//form
        public DateTime CreditCardExpiration { get; private set; }//form
        public string Cvv { get; private set; }//form
        public object DeliveryPrice { get; private set; }

        public int UniversityId { get; private set; }
        public string V1 { get; private set; }
        public string P2 { get; private set; }
        public string V2 { get; private set; }
        public string P3 { get; private set; }
        public string V3 { get; private set; }
        public string P4 { get; private set; }
        public string V4 { get; private set; }
        public string P5 { get; private set; }
        public string V5 { get; private set; }
        public string P6 { get; private set; }
        public string V6 { get; private set; }
        public string P1 { get; private set; }
        public object Email { get; private set; }//form
        public object Phone { get; private set; }//form
        public object Phone2 { get; private set; }//form
        public float TotalPrice { get; private set; }
        public float ExtraFeaturePrice { get; private set; }
        public int NumberOfPayment { get; private set; }//form
    }
}