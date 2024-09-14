using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyBasket.Api.Constants;
using SurveyBasket.Api.Models;

namespace SurveyBasket.Api.EntitiesConfigurations
{
    public class RoleConfirguartions : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.HasData([
                new ApplicationRole{
                    Id = DefaultRoles.AdminRoleId,
                    Name = DefaultRoles.Admin,
                    ConcurrencyStamp = DefaultRoles.AdminRoleConcurrencyStamp,
                    NormalizedName = DefaultRoles.Admin.ToUpper()
                },
                new ApplicationRole{
                    Id= DefaultRoles.MemberRoleId,
                    Name = DefaultRoles.Member,
                    NormalizedName = DefaultRoles.Member.ToUpper(),
                    ConcurrencyStamp= DefaultRoles.MemberRoleConcurrencyStamp,
                    IsDefault = true
                }
            ]);
        }
    }
}
