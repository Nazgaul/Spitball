using System;
using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class StoreOrderData
    {
        protected StoreOrderData()
        {

        }
        public StoreOrderData(long prodcutId, string identificationNumber, string firstName, string lastName,
            string address, string cardHolderIdentificationNumber, string notes, string city, string creditCardNameHolder, string creditCardNumber, DateTime creditCardExpiration, string cvv, int universityId, string p1, string v1, string p2, string v2, string p3,
            string v3, string p4, string v4, string p5, string v5, string p6, string v6, string email, string phone, string phone2, float extraFeaturePrice, int numberOfPayment)
        {
            ProdcutId = prodcutId;
            IdentificationNumber = identificationNumber;
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            CardHolderIdentificationNumber = cardHolderIdentificationNumber;
            Notes = notes;
            City = city;
            CreditCardNameHolder = creditCardNameHolder;
            CreditCardNumber = creditCardNumber;
            CreditCardExpiration = creditCardExpiration;
            Cvv = cvv;
            // DeliveryPrice = deliveryPrice;
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
            ExtraFeaturePrice = extraFeaturePrice;
            NumberOfPayment = numberOfPayment;
        }
        [ProtoMember(1)]
        public long ProdcutId { get; private set; }

        [ProtoMember(2)]
        public string IdentificationNumber { get; private set; } //form

        [ProtoMember(3)]
        public string FirstName { get; private set; }//form

        [ProtoMember(4)]
        public string LastName { get; private set; }//form

        [ProtoMember(5)]
        public string Address { get; private set; }//form

        [ProtoMember(6)]
        public string CardHolderIdentificationNumber { get; private set; }//form

        [ProtoMember(7)]
        public string Notes { get; private set; }//form

        [ProtoMember(8)]
        public string City { get; private set; }//form

        [ProtoMember(9)]
        public string CreditCardNameHolder { get; private set; }//form

        [ProtoMember(10)]
        public string CreditCardNumber { get; private set; }//form

        [ProtoMember(11)]
        public DateTime CreditCardExpiration { get; private set; }//form

        [ProtoMember(12)]
        public string Cvv { get; private set; }//form

        [ProtoMember(13)]
        public int UniversityId { get; private set; }

        [ProtoMember(14)]
        public string V1 { get; private set; }

        [ProtoMember(15)]
        public string P2 { get; private set; }

        [ProtoMember(16)]
        public string V2 { get; private set; }

        [ProtoMember(17)]
        public string P3 { get; private set; }

        [ProtoMember(18)]
        public string V3 { get; private set; }

        [ProtoMember(19)]
        public string P4 { get; private set; }

        [ProtoMember(20)]
        public string V4 { get; private set; }

        [ProtoMember(21)]
        public string P5 { get; private set; }

        [ProtoMember(22)]
        public string V5 { get; private set; }

        [ProtoMember(23)]
        public string P6 { get; private set; }

        [ProtoMember(24)]
        public string V6 { get; private set; }

        [ProtoMember(25)]
        public string P1 { get; private set; }

        [ProtoMember(26)]
        public string Email { get; private set; }//form

        [ProtoMember(27)]
        public string Phone { get; private set; }//form

        [ProtoMember(28)]
        public string Phone2 { get; private set; }//form
        //public float TotalPrice { get; private set; }

        [ProtoMember(29)]
        public float ExtraFeaturePrice { get; private set; }

        [ProtoMember(30)]
        public int NumberOfPayment { get; private set; }//form
    }
}
