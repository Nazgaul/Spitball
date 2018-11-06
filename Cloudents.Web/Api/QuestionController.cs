﻿using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Web.Extensions;
using Cloudents.Web.Identity;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize, ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly Lazy<ICommandBus> _commandBus;
        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<QuestionController> _localizer;

        public QuestionController(Lazy<ICommandBus> commandBus, UserManager<User> userManager,
            IStringLocalizer<QuestionController> localizer)
        {
            _commandBus = commandBus;
            _userManager = userManager;
            _localizer = localizer;
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CreateQuestionResponse>> CreateQuestionAsync([FromBody]CreateQuestionRequest model,
            [Required(ErrorMessage = "NeedCountry"), ClaimModelBinder(AppClaimsPrincipalFactory.Country)] string country,
            CancellationToken token)
        {
            try
            {
                Debug.Assert(model.SubjectId != null, "model.SubjectId != null");
                

                var command = new CreateQuestionCommand(model.SubjectId.Value, model.Text, model.Price, _userManager.GetLongUserId(User), model.Files, model.Color.GetValueOrDefault());
                await _commandBus.Value.DispatchAsync(command, token).ConfigureAwait(false);

                var toasterMessage = _localizer["PostedQuestionToasterOk"];
                if (!Language.ListOfWhiteListCountries.Contains(country))
                {
                    toasterMessage = _localizer["PostedQuestionToasterPending"];
                }

                return new CreateQuestionResponse(toasterMessage);
            }
            catch (InvalidOperationException)
            {
                ModelState.AddModelError(string.Empty, _localizer["QuestionFlood"]);
                return BadRequest(ModelState);
            }
        }

        [HttpGet("subject")]
        [ResponseCache(Duration = TimeConst.Day)]
        public IEnumerable<QuestionSubjectResponse> GetSubjectsAsync()
        {
            var values = QuestionSubjectMethod.GetValues();
            return values.Select(s => new QuestionSubjectResponse((int)s, s.GetEnumLocalization()));
        }

        [HttpPut("correct")]
        public async Task<IActionResult> MarkAsCorrectAsync([FromBody]MarkAsCorrectRequest model, CancellationToken token)
        {
            // var link = Url.Link("WalletRoute", null);
            var command = new MarkAnswerAsCorrectCommand(model.AnswerId, _userManager.GetLongUserId(User));

            await _commandBus.Value.DispatchAsync(command, token).ConfigureAwait(false);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<QuestionDetailDto>> GetQuestionAsync(long id,
            [FromServices] IQueryBus bus, CancellationToken token)
        {
            var retVal = await bus.QueryAsync(new QuestionDataByIdQuery(id), token).ConfigureAwait(false);
            if (retVal == null)
            {
                return NotFound();
            }
            return retVal;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> DeleteQuestionAsync([FromRoute]DeleteQuestionRequest model, CancellationToken token)
        {
            try
            {
                var command = new DeleteQuestionCommand(model.Id, _userManager.GetLongUserId(User));
                await _commandBus.Value.DispatchAsync(command, token).ConfigureAwait(false);
                return Ok();
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }

        // [AllowAnonymous, HttpGet(Name = "QuestionSearch")]
        //private async Task<ActionResult<WebResponseWithFacet<QuestionDto>>> GetQuestionsAsync2([FromQuery]GetQuestionsRequest model,
        //    CancellationToken token)
        //{
        //    var query = new QuestionsQuery(model.Term, model.Source, model.Page.GetValueOrDefault(), model.Filter?.Where(w => w.HasValue).Select(s => s.Value));
        //    var result = await _questionSearch.SearchAsync(query, token);
        //    string nextPageLink = null;
        //    if (result.Result.Any())
        //    {
        //        nextPageLink = Url.NextPageLink("QuestionSearch", null, model);
        //    }



        //    return new WebResponseWithFacet<QuestionDto>
        //    {
        //        Result = result.Result,
        //        Filters = new IFilters[]
        //        {
        //            new Filters<string>(nameof(GetQuestionsRequest.Filter),_localizer["FilterTypeTitle"], result.FacetState.Select(s=> new KeyValuePair<string, string>(s.ToString("G"),s.GetEnumLocalization()))),
        //            new Filters<int>(nameof(GetQuestionsRequest.Source),_localizer["SubjectTypeTitle"], result.FacetSubject)
        //        },
        //        NextPageLink = nextPageLink
        //    };
        //}



        [AllowAnonymous, HttpGet(Name = "QuestionSearch")]
        public async Task<ActionResult<WebResponseWithFacet<QuestionDto>>> GetQuestionsAsync(
            [FromQuery]GetQuestionsRequest model,
            [FromServices] IQueryBus queryBus,
           CancellationToken token)
        {

            var resultTask = new List<Task<IEnumerable<QuestionDto>>>();

            QuestionFilter[] filters;
            if (model.Filter == null || model.Filter.Length == 0)
            {
                filters = new[] { QuestionFilter.All };
            }
            else
            {
                filters = model.Filter.Where(w => w.HasValue).Select(s => s.Value).ToArray();
            }



            foreach (var filter in filters)
            {
                var query = new QuestionsQuery(model.Term, model.Source, model.Page.GetValueOrDefault(), filter);
                Task<IEnumerable<QuestionDto>> t = queryBus.QueryAsync(query, token);
                resultTask.Add(t);
            }

            //var querySubject = new QuestionSubjectQuery();
            //var subjects = await queryBus.QueryAsync(querySubject, token).ConfigureAwait(false);

            var results = await Task.WhenAll(resultTask);

            var result = results.SelectMany(s => s)
                .Distinct(new QuestionDtoEqualityComparer())
                //.Select(s=>
                //{
                //    s.Subject = subjects.FirstOrDefault(w=>w.Id ==s.SubjectId)?.Subject;
                //    return s;
                //})
                .OrderByDescending(o => o.DateTime).ToList();

            string nextPageLink = null;
            if (result.Any())
            {
                nextPageLink = Url.NextPageLink("QuestionSearch", null, model);
            }

            var values = EnumExtension.GetValues<QuestionFilter>();
            var facets = values.Where(w => w.GetAttributeValue<PublicValueAttribute>() != null).ToArray();//.Select(s => s.GetEnumLocalization());


            return new WebResponseWithFacet<QuestionDto>
            {
                Result = result,
                Filters = new IFilters[]
                {
                   new Filters<string>(nameof(GetQuestionsRequest.Filter),_localizer["FilterTypeTitle"],
                       facets.Select(s=> new KeyValuePair<string, string>(s.ToString("G"),s.GetEnumLocalization()))),

                   new Filters<string>(nameof(GetQuestionsRequest.Source),_localizer["SubjectTypeTitle"],
                       QuestionSubjectMethod.GetValues()
                           .Select(s => new KeyValuePair<string, string>(s.ToString("G"), s.GetEnumLocalization())))
                    //new Models.Filters(nameof(GetQuestionsRequest.Filter),_localizer["FilterTypeTitle"], EnumExtension.GetPublicEnumNames(typeof(QuestionFilter))),
                    //new Models.Filters(nameof(GetQuestionsRequest.Source),_localizer["SubjectTypeTitle"], subjects.Select(s=>s.Subject))
                },
                NextPageLink = nextPageLink
            };

            //var query = _mapper.Map<QuestionsQuery>(model);
            //var resultTask = new List<Task<IEnumerable<QuestionDto>>>();
            //var filters = (model.Filter ?? new QuestionFilter?[] { QuestionFilter.All }).Distinct().ToArray();
            //if (filters.Length == Enum.GetValues(filters.First().GetType()).Length)
            //{
            //    filters = new QuestionFilter?[] {QuestionFilter.All};
            //}

            //foreach (var filter in filters)
            //{
            //    var query = new QuestionsQuery(model.Term, model.Source, model.Page.GetValueOrDefault(), filter);
            //    Task<IEnumerable<QuestionDto>> t = queryBus.QueryAsync(query, token);
            //    resultTask.Add(t);
            //}


            //var result = results.SelectMany(s => s)
            //    .Distinct(new QuestionDtoEqualityComparer()).OrderByDescending(o => o.DateTime).ToList();

            //// var result = await queryBus.QueryAsync(query, token).ConfigureAwait(false);
            ////var p = result.ToList();
            //string nextPageLink = null;
            //if (result.Result.Any())
            //{
            //    nextPageLink = Url.NextPageLink("QuestionSearch", null, model);
            //}



            //return new WebResponseWithFacet<QuestionDto>
            //{
            //    Result = result.Result,
            //    Filters = new IFilters[]
            //    {
            //        new Filters<string>(nameof(GetQuestionsRequest.Filter),_localizer["FilterTypeTitle"], result.FacetState.Select(s=> new KeyValuePair<string, string>(s.ToString("G"),s.GetEnumLocalization()))),
            //        new Filters<int>(nameof(GetQuestionsRequest.Source),_localizer["SubjectTypeTitle"], result.FacetSubject)
            //    },
            //    NextPageLink = nextPageLink
            //};
        }
    }
}