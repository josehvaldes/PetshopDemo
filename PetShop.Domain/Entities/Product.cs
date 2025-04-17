using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Domain.Entities
{
    public partial class Product
    {
        public string guid { get; set; } = string.Empty;
        public string category { get; set; } = string.Empty;
        public string pettype { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public int stock { get; set; }
        public double unitaryprice { get; set; }

        [IgnoreDataMember]
        public string domain { get; set; } = string.Empty;
        [IgnoreDataMember]
        public string name { get; set; } = string.Empty;
    }
}
