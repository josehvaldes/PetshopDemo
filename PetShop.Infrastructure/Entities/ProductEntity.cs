using Azure;
using Azure.Data.Tables;
using PetShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PetShop.Infrastructure.Entities
{
    public class ProductEntity : Product, ITableEntity
    {
        [JsonIgnore]
        public string PartitionKey { get { return domain; } set { domain = value; } }
        [JsonIgnore]
        public string RowKey { get { return name; } set { name = value; } }

        [JsonIgnore]
        public DateTimeOffset? Timestamp { get; set; }
        [JsonIgnore]
        public ETag ETag { get; set; }
    }
}
