using FluentValidation;
using PetShopAPI.Models;

namespace PetShopAPI.Validators
{
    public class AuthenticationRequestValidator : AbstractValidator<AuthenticationRequest>
    {
        public AuthenticationRequestValidator() 
        {
            RuleFor(x=> x.Username).NotNull().NotEmpty().WithMessage("{PropertyName} is required!");
            RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage("{PropertyName} is required!");
            RuleFor(x => x.Domain).NotNull().NotEmpty().WithMessage("{PropertyName} is required!");

        }
    }
}
