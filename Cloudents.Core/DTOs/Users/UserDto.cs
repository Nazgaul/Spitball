﻿using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Cloudents.Core.DTOs.Users
{
    public class UserDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Image { get; set; }
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global" , Justification = "Dto json serialize")]
    public class UserAccountDto
    {
        [NonSerialized]
        public bool _needPayment;

        [NonSerialized]
        public Country? Country;
        private ItemState? _isTutor;


        public decimal Balance { get; set; }

        public string Email { get; set; }
        //public string PhoneNumber { get; set; }
        public long Id { get; set; }
      
        public string FirstName { get; set; }
        public string? LastName { get; set; }

        public string? Image { get; set; }
        public int? ChatUnread { get; set; }
        public ItemState? IsTutor
        {
            get
            {
                if (_isTutor == null)
                {
                    return null;
                }
                
                return ItemState.Ok;
            }
            set => _isTutor = value;
        }

        public bool TutorSubscription { get; set; }

        public bool NeedPayment
        {
            get
            {
                if (Country == Country.India)
                {
                    return false;
                }

                return _needPayment;
            } 
        }

        public bool IsSold { get; set; }
       


        public string CurrencySymbol => (Country ?? Country.UnitedStates).RegionInfo.ISOCurrencySymbol;
    }
}