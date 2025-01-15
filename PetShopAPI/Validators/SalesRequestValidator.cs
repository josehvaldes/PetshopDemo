using FluentValidation;
using PetShop.Model;
using PetShopAPI.Models;

namespace PetShopAPI.Validators
{
    public class SalesRequestValidator : AbstractValidator<SalesRequest>
    {
        public SalesRequestValidator() 
        {
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("{PropertyName} has to be greater than zero.");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("{PropertyName} has to be greater than zero.");
            RuleFor(x => x.Price).NotNull().NotEmpty().WithMessage("{PropertyName} is required.");
            RuleFor(x => x.Client).NotNull().WithMessage("{PropertyName} is required.");
            RuleFor(x => x.Client).SetValidator(new ClientRequestValidator());
        }
    }
}
