using AuthService.Entities;
using FluentValidation;

namespace AuthService.Models.Validation
{
    public class UserBodyResponseValidation : AbstractValidator<UserBodyResponse>
    {
        public UserBodyResponseValidation(UserDbContext dbContext)
        {
            RuleFor(u => u.Email)
                .NotEmpty();
            RuleFor(u => u.Password)
                .NotEmpty();
            RuleFor(x => x.Email).Custom(
                (value, context) =>
                {
                    var loginInUse = dbContext.Users.Any(u => u.Email == value);

                    if (loginInUse)
                    {
                        context.AddFailure("Login", "Already in use");
                    }
                });
        }
    }
}