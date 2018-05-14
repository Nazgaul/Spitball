using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    public class CreateQuestionCommand : ICommand
    {
        public long SubjectId { get; set; }
        public string Text { get; set; }

        public decimal Price { get; private set; }

        public long UserId { get; set; }
    }
}