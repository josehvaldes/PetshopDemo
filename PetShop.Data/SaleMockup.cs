using PetShop.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Data
{
    public class SaleMockup : ISaleRepository
    {
        public static List<SaleEntity> _mockupSales = new List<SaleEntity>();

        public async Task<SaleEntity?> Create(SaleEntity entity)
        {
            return await Task.Run(() => {
                _mockupSales.Add(entity);
                return entity;
            });
        }

        public async Task<bool> Delete(SaleEntity entity)
        {
            return await Task.Run(() => {
                var items = _mockupSales.Where(x => x.saleid == entity.saleid && x.domain == entity.domain);
                if (items.Any()) 
                {
                    _mockupSales.Remove(items.First());
                    return true;
                }
                return false;
            });
        }

        public async Task<IEnumerable<SaleEntity>> RetrieveList(string domain)
        {
            return await Task.Run(() => {
                return _mockupSales.Where(x => x.domain == domain);
            });
        }
    }
}
