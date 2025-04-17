using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Domain.Entities
{
    public partial class Client
    {
        public string guid { get; set; } = string.Empty;
        public string fullname { get; set; } = string.Empty;

        [IgnoreDataMember]
        public string taxnumber { get; set; } = string.Empty;
        [IgnoreDataMember]
        public string taxnumberend { get; set; } = string.Empty;
    }
}
