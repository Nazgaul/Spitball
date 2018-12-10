using System;
using System.Collections.Generic;
using Cloudents.Core.Enum;

namespace Cloudents.Core
{
    public static class ReputationSystem
    {
        private static readonly SortedSet<string> Tier1Users =
            new SortedSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "US", "CA", "AU" , "GB", "IE", "IL", "NZ", "MX", "SE" ,
                "NO", "DK", "FI", "NL", "BE","LU","DE","CH","AT","ZA"
            };
        public static decimal InitBalance(string country)
        {
            var initBalance = 100;
            if (Tier1Users.Contains(country))
            {
                initBalance = 200;
            }

            return initBalance;
        }

        public const decimal University = 5;
        public const decimal Course = 5;
        public const decimal ReferringUser = 10;
        public const decimal Retention = 1;
        public const decimal PostItem = 1;

        public const decimal AutoPostValue = 100;

        public static ItemState GetItemState(int score)
        {
            if (score < ReputationSystem.AutoPostValue)
            {
                return ItemState.Pending;
            }

            return ItemState.Ok;
        }
    }
}