using System.ComponentModel.DataAnnotations;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Cloudents.Management.Command
{
    public class DeleteQuestionCommand : ICommand
    {
        [Required,Range(1,long.MaxValue)]
        public long QuestionId { get; set; }

        [BindNever] public long UserId { get; set; }
    }

    public class UpdateUserBalanceCommand : ICommand
    {
        public UpdateUserBalanceCommand(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }
    }
}