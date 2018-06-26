using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.QueryHandler;
using Cloudents.Core.Storage;

namespace ConsoleApp
{
    public class TransactionPopulation
    {
        private IContainer container;

        public TransactionPopulation(IContainer container)
        {
            this.container = container;
        }

        public async Task CreateTransactionOnExistingData()
        {
            using (var child = container.BeginLifetimeScope())
            {
                using (var t = child.Resolve<IUnitOfWork>())
                {
                    await CreateUserAudit(child);
                    await t.CommitAsync(default);
                }
            }

            using (var child = container.BeginLifetimeScope())
            {
                using (var t = child.Resolve<IUnitOfWork>())
                {
                    await CreateQuestionAudit(child);
                    await t.CommitAsync(default);
                }
            }

            using (var child = container.BeginLifetimeScope())
            {
                using (var t = child.Resolve<IUnitOfWork>())
                {

                    await CreateAnswerAudit(child);
                    await t.CommitAsync(default);
                }
            }

            using (var child = container.BeginLifetimeScope())
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
                var transaction = Transaction.UserCreateTransaction(user1);
                await CreateTransactionAsync(transaction, container);
                //user1.AddTransaction(ActionType.SignUp, TransactionType.Awarded, 100);
            }
        }

        private async Task CreateQuestionAudit(ILifetimeScope container)
        {
            var t = container.Resolve<IQuestionRepository>();
            var users = await t.GetAllQuestionsAsync();

            foreach (var user1 in users)
            {
                var transaction = Transaction.QuestionCreateTransaction(user1);
                await CreateTransactionAsync(transaction, container);
            }
        }

        private async Task CreateAnswerAudit(ILifetimeScope container)
        {
            var t = container.Resolve<IQuestionRepository>();
            var blob = container.Resolve<IBlobProvider<QuestionAnswerContainer>>();
            var questions = await t.GetAllQuestionsAsync();

            foreach (var question in questions)
            {
                foreach (var answer in question.Answers)
                {
                    var transaction = Transaction.AnswerCreateTransaction(answer);
                    await CreateTransactionAsync(transaction, container);
                }
                //user1.AddTransaction(ActionType.SignUp, TransactionType.Awarded, 100);
            }
        }

        private async Task MarkAnswerAudit(ILifetimeScope container)
        {
            var t = container.Resolve<IQuestionRepository>();
            var blob = container.Resolve<IBlobProvider<QuestionAnswerContainer>>();
            var questions = await t.GetAllQuestionsAsync();

            foreach (var question in questions)
            {
                var ca = question.CorrectAnswer;
                if (ca != null)
                {
                    var transaction = Transaction.QuestionMarkAsCorrect(question);
                    await CreateTransactionAsync(transaction, container);

                }
            }
        }

        private async Task CreateTransactionAsync(Transaction transaction, ILifetimeScope container)
        {
            //var t = container.Resolve<IUnitOfWork>();
            var _repository = container.Resolve<ITransactionRepository>();
            await _repository.AddAsync(transaction, default);
            //await t.CommitAsync(default);
        }

        private async Task CreateTransactionAsync(IEnumerable<Transaction> transaction, ILifetimeScope container)
        {
            //var t = container.Resolve<IUnitOfWork>();
            var _repository = container.Resolve<ITransactionRepository>();
            await _repository.AddAsync(transaction, default);
            //await t.CommitAsync(default);
        }
    }
}