using System;
using System.Collections.Generic;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Entities.Db
{
    public class Transaction
    {
        public Transaction(User user, ActionType action, TransactionType type, decimal price)
        {
            User = user;
            Action = action;
            Type = type;
            Price = price;
            //Balance = balance;
            if (Balance < 0)
            {
                throw new InvalidOperationException("not enough tokens");
            }
            Created = DateTime.UtcNow;
        }



        //private const decimal InitialBalance = 100;

        //public static Transaction UserCreateTransaction(User user)
        //{
        //    return new Transaction(user, ActionType.SignUp, TransactionType.Awarded, InitialBalance);
        //}

        

        

        

        


        //public static IList<Transaction> QuestionMarkAsCorrect(Question question)
        //{
        //    if (question.CorrectAnswer == null)
        //    {
        //        throw new InvalidOperationException("need to have correct answer");
        //    }
        //    var transaction = new List<Transaction>();
        //    transaction.Add(new Transaction(question.User, ActionType.QuestionCorrect, TransactionType.Spent, -question.Price));
        //    transaction.Add(new Transaction(question.User, ActionType.QuestionCorrect, TransactionType.Stake, question.Price));


        //    //User.AddTransaction(ActionType.QuestionCorrect, TransactionType.Spent, -Price);
        //    //User.AddTransaction(ActionType.QuestionCorrect, TransactionType.Stake, Price);

        //    foreach (var answer in question.Answers)
        //    {
        //        if (answer.Id == question.CorrectAnswer.Id)
        //        {
        //            transaction.Add(new Transaction(answer.User, ActionType.QuestionCorrect, TransactionType.Earned, question.Price));
        //        }
        //        transaction.Add(new Transaction(answer.User, ActionType.QuestionCorrect, TransactionType.Pending, -question.Price));
        //    }
        //    return transaction;
        //}


        protected Transaction()
        {

        }

        public virtual Guid Id { get; set; }
        public virtual User User { get; set; }

        public virtual DateTime Created { get; set; }

        public virtual ActionType Action { get; set; }
        public virtual TransactionType Type { get; set; }
        public virtual decimal Price { get; set; }
        public virtual decimal Balance { get; set; }

        //public virtual Transaction NextTransaction { get; set; }

        //public virtual Transaction CreateTransaction(ActionType action, TransactionType type, decimal price)
        //{
        //    var balance = Balance + Price;
        //    //if (balance < 0)
        //    //{
        //    //    throw new InvalidOperationException("not enough tokens");
        //    //}
        //    var t = new Transaction(User, action, type, price, balance);
        //    NextTransaction = t;
        //    return NextTransaction;
        //}
    }
}