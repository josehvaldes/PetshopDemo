using FluentValidation;
using PetShopAPI.Models;

namespace PetShopAPI.Validators
{
    public class ClientUpdateRequestValidator: AbstractValidator<ClientUpdateRequest>
    {
        public ClientUpdateRequestValidator() 
        {
            RuleFor(x => x.Fullname).NotNull().NotEmpty().WithMessage("{PropertyName} is required!");
        }
    }
}
