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
    public class ClientEntity : ITableEntity
    {
        [IgnoreDataMember]
        public string taxnumber { get; set; } = string.Empty;
        [IgnoreDataMember]
        public string taxnumberend { get; set; } = string.Empty;

        [JsonIgnore]
        public string PartitionKey { get { return taxnumberend; } set { taxnumberend = value; } }
        [JsonIgnore]
        public string RowKey { get { return taxnumber; } set { taxnumber = value; } }
        public string guid { get; set; } = string.Empty;
        public string fullname { get; set; } = string.Empty;
        [JsonIgnore]
        public DateTimeOffset? Timestamp { get; set; }
        [JsonIgnore]
        public ETag ETag { get; set; }
    }
}
