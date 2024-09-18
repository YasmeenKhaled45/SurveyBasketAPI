using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Constants;
using SurveyBasket.Api.Contracts.Filters;
using SurveyBasket.Api.Contracts.Questions;
using SurveyBasket.Api.Services;

namespace SurveyBasket.Api.Controllers
{
    [Route("api/polls/{PollId}/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestionsController(IQuestionService questionService) : ControllerBase
    {
        private readonly IQuestionService _questionService = questionService;
        [HttpGet("AllQuestions")]
        [HasPermission(Permissions.GetQuestions)]
        public async Task<IActionResult> GetAllQuestions(int PollId , [FromQuery] RequestFilters filters ,CancellationToken cancellation)
        {
   
            var res = await _questionService.GetAllQuestions(PollId,filters, cancellation);
            return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
        }
        [HttpGet("{Id}")]
        [HasPermission(Permissions.GetQuestions)]
        public async Task<IActionResult> GetById([FromRoute] int PollId ,[FromRoute] int Id,CancellationToken cancellation)
        {
            var res =  await _questionService.GetQuestionById(PollId, Id, cancellation);
            return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
        }
        [HttpPost("")]
        [HasPermission(Permissions.AddQuestions)]
        public async Task<IActionResult> Add([FromRoute]int PollId,[FromBody]QuestionRequest request , CancellationToken cancellation)
        {
            var result = await _questionService.AddAsync(PollId, request , cancellation);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }
        [HttpPut("{Id}")]
        [HasPermission(Permissions.UpdateQuestions)]
        public async Task<IActionResult> ToggleQuestion([FromRoute] int PollId,[FromRoute] int Id , CancellationToken cancellationToken)
        {
            var res = await _questionService.ToggleQuestion(PollId, Id, cancellationToken);
            return res.IsSuccess ? NoContent() : BadRequest(res.Error);
        }
        [HttpPut("{Id}/updateasync")]
        [HasPermission(Permissions.UpdateQuestions)]
        public async Task<IActionResult> UpdateAsync([FromRoute] int PollId, [FromRoute] int Id, 
            [FromBody] QuestionRequest request , CancellationToken cancellationToken)
        {
            var result = await _questionService.UpdateAsync(PollId,Id,request , cancellationToken);
            if (result.IsSuccess)
                return NoContent();

            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }
    }
}
