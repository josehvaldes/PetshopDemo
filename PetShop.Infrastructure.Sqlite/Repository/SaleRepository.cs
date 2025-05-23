using PetShop.Application.Interfaces.Repository;
using PetShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Infrastructure.Sqlite.Repository
{
    public class SaleRepository : ISaleRepository
    {
        private readonly DatabaseFixture _databaseFixture;

        public SaleRepository()
        {
            _databaseFixture = new DatabaseFixture();
        }

        public SaleRepository(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
        }

        public Task<bool> Create(Sale entity)
        {
            using var connection = _databaseFixture.GetConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Sales (saleId, domain, price, quantity, username, clienttaxnum, productname) " +
                $"VALUES (@saleId, @domain, @price, @quantity, @username, @clienttaxnum, @productname)";
            
            var saleIdParam = command.CreateParameter();
            saleIdParam.ParameterName = "@saleId";
            saleIdParam.Value = entity.saleid;
            command.Parameters.Add(saleIdParam);

            var saleDomainParam = command.CreateParameter();
            saleDomainParam.ParameterName = "@domain";
            saleDomainParam.Value = entity.domain;
            command.Parameters.Add(saleDomainParam);

            var salePriceParam = command.CreateParameter();
            salePriceParam.ParameterName = "@price";
            salePriceParam.Value = entity.price;
            command.Parameters.Add(salePriceParam);

            var saleQuantityParam = command.CreateParameter();
            saleQuantityParam.ParameterName = "@quantity";
            saleQuantityParam.Value = entity.quantity;
            command.Parameters.Add(saleQuantityParam);

            var saleUsernameParam = command.CreateParameter();
            saleUsernameParam.ParameterName = "@username";
            saleUsernameParam.Value = entity.username;
            command.Parameters.Add(saleUsernameParam);

            var saleClientTaxNumParam = command.CreateParameter();
            saleClientTaxNumParam.ParameterName = "@clienttaxnum";
            saleClientTaxNumParam.Value = entity.clienttaxnum;
            command.Parameters.Add(saleClientTaxNumParam);

            var saleProductNameParam = command.CreateParameter();
            saleProductNameParam.ParameterName = "@productname";
            saleProductNameParam.Value = entity.productname;
            command.Parameters.Add(saleProductNameParam);

            command.ExecuteNonQuery();
            return Task.FromResult(true);
        }

        public Task<bool> Delete(Sale entity)
        {
            using var connection = _databaseFixture.GetConnection();
            using var command = connection.CreateCommand();

            command.CommandText = "DELETE FROM Sales WHERE saleId = @saleId";
            var saleIdParam = command.CreateParameter();
            saleIdParam.ParameterName = "@saleId";
            saleIdParam.Value = entity.saleid;
            command.Parameters.Add(saleIdParam);
            command.ExecuteNonQuery();
            return Task.FromResult(true);
        }

        public Task<IEnumerable<Sale>> RetrieveList(string domain)
        {
            using var connection = _databaseFixture.GetConnection();
            using var command = connection.CreateCommand();

            command.CommandText = "SELECT * FROM Sales WHERE domain = @domain";
            var domainParam = command.CreateParameter();
            domainParam.ParameterName = "@domain";
            domainParam.Value = domain;
            command.Parameters.Add(domainParam);
            using var reader = command.ExecuteReader();
            var sales = new List<Sale>();
            while (reader.Read())
            {
                var sale = new Sale
                {
                    saleid = reader.GetString(0),
                    domain = reader.GetString(1),
                    price = reader.GetDouble(2),
                    quantity = reader.GetInt32(3),
                    username = reader.GetString(4),
                    clienttaxnum = reader.GetString(5),
                    productname = reader.GetString(6)
                };
                sales.Add(sale);
            }

            return Task.FromResult<IEnumerable<Sale>>(sales);
        }
    }
}
