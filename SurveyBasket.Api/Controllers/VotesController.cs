using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;
using SurveyBasket.Api.Constants;
using SurveyBasket.Api.Contracts.Votes;
using SurveyBasket.Api.Services;
using System.Security.Claims;

namespace SurveyBasket.Api.Controllers
{
    [Route("api/polls/{pollId}/vote")]
    [ApiController]
    [Authorize(Roles = DefaultRoles.Member)]
    [EnableRateLimiting(RateLimiters.Concurrency)]
    public class VotesController(IQuestionService questionService,IVoteService voteService) : ControllerBase
    {
        public IQuestionService QuestionService = questionService;
        private readonly IVoteService _voteService = voteService;

        [HttpGet("")]
        public async Task<IActionResult> AvailableVotes([FromRoute] int pollId , CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await QuestionService.GetAvailableQuestions(pollId, userId!, cancellationToken);
            if(result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return BadRequest(result.Error);
        }
        [HttpPost("SaveVote")]
        public async Task<IActionResult> SaveVote([FromRoute] int pollId , [FromBody] VoteRequest request , CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _voteService.SaveVote(pollId , userId! , request , cancellationToken);
            if (result.IsSuccess)
                return Created();

            return BadRequest(result.Error);
        }
    }
}
