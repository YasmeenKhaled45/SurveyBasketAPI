using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Contracts.Users;
using SurveyBasket.Api.Services;
using System.Security.Claims;

namespace SurveyBasket.Api.Controllers
{
    [Route("Account")]
    [ApiController]
    [Authorize]
    public class AccountController(IUserService userService) : ControllerBase
    {
        private readonly IUserService userService = userService;

        [HttpGet]
        public async Task<IActionResult> GetUserProfile()
        {
            var result = await userService.GetUserProfile(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return Ok(result.Value);
        }
        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfile request)
        {
            var result = await userService.UpdateProfile(User.FindFirstValue(ClaimTypes.NameIdentifier)!, request);
            return NoContent();
        }
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword change)
        {
            var res = await userService.ChangePassword(User.FindFirstValue(ClaimTypes.NameIdentifier)!, change);
            return res.IsSuccess ? NoContent() : BadRequest(res.Error);

        }
    }
}
