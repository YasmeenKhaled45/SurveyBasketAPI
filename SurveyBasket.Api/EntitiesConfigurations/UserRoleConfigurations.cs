using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyBasket.Api.Constants;

namespace SurveyBasket.Api.EntitiesConfigurations
{
    public class UserRoleConfigurations : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasData(new IdentityUserRole<string>
            {
                UserId = DefaultUsers.AdminId,
                RoleId = DefaultRoles.AdminRoleId
            });
        }
    }
}
