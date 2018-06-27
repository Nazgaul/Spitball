﻿using System;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Interfaces;

namespace ConsoleApp
{
    public class TransactionPopulation
    {
        private IContainer _container;

        public TransactionPopulation(IContainer container)
        {
            _container = container;
        }

        public async Task CreateTransactionOnExistingData()
        {
            using (var child = _container.BeginLifetimeScope())
            {
                using (var t = child.Resolve<IUnitOfWork>())
                {
                    await CreateUserAudit(child);
                    await t.CommitAsync(default);
                }
            }

            using (var child = _container.BeginLifetimeScope())
            {
                using (var t = child.Resolve<IUnitOfWork>())
                {
                    await CreateQuestionAudit(child);
                    await t.CommitAsync(default);
                }
            }

            using (var child = _container.BeginLifetimeScope())
            {
                using (var t = child.Resolve<IUnitOfWork>())
                {

                    await CreateAnswerAudit(child);
                    await t.CommitAsync(default);
                }
            }

            using (var child = _container.BeginLifetimeScope())
            {
                using (var t = child.Resolve<IUnitOfWork>())
                {
                    await MarkAnswerAudit(child);
                    await t.CommitAsync(default);
                }
            }
        }

        private async Task CreateUserAudit(ILifetimeScope container)
        {
            var t = container.Resolve<IUserRepository>();
            var users = await t.GetAllUsersAsync(default);


            foreach (var user1 in users)
            {
                user1.UserCreateTransaction();
                await t.UpdateAsync(user1, default);
                //var transaction = Transaction.UserCreateTransaction(user1);
                //await CreateTransactionAsync(transaction, container);
                //user1.AddTransaction(ActionType.SignUp, TransactionType.Awarded, 100);
            }
        }

        private async Task CreateQuestionAudit(ILifetimeScope container)
        {
            var t = container.Resolve<IQuestionRepository>();
            var questions = await t.GetAllQuestionsAsync();
            var transactionRepository = container.Resolve<ITransactionRepository>();

            foreach (var question in questions)
            {
                var tttt = question.QuestionCreateTransaction();
                await Task.Delay(TimeSpan.FromMilliseconds(1));
                await transactionRepository.AddAsync(tttt, default);
                //var transaction = Transaction.QuestionCreateTransaction(user1);
                // await CreateTransactionAsync(transaction, container);
            }
        }

        private async Task CreateAnswerAudit(ILifetimeScope container)
        {
            var t = container.Resolve<IQuestionRepository>();
            var transactionRepository = container.Resolve<ITransactionRepository>();

            var questions = await t.GetAllQuestionsAsync();

            foreach (var question in questions)
            {
                foreach (var answer in question.Answers)
                {
                    var tttt = answer.AnswerCreateTransaction();
                    await Task.Delay(TimeSpan.FromMilliseconds(1));
                    await transactionRepository.AddAsync(tttt, default);
                    //var transaction = Transaction.AnswerCreateTransaction(answer);
                    //await CreateTransactionAsync(transaction, container);
                }
                //user1.AddTransaction(ActionType.SignUp, TransactionType.Awarded, 100);
            }
        }

        private async Task MarkAnswerAudit(ILifetimeScope container)
        {
            var t = container.Resolve<IQuestionRepository>();
            var transactionRepository = container.Resolve<ITransactionRepository>();

            var questions = await t.GetAllQuestionsAsync();

            foreach (var question in questions)
            {
                var ca = question.CorrectAnswer;
                if (ca != null)
                {

                    var tttt = question.MarkCorrectTransaction();
                    await Task.Delay(TimeSpan.FromMilliseconds(1));
                    await transactionRepository.AddAsync(tttt, default);
                    //await CreateTransactionAsync(transaction, container);

                }
            }
        }

        //private async Task CreateTransactionAsync(Transaction transaction, ILifetimeScope container)
        //{
        //    //var t = container.Resolve<IUnitOfWork>();
        //    var _repository = container.Resolve<ITransactionRepository>();
        //    await _repository.AddAsync(transaction, default);
        //    //await t.CommitAsync(default);
        //}

        //private async Task CreateTransactionAsync(IEnumerable<Transaction> transaction, ILifetimeScope container)
        //{
        //    //var t = container.Resolve<IUnitOfWork>();
        //    var _repository = container.Resolve<ITransactionRepository>();
        //    await _repository.AddAsync(transaction, default);
        //    //await t.CommitAsync(default);
        //}
    }
}