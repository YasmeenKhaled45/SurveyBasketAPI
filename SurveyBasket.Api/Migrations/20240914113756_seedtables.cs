using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SurveyBasket.Api.Migrations
{
    /// <inheritdoc />
    public partial class seedtables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDeleted", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "126ae60e-f646-411b-bd89-658e60f2ebff", "4d7ea376-4402-4de9-930d-f46af089778e", false, false, "Admin", "ADMIN" },
                    { "36ea7fb0-137f-486d-a6e1-225222d8017a", "81bee964-0149-4acf-888d-098afcb8df10", true, false, "Member", "MEMBER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "a1b6260e-9983-48fe-8d66-f89ebf40b0bd", 0, "49d7c7b0-9201-46bc-afe8-dcfbb4ead838", "admin@surveybasket.com", true, "SurveyBasket", "Admin", false, null, "ADMIN@SURVEYBASKET.COM", "ADMIN@SURVEYBASKET.COM", "AQAAAAIAAYagAAAAEOsomlNrVYbbqGviIK4EH4F40N+qAKUfk2i9xBmic13JK7pC887Fd7ov14K1RtPz4A==", null, false, "713DDFD886634800A5989628EF79EDE9", false, "admin@surveybasket.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "126ae60e-f646-411b-bd89-658e60f2ebff", "a1b6260e-9983-48fe-8d66-f89ebf40b0bd" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "36ea7fb0-137f-486d-a6e1-225222d8017a");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "126ae60e-f646-411b-bd89-658e60f2ebff", "a1b6260e-9983-48fe-8d66-f89ebf40b0bd" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126ae60e-f646-411b-bd89-658e60f2ebff");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b6260e-9983-48fe-8d66-f89ebf40b0bd");
        }
    }
}
