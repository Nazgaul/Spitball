using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.QueryHandler;
using Cloudents.Core.Storage;
using Cloudents.Infrastructure.Data.Query;

namespace ConsoleApp
{
    public class AuditPopulation
    {
        private readonly IContainer _container;

        public AuditPopulation(IContainer container)
        {
            _container = container;
        }

        public  async Task CreateAuditOnExistingDataAsync()
        {
            using (var child = _container.BeginLifetimeScope())
            {
                using (var t = child.Resolve<IUnitOfWork>())
                {
                    await CreateUserAuditAsync(child);
                    await t.CommitAsync(default);
                }
            }

            using (var child = _container.BeginLifetimeScope())
            {
                using (var t = child.Resolve<IUnitOfWork>())
                {
                    await CreateQuestionAuditAsync(child);
                    await t.CommitAsync(default);
                }
            }

            using (var child = _container.BeginLifetimeScope())
            {
                using (var t = child.Resolve<IUnitOfWork>())
                {
                    await CreateAnswerAuditAsync(child);
                    await t.CommitAsync(default);
                }
            }

            using (var child = _container.BeginLifetimeScope())
            {
                using (var t = child.Resolve<IUnitOfWork>())
                {
                    await MarkAnswerAuditAsync(child);
                    await t.CommitAsync(default);
                }
            }
        }

        private  async Task CreateUserAuditAsync(ILifetimeScope container)
        {
            var t = container.Resolve<IUserRepository>();
            var users = await t.GetAllUsersAsync(default);

            foreach (var user1 in users)
            {
                var command = new CreateUserCommand(user1);
                await CreateAuditAsync(command, container);
            }
        }

        private  async Task CreateQuestionAuditAsync(ILifetimeScope container)
        {
            var t = container.Resolve<IQuestionRepository>();
            var blob = container.Resolve<IBlobProvider<QuestionAnswerContainer>>();
            var users = await t.GetAllQuestionsAsync();

            foreach (var user1 in users)
            {
                var blobs = await blob.FilesInDirectoryAsync($"question/{user1.Id}", default);
                var xxx = QuestionDetailQueryHandler.AggregateFiles(blobs);
                var command = new CreateQuestionCommand()
                {
                    Text = user1.Text,
                    Files = xxx[null].Select(s => s.Segments.Last()),
                    Price = user1.Price,
                    UserId = user1.User.Id,
                    SubjectId = user1.Subject.Id
                };
                await CreateAuditAsync(command, container);
                //user1.AddTransaction(ActionType.SignUp, TransactionType.Awarded, 100);
            }
        }

        private  async Task CreateAnswerAuditAsync(ILifetimeScope container)
        {
            var t = container.Resolve<IQuestionRepository>();
            var blob = container.Resolve<IBlobProvider<QuestionAnswerContainer>>();
            var questions = await t.GetAllQuestionsAsync();

            foreach (var question in questions)
            {
                foreach (var answer in question.Answers)
                {
                    var blobs = await blob.FilesInDirectoryAsync($"question/{question.Id}", default);
                    var xxx = QuestionDetailQueryHandler.AggregateFiles(blobs);

                    var files = xxx[answer.Id].Select(s => s.Segments.Last());
                    var command = new CreateAnswerCommand(question.Id, answer.Text, answer.User.Id, files);
                    await CreateAuditAsync(command, container);
                }
            }
        }

        private  async Task MarkAnswerAuditAsync(ILifetimeScope container)
        {
            var t = container.Resolve<IQuestionRepository>();
            var questions = await t.GetAllQuestionsAsync();

            foreach (var question in questions)
            {
                var ca = question.CorrectAnswer;
                if (ca != null)
                {
                    var command = new MarkAnswerAsCorrectCommand(ca.Id, ca.User.Id);
                    await CreateAuditAsync(command, container);
                }
            }
        }

        private  async Task CreateAuditAsync(ICommand message, ILifetimeScope container)
        {
            var repository = container.Resolve<IRepository<Audit>>();
            var audit = new Audit(message);
            await repository.AddAsync(audit, default);
        }
    }
}