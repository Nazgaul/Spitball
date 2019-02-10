﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Cloudents.Core.Exceptions;

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
        public IReadOnlyCollection<Transaction> TransactionsReadOnly => Transactions.ToList(); 
        public decimal Balance { get; private set; }

        public int Score { get; private set; }

        public void Add(Transaction t, RegularUser user)
        {
            t.User = user;
            Transactions.Add(t);
            Balance += t.Price;
            if (Balance < 0)
           
            {
                throw new InsufficientFundException();
            }
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