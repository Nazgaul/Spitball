﻿using System;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Interfaces;

namespace ConsoleApp
{
    public class TransactionPopulation
    {
        private readonly IContainer _container;

        public TransactionPopulation(IContainer container)
        {
            _container = container;
        }

        public async Task CreateTransactionOnExistingDataAsync()
        {
            using (var child = _container.BeginLifetimeScope())
            {
                using (var t = child.Resolve<IUnitOfWork>())
                {
                    await CreateUserAuditAsync(child).ConfigureAwait(false);
                    await t.CommitAsync(default).ConfigureAwait(false);
                }
            }

            //using (var child = _container.BeginLifetimeScope())
            //{
            //    using (var t = child.Resolve<IUnitOfWork>())
            //    {
            //        await CreateQuestionAuditAsync(child).ConfigureAwait(false);
            //        await t.CommitAsync(default).ConfigureAwait(false);
            //    }
            //}

            //using (var child = _container.BeginLifetimeScope())
            //{
            //    using (var t = child.Resolve<IUnitOfWork>())
            //    {
            //        await CreateAnswerAuditAsync(child).ConfigureAwait(false);
            //        await t.CommitAsync(default).ConfigureAwait(false);
            //    }
            //}

            using (var child = _container.BeginLifetimeScope())
            {
                using (var t = child.Resolve<IUnitOfWork>())
                {
                    await MarkAnswerAuditAsync(child).ConfigureAwait(false);
                    await t.CommitAsync(default).ConfigureAwait(false);
                }
            }
        }

        private static async Task CreateUserAuditAsync(ILifetimeScope container)
        {
            var t = container.Resolve<IUserRepository>();
            var users = await t.GetAllUsersAsync(default).ConfigureAwait(false);

            foreach (var user1 in users)
            {
                user1.UserCreateTransaction();
                await t.UpdateAsync(user1, default).ConfigureAwait(false);
            }
        }

        //private static async Task CreateQuestionAuditAsync(ILifetimeScope container)
        //{
        //    var t = container.Resolve<IQuestionRepository>();
        //    var questions = await t.GetAllQuestionsAsync().ConfigureAwait(false);
        //    var transactionRepository = container.Resolve<ITransactionRepository>();

        //    foreach (var question in questions)
        //    {
        //        var tttt = question.QuestionCreateTransaction();
        //        await Task.Delay(TimeSpan.FromMilliseconds(1)).ConfigureAwait(false);
        //        await transactionRepository.AddAsync(tttt, default).ConfigureAwait(false);
        //    }
        //}

        //private static async Task CreateAnswerAuditAsync(ILifetimeScope container)
        //{
        //    var t = container.Resolve<IQuestionRepository>();
        //    var transactionRepository = container.Resolve<ITransactionRepository>();

        //    var questions = await t.GetAllQuestionsAsync().ConfigureAwait(false);

        //    foreach (var question in questions)
        //    {
        //        foreach (var answer in question.Answers)
        //        {
        //            var tttt = answer.AnswerCreateTransaction();
        //            await Task.Delay(TimeSpan.FromMilliseconds(1)).ConfigureAwait(false);
        //            await transactionRepository.AddAsync(tttt, default).ConfigureAwait(false);
        //        }
        //    }
        //}

        private static async Task MarkAnswerAuditAsync(ILifetimeScope container)
        {
            var t = container.Resolve<IQuestionRepository>();
            var transactionRepository = container.Resolve<ITransactionRepository>();

            var questions = await t.GetAllQuestionsAsync().ConfigureAwait(false);

            foreach (var question in questions)
            {
                var ca = question.CorrectAnswer;
                if (ca != null)
                {
                    var tttt = question.MarkCorrectTransaction(ca);
                    await Task.Delay(TimeSpan.FromMilliseconds(1)).ConfigureAwait(false);
                    await transactionRepository.AddAsync(tttt, default).ConfigureAwait(false);
                }
            }
        }
    }
}