using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Domain.Entities
{
    public partial class Sale
    {
        public string productname { get; set; } = string.Empty;
        public string clienttaxnum { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public int quantity { get; set; }
        public double price { get; set; }

        [IgnoreDataMember]
        public string domain { get; set; } = string.Empty;
        [IgnoreDataMember]
        public string saleid { get; set; } = string.Empty;
    }
}
