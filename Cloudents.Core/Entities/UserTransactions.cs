using Cloudents.Core.Enum;
using Cloudents.Core.Exceptions;
using System.Collections.Generic;

//[assembly: InternalsVisibleTo("Cloudents.Persistance")]
namespace Cloudents.Core.Entities
{
    public sealed class UserTransactions
    {
        public UserTransactions()
        {
            Transactions ??= new List<Transaction>();
        }
        internal ICollection<Transaction> Transactions { get; private set; }
        public IEnumerable<Transaction> TransactionsReadOnly => Transactions;
        public decimal Balance { get; private set; }

        public int Score { get; private set; }

        public void Add(Transaction t, User user)
        {
            t.User = user;
            Transactions.Add(t);
            Balance += t.Price;
            if (Balance < 0)

            {
                throw new InsufficientFundException();
            }
            //if (t.Price > 0 && t.Type == TransactionType.Earned)
            //{
            //    Score += t.AwardScore;
            //}
            if (t.Price > 0 && t.Type == TransactionType.Earned)
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