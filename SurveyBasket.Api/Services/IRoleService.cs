using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contracts.Roles;

namespace SurveyBasket.Api.Services
{
    public interface IRoleService
    {
        Task<Result<RoleDetailResponse>> AddRoleAsync(RoleRequest request);
        Task<Result> UpdateRole(string Id, RoleRequest request); 
        Task<IEnumerable<RoleResponse>> GetAllRoles(CancellationToken cancellationToken);
    }
}
