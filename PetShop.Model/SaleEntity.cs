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
    public class SaleEntity : ITableEntity
    {
        [IgnoreDataMember]
        public string domain { get; set; } = string.Empty;
        [IgnoreDataMember]
        public string saleid { get; set; } = string.Empty;
        [JsonIgnore]
        public string PartitionKey { get { return domain; } set { domain = value; } }
        [JsonIgnore]
        public string RowKey { get { return saleid; } set { saleid = value; } }

        public string productname { get; set; } = string.Empty;
        public string clienttaxnum { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public int quantity { get; set; }
        public double price { get; set; }

        [JsonIgnore]
        public DateTimeOffset? Timestamp { get; set; }
        [JsonIgnore]
        public ETag ETag { get; set; }
    }
}
