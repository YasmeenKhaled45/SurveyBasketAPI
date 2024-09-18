using FluentValidation;

namespace SurveyBasket.Api.Contracts.Roles
{
    public class RoleRequestValidator : AbstractValidator<RoleRequest>
    {
        public RoleRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty()
                .Length(3, 200);

            RuleFor(x => x.permissions).NotNull()
                .NotEmpty();

            RuleFor(x => x.permissions).
                Must(x => x.Distinct().Count() == x.Count)
                .WithMessage("You can not add duplicated permissions for the same role")
                .When(x => x.permissions != null);

        }
    }
}
