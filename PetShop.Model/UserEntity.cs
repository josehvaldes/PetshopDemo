using Azure;
using Azure.Data.Tables;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace PetShop.Model
{
    public class UserEntity : ITableEntity
    {
        
        [IgnoreDataMember]
        public string domain { get; set; } = string.Empty;
        [IgnoreDataMember]
        public string username { get; set; } = string.Empty;
        
        [JsonIgnore]
        public string PartitionKey { get { return domain; } set { domain = value; } }
        [JsonIgnore]
        public string RowKey { get { return username; } set { username = value; } }

        public string guid { get; set; } = Guid.NewGuid().ToString();
        public string hash { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string roles { get; set; } = string.Empty;

        [JsonIgnore]
        public ETag ETag { get; set; }
        [JsonIgnore]
        public DateTimeOffset? Timestamp { get; set; }
    }
}
