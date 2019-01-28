using System.Collections.Generic;
using System.Linq;
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
        internal IList<Transaction> Transactions { get; private set; }

        public IReadOnlyList<Transaction> TransactionsReadOnly => Transactions.ToList();

        public decimal Balance { get; private set; }

        public int Score { get; private set; }

        public void Add(Transaction t)
        {
            Transactions.Add(t);
            Balance += t.TransactionType.Price;
            if (t.TransactionType.Price > 0)
            {
                Score += (int)t.TransactionType.Price;
            }
        }

        public void UpdateBalance(decimal balance, int score)
        {
            Balance = balance;
            Score = score;
        }

    }
}