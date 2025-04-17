using FluentValidation;
using PetShop.Application.Requests;

namespace PetShopAPI.Validators
{
    public class ProductRequestValidator : AbstractValidator<ProductRequest>
    {
        public ProductRequestValidator() 
        {
            RuleFor(x => x.UnitaryPrice).GreaterThan(0).WithMessage("{PropertyName} has to be greater than zero.");
            RuleFor(x => x.Stock).GreaterThan(0).WithMessage("{PropertyName} has to be greater than zero.");
            RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("{PropertyName} is required.");
            RuleFor(x => x.Domain).NotNull().NotEmpty().WithMessage("{PropertyName} is required.");
            RuleFor(x => x.Category).NotNull().NotEmpty().WithMessage("{PropertyName} is required.");
            RuleFor(x => x.Description).NotNull().NotEmpty().WithMessage("{PropertyName} is required.");
            RuleFor(x => x.PetType).NotNull().NotEmpty().WithMessage("{PropertyName} is required.");
        }
    }
}
