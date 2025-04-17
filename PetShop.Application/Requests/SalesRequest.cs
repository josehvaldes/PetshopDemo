using System.Text.Json.Serialization;

namespace PetShop.Application.Requests
{
    public class SalesRequest
    {
        public string ProductName { get; set; } = string.Empty;
        
        public required ClientRequest Client { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

        //ignore username and domain so they are not included in swagger
        [JsonIgnore]
        public string Username { get; set; } = string.Empty;
        [JsonIgnore]
        public string Domain { get; set; } = string.Empty;
        
        
    }
}
