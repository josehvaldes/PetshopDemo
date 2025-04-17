using FluentValidation;
using PetShop.Application.Requests;

namespace PetShopSalesAPI.Validators
{
    public class ClientRequestValidator: AbstractValidator<ClientRequest>
    {
        public ClientRequestValidator() 
        {
            RuleFor(x=> x.TaxNumber).NotNull().NotEmpty().WithMessage("{PropertyName} is required.");
            RuleFor(x => x.FullName).NotNull().NotEmpty().WithMessage("{PropertyName} is required.");
        }

    }
}
