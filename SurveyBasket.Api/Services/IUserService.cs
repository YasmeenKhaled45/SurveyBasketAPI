using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contracts.Users;

namespace SurveyBasket.Api.Services
{
    public interface IUserService
    {
        Task<Result<UserProfileResponse>> GetUserProfile(string UserId);
        Task<Result> UpdateProfile(string UserId, UpdateProfile request);
        Task<Result> ChangePassword(string UserId, ChangePassword request);
    }
}
