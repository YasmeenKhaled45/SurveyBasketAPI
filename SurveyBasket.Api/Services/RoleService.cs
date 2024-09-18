using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Constants;
using SurveyBasket.Api.Contracts.Roles;
using SurveyBasket.Api.Models;

namespace SurveyBasket.Api.Services
{
    public class RoleService(ApplicationDbContext context, RoleManager<ApplicationRole> roleManager) : IRoleService
    {
        private readonly ApplicationDbContext context = context;
        private readonly RoleManager<ApplicationRole> roleManager = roleManager;

        public async Task<IEnumerable<RoleResponse>> GetAllRoles(CancellationToken cancellationToken)=>
          await roleManager.Roles.Where(x=>!x.IsDefault && !x.IsDeleted).ProjectToType<RoleResponse>()
            .ToListAsync(cancellationToken);
        public async Task<Result<RoleDetailResponse>> AddRoleAsync(RoleRequest request)
        {
            var roleexists = await roleManager.RoleExistsAsync(request.Name);
            if (roleexists)
                return Result.Failure<RoleDetailResponse>(new Error("RoleErrors", "DuplicatedRole!"));

            var allowedpermissions = Permissions.GetAllPermissions();
            if (request.permissions.Except(allowedpermissions).Any())
                return Result.Failure<RoleDetailResponse>(new Error("RoleErrors", "InvalidPermissions!"));

            var role = new ApplicationRole
            {
                Name = request.Name,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
            };
            var res = await roleManager.CreateAsync(role);
            if (res.Succeeded)
            {
                var permissions = request.permissions
                    .Select(x => new IdentityRoleClaim<string>
                    {
                        ClaimType = Permissions.Type,
                        ClaimValue = x,
                        RoleId = role.Id
                    });
                await context.AddRangeAsync(permissions);
                await context.SaveChangesAsync();
               var response = new RoleDetailResponse(role.Id, role.Name,request.permissions);
                return Result.Success(response);
            }
            var error = res.Errors.First();
            return Result.Failure<RoleDetailResponse>(new Error(error.Code, error.Description));
        }

    }
}
