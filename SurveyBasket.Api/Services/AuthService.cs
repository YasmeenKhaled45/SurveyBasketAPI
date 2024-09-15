using Hangfire;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Constants;
using SurveyBasket.Api.Contracts.Authentication;
using SurveyBasket.Api.Contracts.JWT;
using SurveyBasket.Api.Helpers;
using SurveyBasket.Api.Models;
using System.Security.Cryptography;
using System.Text;

namespace SurveyBasket.Api.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,IJWTProvider jWTProvider,
        ILogger<AuthService> logger,IEmailSender emailSender,IHttpContextAccessor contextAccessor,
        ApplicationDbContext context) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJWTProvider jWTProvider1 = jWTProvider;
        private readonly ApplicationDbContext context = context;
        private readonly int refreshtokenexpiration = 14;
        public SignInManager<ApplicationUser> SignInManager { get; } = signInManager;
        public ILogger<AuthService> Logger { get; } = logger;
        public IEmailSender EmailSender { get; } = emailSender;
        public IHttpContextAccessor ContextAccessor { get; } = contextAccessor;

        public async Task<Result> RegisterAsync(RegisterRequest register, CancellationToken cancellationToken = default)
        {
            var emailExists = await _userManager.Users.AnyAsync(x => x.Email == register.Email, cancellationToken);
            if (emailExists)
                return Result.Failure(new Error("Registration Failed!", "Email already exists!"));
            var user = register.Adapt<ApplicationUser>();
            user.UserName = register.Email;
            Logger.LogInformation("Mapped UserName: {userName}", user.UserName);
            if (string.IsNullOrEmpty(user.UserName))
            {
                return Result.Failure(new Error("Registration Failed!", "UserName is not being set correctly."));
            }

            var result = await _userManager.CreateAsync(user, register.Password);
            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                Logger.LogInformation("ConfirmationCode: {code}", code);
                await SendConfirmationEmail(user, code);
                return Result.Success();
            }
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            Logger.LogError("User creation failed: {errors}", errors);

            return Result.Failure(new Error("User creation failed", errors));
        }
        public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
        {
            if (await _userManager.FindByIdAsync(request.UserId) is not { } user)
                return Result.Failure(new Error("UserErrors" , "Invalid Code!"));
            if(user.EmailConfirmed)
                return Result.Failure(new Error("DuplicatedConfirmtion", "Email already Confirmed!"));
            var code = request.Code;
            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            } 
            catch (FormatException ex)
            {
                return Result.Failure(new Error("UserErrors", "Invalid Code!"));
            }
            var res = await _userManager.ConfirmEmailAsync(user, code);
            if (res.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, DefaultRoles.Member);
                return Result.Success();
            }
               
            var errors = string.Join(", ", res.Errors.Select(e => e.Description));
            return Result.Failure(new Error("Confirmation Failed", errors));
        }
        public async Task<Result> ResendEmailCodeAsync(ResendEmailCode resendEmail)
        {
            if (await _userManager.FindByEmailAsync(resendEmail.Email) is not { } user)
                return Result.Success();
            if (user.EmailConfirmed)
                return Result.Failure(new Error("DuplicatedConfirmation", "Email already Confirmed!"));
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            Logger.LogInformation("ConfirmationCode: {code}", code);

            return Result.Success();
        }
        public async Task<Result<AuthResponse>> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken = default)
        {
             if(await _userManager.FindByEmailAsync(Email) is not { } user)
                return Result.Failure<AuthResponse>(new Error("User.InvalidCredintials", "Invalid Email or Password"));
            var result = await SignInManager.PasswordSignInAsync(user, Password, false, false);
            if(result.Succeeded)
            {
                var (userroles, userpermissions) = await GetUserRolesAndPermissions(user);
           
                var (Token, ExpiresIn) = jWTProvider1.GenerateToken(user,userroles,userpermissions);
                var refreshtoken = GenereateRefreshToken();
                var refreshtokenExpiration = DateTime.UtcNow.AddDays(refreshtokenexpiration);

                user.RefreshTokens.Add(new RefreshToken
                {
                    Token = refreshtoken,
                    ExpiresOn = refreshtokenExpiration,
                });
                await _userManager.UpdateAsync(user);
                var response = new AuthResponse(user.Id, user.FirstName, user.LastName, user.Email, Token, ExpiresIn, refreshtoken, refreshtokenExpiration);
                return Result.Success(response);
            }
            return Result.Failure<AuthResponse>(new Error("Login Failed","Email is not confirmed"));
         }
        public async Task<AuthResponse?> GetRefreshTokenAsync(string Token, string RefreshToken, CancellationToken cancellationToken = default)
        {
             var userId = jWTProvider1.ValidateToken(Token);
            if (userId == null)
                return null;

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return null;

            var userfrereshtoken = user.RefreshTokens.SingleOrDefault(x => x.Token == RefreshToken && x.IsActive);
            if(userfrereshtoken is null)
                return null;

            userfrereshtoken.RevokedOn = DateTime.UtcNow;
            var (userroles, userpermissions) = await GetUserRolesAndPermissions(user);
            var (newtoken, ExpiresIn) = jWTProvider1.GenerateToken(user,userroles,userpermissions);
            var newrefreshtoken = GenereateRefreshToken();
            var refreshtokenExpiration = DateTime.UtcNow.AddDays(refreshtokenexpiration);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newrefreshtoken,
                ExpiresOn = refreshtokenExpiration,
            });
            await _userManager.UpdateAsync(user);
            return new AuthResponse(user.Id, user.FirstName, user.LastName, user.Email, newtoken, ExpiresIn, newrefreshtoken, refreshtokenExpiration);
        }
        private static string GenereateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public async Task<bool> RevokeTokenAsync(string Token, string RefreshToken, CancellationToken cancellationToken = default)
        {
            var userId = jWTProvider1.ValidateToken(Token);
            if (userId == null)
                return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return false;

            var userfrereshtoken = user.RefreshTokens.SingleOrDefault(x => x.Token == RefreshToken && x.IsActive);
            if (userfrereshtoken is null)
                return false;

            userfrereshtoken.RevokedOn = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            return true;
        }

        private async Task SendConfirmationEmail(ApplicationUser user,string code)
        {
            var origin = ContextAccessor.HttpContext?.Request.Headers.Origin;
            var emailbody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
               templeteModel: new Dictionary<string, string>
                {
                        {"{{name}}", user.FirstName},
                        {"{{action_url}}", $"{origin}/auth/ConfirmEmail?userId={user.Id}&code={code}"}
                });
            BackgroundJob.Enqueue(() => EmailSender.SendEmailAsync(user.Email!, "SurveyBasket", emailbody));
            await Task.CompletedTask;
        }

        public async Task<Result> SendResetPasswordCode(string Email)
        {
            if (await _userManager.FindByEmailAsync(Email) is not { } user)
                return Result.Success();
            if (!user.EmailConfirmed)
                return Result.Failure(new Error("UserErrors","Email is not confirmed!"));
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            Logger.LogInformation("ResetCode: {code}", code);
            await sendResetPasswordEmail(user, code);
            return Result.Success();
        }
        public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null || !user.EmailConfirmed)
                return Result.Failure(new Error("UserErrors", "Invalid Code!"));
            IdentityResult result;
            try
            {
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
                result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);
            }catch(FormatException)
            {
                result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
            }
            if (result.Succeeded)
                return Result.Success();
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code,error.Description));
        }
        private async Task sendResetPasswordEmail(ApplicationUser user, string code)
        {
            var origin = ContextAccessor.HttpContext?.Request.Headers.Origin;
            var emailbody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword",
               templeteModel: new Dictionary<string, string>
                {
                        {"{{name}}", user.FirstName},
                        {"{{action_url}}", $"{origin}/auth/forgetPassword?email={user.Email}&code={code}"}
                });
            BackgroundJob.Enqueue(() => EmailSender.SendEmailAsync(user.Email!, "SurveyBasket : ResetPassword", emailbody));
            await Task.CompletedTask;
        }

        private async Task<(IEnumerable<string> Roles , IEnumerable<string> permissions)> GetUserRolesAndPermissions(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var permissions = await (from r in context.Roles
                                     join p in context.RoleClaims
                                     on r.Id equals p.RoleId
                                     where roles.Contains(r.Name!)
                                     select p.ClaimValue!)
                                      .Distinct().ToListAsync();
            return (roles, permissions);
        }
    }

}
