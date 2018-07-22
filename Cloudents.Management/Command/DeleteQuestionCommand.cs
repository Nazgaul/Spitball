using System.ComponentModel.DataAnnotations;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Cloudents.Management.Command
{
    public class DeleteQuestionCommand : ICommand
    {
        public DeleteQuestionCommand(long questionId)
        {
            QuestionId = questionId;
        }

        public long QuestionId { get;}

    }

   
}