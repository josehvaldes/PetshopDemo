using FluentValidation;
using PetShopAPI.Models;

namespace PetShopAPI.Validators
{
    public class AddUserRequestValidator: AbstractValidator<AddUserRequest>
    {
        public AddUserRequestValidator() 
        {
            RuleFor(x => x.Username).NotNull().NotEmpty().WithMessage("{PropertyName} is required!");
            RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage("{PropertyName} is required!");
            RuleFor(x => x.Domain).NotNull().NotEmpty().WithMessage("{PropertyName} is required!");
            RuleFor(x => x.Email).NotNull().NotEmpty().WithMessage("{PropertyName} is required!");
            RuleFor(x => x.Email).EmailAddress().WithMessage("{PropertyName} is not a vali email!");
        }
    }
}
