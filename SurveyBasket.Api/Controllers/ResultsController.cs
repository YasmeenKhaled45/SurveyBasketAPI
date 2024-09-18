using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Constants;
using SurveyBasket.Api.Contracts.Filters;
using SurveyBasket.Api.Services;

namespace SurveyBasket.Api.Controllers
{
    [Route("api/polls/{pollId}/[controller]")]
    [ApiController]
    [HasPermission(Permissions.Results)]
    public class ResultsController(IResultService resultService) : ControllerBase
    {
        private readonly IResultService _resultService = resultService;

        [HttpGet]
        public async Task<IActionResult> PollVotes([FromRoute] int pollId , CancellationToken cancellationToken)
        {
             var result = await _resultService.GetPollVotes(pollId,cancellationToken);
             return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }
        [HttpGet("GetVotesPerDay")]
        public async Task<IActionResult> VotesPerDay([FromRoute] int pollId , CancellationToken cancellationToken)
        {
            var result = await _resultService.VotesPerDay(pollId, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }
        [HttpGet("VotesPerQuestion")]
        public async Task<IActionResult> VotesPerQuestion([FromRoute] int pollId , CancellationToken cancellationToken)
        {
            var result = await _resultService.VotesPerQuestion(pollId, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }
    }
}
