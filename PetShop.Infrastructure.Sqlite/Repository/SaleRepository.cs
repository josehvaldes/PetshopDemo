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

        public async Task<bool> Create(Sale entity)
        {
            try
            {
                //remove using to that reuse connection later
                var connection = _databaseFixture.GetConnection();
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

                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex) 
            {
                return false;
            }
        }

        public async Task<bool> Delete(Sale entity)
        {
            try 
            {
                using var connection = _databaseFixture.GetConnection();
                using var command = connection.CreateCommand();

                command.CommandText = "DELETE FROM Sales WHERE saleId = @saleId";
                var saleIdParam = command.CreateParameter();
                saleIdParam.ParameterName = "@saleId";
                saleIdParam.Value = entity.saleid;
                command.Parameters.Add(saleIdParam);
                await command.ExecuteNonQueryAsync();
                return true;

            }
            catch (Exception e) 
            {
                return false;
            }         

        }

        public async Task<IEnumerable<Sale>> RetrieveList(string domain)
        {
            //Remove 'using' to reuse the same connection
            var connection = _databaseFixture.GetConnection();
            using var command = connection.CreateCommand();

            command.CommandText = "SELECT * FROM Sales WHERE domain = @domain";
            var domainParam = command.CreateParameter();
            domainParam.ParameterName = "@domain";
            domainParam.Value = domain;
            command.Parameters.Add(domainParam);
            using var reader = await command.ExecuteReaderAsync();
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

            return sales;
            
        }
    }
}
