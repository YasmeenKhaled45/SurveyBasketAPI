using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SurveyBasket.Api.Constants;
using SurveyBasket.Api.Contracts.Filters;
using SurveyBasket.Api.Contracts.Polls;
using SurveyBasket.Api.Models;
using SurveyBasket.Api.Services;

namespace SurveyBasket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PollsController(IPollService pollService) : ControllerBase
    {
        private readonly IPollService _pollService = pollService;

        [HttpGet("AllPolls")]
        [HasPermission(Permissions.GetPolls)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            return Ok(await _pollService.GetAll(cancellationToken));
        }
        [HttpGet("CurrentPolls")]
        [Authorize(Roles = DefaultRoles.Member)]
        public async Task<IActionResult> GetCurrentPolls(CancellationToken cancellationToken)
        {
            return Ok(await _pollService.GetAvailablePolls(cancellationToken));
        }
        [HttpPost("Poll")]
        [HasPermission(Permissions.AddPolls)]
        public async Task<IActionResult> Add([FromBody] PollRequest poll ,
            CancellationToken cancellation)
        {

            var polls = await _pollService.Add(poll,cancellation);
            return polls.IsSuccess ? Ok(poll) : BadRequest(polls.Error);
        }
        [HttpPut("{Id}")]
        [HasPermission(Permissions.UpdatePolls)]
        public async Task<IActionResult> Update([FromRoute] int Id, [FromBody] PollRequest poll,CancellationToken cancellationToken)
        {
            var Ispudated = await _pollService.Update(Id, poll ,cancellationToken);
            return Ispudated.IsSuccess ? NoContent() : NotFound(Ispudated.Error);
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete([FromRoute] int Id , CancellationToken cancellationToken=default)
        {
            var isDeleted = await _pollService.Delete(Id,cancellationToken);
           return isDeleted.IsSuccess ? NoContent() : NotFound(isDeleted.Error);
        }

        [HttpPut("{Id}/TogglePublish")]
        [HasPermission(Permissions.UpdatePolls)]
        public async Task<IActionResult> TogglePublish([FromRoute]int Id , CancellationToken cancellationToken)
        {
            var ispudated = await _pollService.TogglePublishStatus(Id,cancellationToken);
            return ispudated.IsSuccess ? NoContent() : NotFound(ispudated.Error);
        }
    }
}
