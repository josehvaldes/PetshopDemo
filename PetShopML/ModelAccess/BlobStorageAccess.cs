using Azure.Identity;
using Azure.Storage.Blobs;

namespace PetShopML.ModelAccess
{
    /// <summary>
    /// Provides access to Azure Blob Storage for retrieving regression models.
    /// </summary>
    public static class BlobStorageAccess
    {
        /// <summary>
        /// Retrieves a regression model from the specified Azure Blob Storage container.
        /// </summary>
        /// <param name="blobcontainerUri">The URI of the Azure Blob Storage container.</param>
        /// <returns>
        /// A <see cref="MemoryStream"/> containing the regression model if successful; otherwise, null.
        /// </returns>
        public static async Task<MemoryStream?> GetRegressionModel(string blobcontainerUri)
        {
            try
            {
                // Create a BlobServiceClient using the provided container URI and Azure Default Credentials
                var blobServiceClient = new BlobServiceClient(
                    new Uri(blobcontainerUri),
                    new DefaultAzureCredential());

                // Define the container and blob names
                var regressionModelName = ""; // The name of the regression model blob
                var containerName = "petshopmodels";

                // Get the container client and blob client
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(regressionModelName);

                // Download the blob content into a memory stream
                MemoryStream memStream = new MemoryStream();
                var response = await blobClient.DownloadToAsync(memStream);

                return memStream;
            }
            catch (Exception )
            {
                // Log or handle the exception as needed
                // Return null if blob storage access fails
                return null;
            }
        }
    }
}
