using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BlobQuickstartV12
{
    class Program
    {
        static async Task Main()
        {
            Console.WriteLine("Azure Blob Storage v12 - .NET quickstart sample\n");

            await UploadMultiFile();

            List<string> resultURL = await UploadMultiFile();
            foreach (string url in resultURL)
            {
                Console.WriteLine("{0}", url);
            }
        }

        static async Task<string> UploadFile()
        {
            string? connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("containername");
            await containerClient.CreateIfNotExistsAsync();


            string localPath = "./data/";
            string fileName = "quickstart" + Guid.NewGuid().ToString() + ".txt";
            string localFilePath = Path.Combine(localPath, fileName);

            // Write text to the file
            await File.WriteAllTextAsync(localFilePath, "Hello, World!");

            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            // Upload data from the local file
            await blobClient.UploadAsync(localFilePath, true);

            return blobClient.Uri.AbsoluteUri;
        }

        static async Task<List<string>> UploadMultiFile()
        {
            string? connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("containername");
            await containerClient.CreateIfNotExistsAsync();

            //Getting all Text files in directory
            string[] fileArray = Directory.GetFiles(@"D:\Project\BlobQuickstartV12\data", "*.txt");

            List<string> resultUrls = new List<string> { };

            Parallel.ForEach(fileArray, file =>
                {
                    BlobClient blobClient = containerClient.GetBlobClient(Path.GetFileName(file));
                    resultUrls.Add(blobClient.Uri.AbsoluteUri);
                    blobClient.UploadAsync(file, true);
                });

            return resultUrls;
        }
    }
}