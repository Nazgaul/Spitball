using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class StoreOrderData : StoreData
    {
        protected StoreOrderData()
        {

        }

        public StoreOrderData(long prodcutId, string identificationNumber, string firstName, string lastName, string address, string cardHolderIdentificationNumber, string notes, string city, string creditCardNameHolder, string creditCardNumber, DateTime creditCardExpiration, string cvv, int universityId, string email, string phone, string phone2, IList<int> features, int numberOfPayment)
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
            UniversityId = universityId;
            Email = email;
            Phone = phone;
            Phone2 = phone2;
            Features = features;
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

        [ProtoMember(26)]
        public string Email { get; private set; }//form

        [ProtoMember(27)]
        public string Phone { get; private set; }//form

        [ProtoMember(28)]
        public string Phone2 { get; private set; }//form

        [ProtoMember(29)]
        public IList<int> Features { get; private set; }

        [ProtoMember(30)]
        public int NumberOfPayment { get; private set; }//form
    }
}
