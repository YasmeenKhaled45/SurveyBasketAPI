using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SurveyBasket.Api.Contracts.Polls;
using SurveyBasket.Api.Models;
using SurveyBasket.Api.Services;

namespace SurveyBasket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PollsController(IPollService pollService) : ControllerBase
    {
        private readonly IPollService _pollService = pollService;

        [HttpGet("AllPolls")]
        public async Task<IActionResult> GetAll()
        {
            var Polls = await _pollService.GetAll();
            var response = Polls.Adapt<IEnumerable<PollResponse>>();
            return Ok(response);
        }

        [HttpPost("Poll")]
        public async Task<IActionResult> Add([FromBody] PollRequest poll ,
            CancellationToken cancellation)
        {

            var polls = await _pollService.Add(poll.Adapt<Poll>(),cancellation);
            return Ok(polls.Adapt<PollResponse>());
        }
        [HttpPut("{Id}")]
        public async Task<IActionResult> Update([FromRoute] int Id, [FromBody] PollRequest poll,CancellationToken cancellationToken)
        {
            var ispudated = await _pollService.Update(Id, poll.Adapt<Poll>(),cancellationToken);
            if (!ispudated)
            {
                return NotFound();
            }
            return NoContent();
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete([FromRoute] int Id , CancellationToken cancellationToken=default)
        {
            var isDeleted = await _pollService.Delete(Id,cancellationToken);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPut("{Id}/TogglePublish")]
        public async Task<IActionResult> TogglePublish([FromRoute]int Id , CancellationToken cancellationToken)
        {
            var ispudated = await _pollService.TogglePublishStatus(Id,cancellationToken);
            if (!ispudated)
                return NotFound();

            return NoContent();
        }
    }
}
