using System;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
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


        public async Task UpdateBalanceAsync()
        {
            using (var child = _container.BeginLifetimeScope())
            {
               
                using (var unitOfWork = child.Resolve<IUnitOfWork>())
                {
                    var repository = child.Resolve<IUserRepository>();
                    var users = await repository.GetAllUsersAsync(default).ConfigureAwait(false);

                    foreach (var user1 in users)
                    {
                        //user1.UserCreateTransaction();
                        //await t.UpdateAsync(user1, default).ConfigureAwait(false);
                    }
                    await unitOfWork.CommitAsync(default).ConfigureAwait(false);
                }
            }
        }

        public async Task AddToUserMoney(decimal money, long userId)
        {
            using (var child = _container.BeginLifetimeScope())
            {
                using (var unitOfWork = child.Resolve<IUnitOfWork>())
                {
                    var t = child.Resolve<IUserRepository>();
                    var user = await t.LoadAsync(userId, default);
                    var p = Transaction.TestEarned(money);
                    user.AddTransaction(p);
                    await t.UpdateAsync(user, default).ConfigureAwait(false);
                    await unitOfWork.CommitAsync(default);
                }
            }
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

            using (var child = _container.BeginLifetimeScope())
            {
                using (var t = child.Resolve<IUnitOfWork>())
                {
                    await CreateQuestionAuditAsync(child).ConfigureAwait(false);
                    await t.CommitAsync(default).ConfigureAwait(false);
                }
            }

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

        private static async Task CreateQuestionAuditAsync(ILifetimeScope container)
        {
            var t = container.Resolve<IQuestionRepository>();
            var questions = await t.GetAllQuestionsAsync().ConfigureAwait(false);
            var transactionRepository = container.Resolve<IRepository<Transaction>>();

            foreach (var question in questions)
            {
                /*var tttt =*/ question.QuestionCreateTransaction();
                await Task.Delay(TimeSpan.FromMilliseconds(1)).ConfigureAwait(false);
                //await transactionRepository.AddAsync(tttt, default).ConfigureAwait(false);
            }
        }

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
            var transactionRepository = container.Resolve<IRepository<Transaction>>();

            var questions = await t.GetAllQuestionsAsync().ConfigureAwait(false);

            foreach (var question in questions)
            {
                var ca = question.CorrectAnswer;
                if (ca != null)
                {
                    /*var tttt =*/
                    question.MarkCorrectTransaction(ca);
                    await Task.Delay(TimeSpan.FromMilliseconds(1)).ConfigureAwait(false);
                    //await transactionRepository.AddAsync(tttt, default).ConfigureAwait(false);
                }
            }
        }
    }
}