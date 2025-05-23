using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Infrastructure.Sqlite.Repository
{
    public class DatabaseFixture : IDisposable
    {
        private DbConnection _connection = null!;

        public DbConnection GetConnection()
        {
            if (_connection == null)
            {
                _connection = new SqliteConnection("DataSource=:memory:");
                _connection.Open();
                
                InitializeSchema(_connection);
            }
            return _connection;
        }


        private void InitializeSchema(DbConnection connection)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = "CREATE TABLE Sales (saleId TEXT, domain TEXT, price DOUBLE, quantity INT, username TEXT, clienttaxnum TEXT, productname TEXT )";
            command.ExecuteNonQuery();
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
