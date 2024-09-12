using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Abstractions;
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
        public async Task<IActionResult> GetAllQuestions(int PollId , CancellationToken cancellation)
        {
            var res = await _questionService.GetAllQuestions(PollId, cancellation);
            return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById([FromRoute] int PollId ,[FromRoute] int Id,CancellationToken cancellation)
        {
            var res =  await _questionService.GetQuestionById(PollId, Id, cancellation);
            return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
        }
        [HttpPost("")]
        public async Task<IActionResult> Add([FromRoute]int PollId,[FromBody]QuestionRequest request , CancellationToken cancellation)
        {
            var result = await _questionService.AddAsync(PollId, request , cancellation);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }
        [HttpPut("{Id}")]
        public async Task<IActionResult> ToggleQuestion([FromRoute] int PollId,[FromRoute] int Id , CancellationToken cancellationToken)
        {
            var res = await _questionService.ToggleQuestion(PollId, Id, cancellationToken);
            return res.IsSuccess ? NoContent() : BadRequest(res.Error);
        }
        [HttpPut("{Id}/updateasync")]
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
