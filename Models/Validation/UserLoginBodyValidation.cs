using AuthService.Entities;
using FluentValidation;

namespace AuthService.Models.Validation
{
    public class UserLoginBodyValidation : AbstractValidator<UserLoginBody>
    {
        public UserLoginBodyValidation()
        {
            RuleFor(u => u.Login)
                .NotEmpty();
            RuleFor(u => u.Password)
                .NotEmpty();
        }
    }
}