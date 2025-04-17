using Microsoft.Extensions.Logging;
using PetShop.Application.Interfaces.Repository;
using PetShop.Application.Interfaces.Services;
using PetShop.Application.Requests;
using PetShop.Domain.Entities;

namespace PetShop.Application.Services
{
    public class SaleService : ISaleService
    {
        private readonly IUserService _userService;
        private readonly IClientService _clientService;
        private readonly ILogger<ISaleService> _logger;
        private readonly ISaleRepository _salesRepository;
        private readonly IProductRepository _productRepository;

        public SaleService(IUserService userService, 
                        IProductRepository productRepository,
                        IClientService clientService,
                        ISaleRepository salesRepository,
                        ILogger<ISaleService> logger) 
        {
            _clientService = clientService;
            _userService = userService;
            _productRepository = productRepository;
            _logger = logger;
            _salesRepository = salesRepository;
        }

        public async Task<CallResponse> Create(SalesRequest request)
        {
            var response = new CallResponse();
            var userEntity = await _userService.Retrieve(request.Domain, request.Username);
            if (userEntity == null) 
            {
                response.AddMessage($"User {request.Domain}/{request.Username} not found");
            }

            var clientEntity = await _clientService.Retrieve(request.Client.TaxNumber);
            if (clientEntity == null)
            {
                //doesnt exist create a new one
                clientEntity = await _clientService.Create(request.Client);
            }
            else 
            {
                if ( !string.Equals(clientEntity.fullname, request.Client.FullName, StringComparison.OrdinalIgnoreCase)) 
                {
                    //mismatch. Cancel sale
                    response.AddMessage($"Client.FullName {request.Client.FullName} doesn't match.");
                }
            }

            var product = await _productRepository.Retrieve(request.Domain, request.ProductName);

            if (product==null) 
            {
                response.AddMessage($"Product {request.ProductName} doesn't exists.");
            }

            //return errors related to missing entities
            if (response.Messages.Any()) 
            {
                return response;
            }

            //business validations
            if (request.Quantity > product?.stock) 
            {
                response.AddMessage($"Stock unavailable. Product {product.name} stock: {product?.stock}");
            }
            var unitaryPrice = product?.unitaryprice ?? 0;
            var expectedPrice = Math.Round(unitaryPrice * request.Quantity, 2);
            if (request.Price != expectedPrice) 
            {
                response.AddMessage($"Price doesn't match. Expected price (Unitary Price x Quantity): {expectedPrice}");
            }

            //skip is any business validation failed
            if (response.Messages.Any()) 
            {
                return response;
            }

            var saleEntity = new Sale() 
            {
                domain = request.Domain,
                saleid = Guid.NewGuid().ToString(),
                productname = request.ProductName,
                clienttaxnum = request.Client.TaxNumber,
                username = request.Username,
                quantity = request.Quantity,
                price = request.Price,
            };

            var entity = await _salesRepository.Create( saleEntity );
            if (entity == null)
            {
                response.AddMessage($"Sales was not created. Review logs for more details");
            }
            else 
            {
                //update product
                if (product != null) 
                {
                    product.stock = product.stock - request.Quantity;
                    var productUpdated = await _productRepository.Update(product);
                    if (!productUpdated)
                    {
                        //rollback sale
                        var deleted = _salesRepository.Delete(saleEntity);
                        response.AddMessage($"Product stock was not updated. Sale rejected. Product: {product.name}, Quantity: {request.Quantity}");
                    }
                    else 
                    {
                        response.SaleId = entity.saleid;
                    }
                }
            }

            return response;
        }

        public async Task<IEnumerable<Sale>> RetrieveList(string domain)
        {
            return await _salesRepository.RetrieveList(domain);
        }
    }
}
