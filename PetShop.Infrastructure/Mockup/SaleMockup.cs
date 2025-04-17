using PetShop.Application.Interfaces;
using PetShop.Domain.Entities;

namespace PetShop.Infrastructure.Mockup
{
    public class SaleMockup : ISaleRepository
    {
        public static List<Sale> _mockupSales = new List<Sale>();

        public async Task<Sale?> Create(Sale entity)
        {
            return await Task.Run(() =>
            {
                _mockupSales.Add(entity);
                return entity;
            });
        }

        public async Task<bool> Delete(Sale entity)
        {
            return await Task.Run(() =>
            {
                var items = _mockupSales.Where(x => x.saleid == entity.saleid && x.domain == entity.domain);
                if (items.Any())
                {
                    _mockupSales.Remove(items.First());
                    return true;
                }
                return false;
            });
        }

        public async Task<IEnumerable<Sale>> RetrieveList(string domain)
        {
            return await Task.Run(() =>
            {
                return _mockupSales.Where(x => x.domain == domain);
            });
        }
    }
}
