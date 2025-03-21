using Azure.Identity;
using Azure.Storage.Blobs;
using Petshop.Common.Settings;

namespace PetShopML.ModelAccess
{
    public static class BlobStorageAccess
    {

        public static async Task<MemoryStream?> GetRegressionModel(string blobcontainerUri) 
        {
            try 
            {
                var blobServiceClient = new BlobServiceClient(
                            new Uri(blobcontainerUri),
                            new DefaultAzureCredential());
                var regressionModelName = "";
                var containerName = "petshopmodels";
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(regressionModelName);
                MemoryStream memStream = new MemoryStream();
                var response = await blobClient.DownloadToAsync(memStream);

                return memStream;
            } 
            catch (Exception e) 
            {
                //Blob Storage Access failed
                return null;
            }
        }
    }
}
