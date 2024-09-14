using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contracts.Users;
using SurveyBasket.Api.Models;

namespace SurveyBasket.Api.Services
{
    public class UserService(UserManager<ApplicationUser> userManager) : IUserService
    {
        private readonly UserManager<ApplicationUser> userManager = userManager;

        public async Task<Result<UserProfileResponse>> GetUserProfile(string UserId)
        {
            var users = await userManager.Users.Where(x=>x.Id == UserId)
                .ProjectToType<UserProfileResponse>()
                .SingleAsync();
            return Result.Success(users);
        }
        public async Task<Result> UpdateProfile(string UserId, UpdateProfile request)
        {
            //var user = await userManager.FindByIdAsync(UserId);
            //user = request.Adapt(user);

            //await userManager.UpdateAsync(user!);
            var user = await userManager.Users.Where(x => x.Id == UserId).ExecuteUpdateAsync(setters =>    // improve performance in database 
            setters.SetProperty(x => x.FirstName, request.FirstName)
            .SetProperty(x => x.LastName, request.LastName));
            return Result.Success();
        }
        public async Task<Result> ChangePassword(string UserId , ChangePassword request)
        {
            var user = await userManager.FindByIdAsync(UserId);
            var res = await userManager.ChangePasswordAsync(user!, request.CurrentPassword , request.NewPassword);
            if(res.Succeeded)
            {
                return Result.Success();
            }
            var error = res.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description));
        }
    }
}
