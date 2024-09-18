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
        public async Task<Result> UpdateRole(string Id, RoleRequest request)
        {
            var roleIsExists = await roleManager.Roles.AnyAsync(x => x.Name == request.Name && x.Id != Id);

            if (roleIsExists)
                return Result.Failure<RoleDetailResponse>(new Error("RoleErrors", "DuplicatedRole!"));

            if (await roleManager.FindByIdAsync(Id) is not { } role)
                return Result.Failure<RoleDetailResponse>(new Error("RoleErrors", "RoleNotFound!"));

            var allowedPermissions = Permissions.GetAllPermissions();

            if (request.permissions.Except(allowedPermissions).Any())
                return Result.Failure<RoleDetailResponse>(new Error("RoleErrors", "InvalidPermissions!"));

            role.Name = request.Name;

            var result = await roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                var currentPermissions = await context.RoleClaims
                    .Where(x => x.RoleId == Id && x.ClaimType == Permissions.Type)
                    .Select(x => x.ClaimValue)
                    .ToListAsync();

                var newPermissions = request.permissions.Except(currentPermissions)
                    .Select(x => new IdentityRoleClaim<string>
                    {
                        ClaimType = Permissions.Type,
                        ClaimValue = x,
                        RoleId = role.Id
                    });

                var removedPermissions = currentPermissions.Except(request.permissions);

                await context.RoleClaims
                    .Where(x => x.RoleId == Id && removedPermissions.Contains(x.ClaimValue))
                .ExecuteDeleteAsync();


                await context.AddRangeAsync(newPermissions);
                await context.SaveChangesAsync();

                return Result.Success();
            }

            var error = result.Errors.First();

            return Result.Failure<RoleDetailResponse>(new Error(error.Code, error.Description));
        }
    }
}
