using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Core.Query.Admin;
using Cloudents.Core.Storage;
using Cloudents.Core.Storage.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cloudents.Management.Pages
{
    public class AddQuestionModel : PageModel
    {
        //private readonly Lazy<ICommandBus> _commandBus;
        private readonly IQueryBus _queryBus;
        private readonly IQueueProvider _queueProvider;


        public AddQuestionModel(/*Lazy<ICommandBus> commandBus,*/ IQueryBus queryBus, IQueueProvider queueProvider)
        {
            //_commandBus = commandBus;
            _queryBus = queryBus;
            _queueProvider = queueProvider;
        }

        [BindProperty] public AddQuestion Model { get; set; }

        public class AddQuestion
        {
            [Required]
            public int SubjectId { get; set; }
            [Required] public string Text { get; set; }

            [Required] public decimal Price { get; set; }
        }

        [ViewData]
        public IEnumerable<QuestionSubjectDto> Subjects { get; set; }

        public async Task OnGet(CancellationToken token)
        {
            var query = new QuestionSubjectQuery();
            Subjects = await _queryBus.QueryAsync(query, token).ConfigureAwait(false);
            //return Ok(result);
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                var query = new QuestionSubjectQuery();
                Subjects = await _queryBus.QueryAsync(query, token).ConfigureAwait(false);
                return Page();
            }


            var userId = await _queryBus.QueryAsync<long>(new AdminEmptyQuery(), token);
            var message = new NewQuestionMessage(Model.SubjectId, Model.Text, Model.Price, userId);
            await _queueProvider.InsertQuestionMessageAsync(message, token);
            //IQueueProvider
            //var command = new CreateQuestionCommand()
            //{
            //    Text = Model.Text,
            //    Price = Model.Price,
            //    SubjectId = Model.SubjectId
            //};

            //await _commandBus.Value.DispatchAsync(command, token);
            return RedirectToPage("AddQuestion");
        }
    }
}