using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SurveyBasket.Api.Contracts.Authentication;
using SurveyBasket.Api.Contracts.JWT;
using SurveyBasket.Api.Services;

namespace SurveyBasket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService , IOptions<JWTOptions> options) : ControllerBase
    {
        private readonly IAuthService authService = authService;
        private readonly JWTOptions jWTOptions = options.Value;

        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody]LoginRequest request , CancellationToken token)
        {
            var authResult = await authService.GetTokenAsync(request.Email,request.Password,token);
            return authResult is null ? BadRequest("Invalid Email or Password!") : Ok(authResult);
        }

        [HttpPost("RefreshTokenAsync")]
        public async Task<IActionResult> RefreshToken([FromBody]RefreshTokenRequest request, CancellationToken token)
        {
            var authResult = await authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, token);
            return authResult is null ? BadRequest("Invalid token!") : Ok(authResult);
        }
        [HttpPost("RevokeRefreshToken")]
        public async Task<IActionResult> RevokeRefreshToken([FromBody]RefreshTokenRequest request, CancellationToken token)
        {
            var isRevoked = await authService.RevokeTokenAsync(request.Token, request.RefreshToken, token);
            return isRevoked ? Ok() : BadRequest("Operation Failed!");
        }

    }
}
