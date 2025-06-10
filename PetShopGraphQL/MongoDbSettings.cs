namespace PetShopGraphQL
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string ProductsCollectionName { get; set; } = string.Empty;
        public string SalesCollectionName { get; set; } = string.Empty;
        public MongoDbSettings()
        {
        }
        public MongoDbSettings(string connectionString, string databaseName,
            string productsCollectionName, string salesCollectionName)
        {
            ConnectionString = connectionString;
            DatabaseName = databaseName;
            ProductsCollectionName = productsCollectionName;
            SalesCollectionName = salesCollectionName;
        }
    }
}
