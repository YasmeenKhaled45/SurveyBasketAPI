using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyBasket.Api.Constants;
using SurveyBasket.Api.Models;

namespace SurveyBasket.Api.EntitiesConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.OwnsMany(x => x.RefreshTokens)
                .ToTable("RefreshTokens")
                .WithOwner()
                .HasForeignKey("UserId");
            builder.Property(x => x.FirstName).HasMaxLength(100);
            builder.Property(x=>x.LastName).HasMaxLength(100);

            var passhash = new PasswordHasher<ApplicationUser>();
            builder.HasData(new ApplicationUser
            {
                Id =  DefaultUsers.AdminId,
                FirstName ="SurveyBasket",
                LastName = "Admin",
                UserName = DefaultUsers.AdminEmail,
                NormalizedUserName = DefaultUsers.AdminEmail.ToUpper(),
                Email = DefaultUsers.AdminEmail,
                NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
                SecurityStamp = DefaultUsers.AdminSecurityStamp,
                ConcurrencyStamp = DefaultUsers.AdminConcurrencyStamp,
                EmailConfirmed = true,
                PasswordHash = passhash.HashPassword(null!,DefaultUsers.Password)
            });
        }

    }
}
