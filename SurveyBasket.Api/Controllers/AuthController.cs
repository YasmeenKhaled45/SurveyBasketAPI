using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SurveyBasket.Api.Contracts.Authentication;
using SurveyBasket.Api.Contracts.JWT;
using SurveyBasket.Api.Services;
using LoginRequest = SurveyBasket.Api.Contracts.Authentication.LoginRequest;
using RegisterRequest = SurveyBasket.Api.Contracts.Authentication.RegisterRequest;
using ResetPasswordRequest = SurveyBasket.Api.Contracts.Authentication.ResetPasswordRequest;

namespace SurveyBasket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService , IOptions<JWTOptions> options) : ControllerBase
    {
        private readonly IAuthService authService = authService;
        private readonly JWTOptions jWTOptions = options.Value;

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody]LoginRequest request , CancellationToken token)
        {
            var authResult = await authService.GetTokenAsync(request.Email,request.Password,token);
            return authResult.IsSuccess ? Ok(authResult.Value) : BadRequest(authResult.Error);
        }
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request , CancellationToken cancellationToken)
        {
            var result = await authService.RegisterAsync(request,cancellationToken);
            return result.IsSuccess ? Ok() : BadRequest(result.Error);
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
        
        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmailAsync([FromBody]ConfirmEmailRequest confirmEmail)
        {
            var result = await authService.ConfirmEmailAsync(confirmEmail);
            return result.IsSuccess ? Ok() : BadRequest();
        }
        [HttpPost("ResendEmailConfirmation")]
        public async Task<IActionResult> ResendEmailConfirm0([FromBody] ResendEmailCode code)
        {
            var result = await authService.ResendEmailCodeAsync(code);
            return result.IsSuccess ? Ok() : BadRequest();
        }
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgotPasswordRequest request)
        {
            var result = await authService.SendResetPasswordCode(request.Email);
            return result.IsSuccess ? Ok() : BadRequest();
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await authService.ResetPasswordAsync(request);
            return result.IsSuccess ? Ok() : BadRequest();
        }
    }
}
