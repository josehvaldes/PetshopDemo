using Azure;
using Azure.Data.Tables;
using PetShop.Domain.Entities;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace PetShop.Infrastructure.Entities
{
    public class UserEntity : User, ITableEntity
    {
        [JsonIgnore]
        public string PartitionKey { get { return domain; } set { domain = value; } }
        [JsonIgnore]
        public string RowKey { get { return username; } set { username = value; } }

        [JsonIgnore]
        public ETag ETag { get; set; }
        [JsonIgnore]
        public DateTimeOffset? Timestamp { get; set; }

    }
}
