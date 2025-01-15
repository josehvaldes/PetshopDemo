using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PetShop.Model
{
    public class ProductEntity : ITableEntity
    {
        [IgnoreDataMember]
        public string domain { get; set; } = string.Empty;
        [IgnoreDataMember]
        public string name { get; set; } = string.Empty;
        [JsonIgnore]
        public string PartitionKey { get { return domain; } set { domain = value; } }
        [JsonIgnore]
        public string RowKey { get { return name; } set { name = value; } }
        
        public string guid { get; set; } = string.Empty;
        public string category { get; set; } = string.Empty;
        public string pettype { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public int stock { get; set; }
        public double unitaryprice { get; set; }

        [JsonIgnore]
        public DateTimeOffset? Timestamp { get; set; }
        [JsonIgnore]
        public ETag ETag { get; set; }
    }
}
