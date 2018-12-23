using System;
using System.Collections.Generic;

namespace Cloudents.Core
{
    public sealed class ReputationAction
    {
        private readonly decimal _amount;

        private static readonly SortedSet<string> Tier1Users =
            new SortedSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "US", "CA", "AU" , "GB", "IE", "IL", "NZ", "MX", "SE" ,
                "NO", "DK", "FI", "NL", "BE","LU","DE","CH","AT","ZA"
            };
        public static decimal FinishRegister(string country)
        {
            var initBalance = 100;
            if (Tier1Users.Contains(country))
            {
                initBalance = 150;
            }

            return initBalance;
        }


        private ReputationAction(decimal amount)
        {
            _amount = amount;
        }
        public static implicit operator decimal(ReputationAction tb)
        {
            return tb._amount;
        }

        public static readonly ReputationAction ReferringUser = new ReputationAction(10);
        public static readonly ReputationAction University = new ReputationAction(5);
        public static readonly ReputationAction FirstCourse = new ReputationAction(5);
        public static readonly ReputationAction Retention = new ReputationAction(1);
        public static readonly ReputationAction AcceptItemOwner = new ReputationAction(1);
        public static readonly ReputationAction AcceptItemUser = new ReputationAction(10);
    }
}