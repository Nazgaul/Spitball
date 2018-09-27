using System;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Cloudents.Core.Entities.Db;

namespace Cloudents.Management.Pages
{
    public class SuspendUserModel : PageModel
    {
        private readonly Lazy<ICommandBus> _commandBus;
        private readonly IQueryBus _queryBus;

        public SuspendUserModel(Lazy<ICommandBus> commandBus, IQueryBus queryBus)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
        }

        [BindProperty] public SuspendUser Model { get; set; }

        public class SuspendUser
        {
            [Required]
            public long Id { get; set; }

            [Required]
            public bool DeleteUserQuestions { get; set; }
        }

        [ViewData]
        public string Email { get; set; }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync(CancellationToken token)
        {
            var userDataByIdQuery = new UserDataByIdQuery(Model.Id);
            var userDataTask = _queryBus.QueryAsync< User>(userDataByIdQuery, token);
            if (Model.DeleteUserQuestions)
            {
                
                var profile = await _queryBus.QueryAsync<ProfileDto>(userDataByIdQuery, token);
                foreach (var question in profile.Questions)
                {
                    var deleteQuestionCommand= new DeleteQuestionCommand(question.Id);
                    await _commandBus.Value.DispatchAsync(deleteQuestionCommand, token).ConfigureAwait(false);
                }
            }
            var command = new SuspendUserCommand(Model.Id);
            await _commandBus.Value.DispatchAsync(command, token);
            var userData = await userDataTask;
            Email = userData.Email;
            return Page();
        }
    }
}