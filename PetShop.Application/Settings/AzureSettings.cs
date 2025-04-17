using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Application.Settings
{
    public class AzureSettings
    {
        public string StorageURI { get; set; } = string.Empty;
        public string AppConfiguration { get; set; } = string.Empty;
        public string BlobStorageURI { get; set; } = string.Empty;
    }
}
