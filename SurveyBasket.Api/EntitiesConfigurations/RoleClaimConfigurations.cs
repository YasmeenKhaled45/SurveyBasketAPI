using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyBasket.Api.Constants;

namespace SurveyBasket.Api.EntitiesConfigurations
{
    public class RoleClaimConfigurations : IEntityTypeConfiguration<IdentityRoleClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
        {
           var permissions = Permissions.GetAllPermissions();
            var adminclaims = new List<IdentityRoleClaim<string>>();

            for (int i = 0; i < permissions.Count; i++)
            {
                adminclaims.Add(new IdentityRoleClaim<string>
                {
                    Id = i + 1,
                    ClaimType = Permissions.Type,
                    ClaimValue = permissions[i],
                    RoleId = DefaultRoles.AdminRoleId
                });
                
            }
        }
    }
}
