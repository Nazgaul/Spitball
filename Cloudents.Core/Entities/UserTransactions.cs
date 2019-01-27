﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Cloudents.Persistance")]
namespace Cloudents.Core.Entities
{
    public sealed class UserTransactions
    {
        public UserTransactions()
        {
            Transactions = Transactions ?? new List<Transaction>();
        }
        internal ICollection<Transaction> Transactions { get; private set; }
        public decimal Balance { get; private set; }

        public int Score { get; private set; }

        public void Add(Transaction t)
        {
            Transactions.Add(t);
            Balance += t.Price;
            if (t.Price > 0)
            {
                Score += (int)t.Price;
            }
        }

        public void UpdateBalance(decimal balance, int score)
        {
            Balance = balance;
            Score = score;
        }

    }
}