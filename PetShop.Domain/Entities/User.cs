using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Domain.Entities
{
    public partial class User
    {
        public string guid { get; set; } = Guid.NewGuid().ToString();
        public string hash { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string roles { get; set; } = string.Empty;
        [IgnoreDataMember]
        public string domain { get; set; } = string.Empty;
        [IgnoreDataMember]
        public string username { get; set; } = string.Empty;
    }
}
