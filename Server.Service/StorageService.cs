using Google.Cloud.Storage.V1;
using HeyRed.Mime;
using Google.Apis.Storage.v1.Data;
using Microsoft.Extensions.Configuration;
using static Google.Apis.Storage.v1.Data.Bucket;
using Google.Apis.Auth.OAuth2;
namespace Server.Service
{
    public class StorageService : IStorageService
    {
        private readonly string _bucketName;
        private readonly StorageClient _storageClient;

        public StorageService(IConfiguration configuration)
        {
            _bucketName = configuration["GoogleCloud:BucketName"];
            var credentialsJsonPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");

            if (string.IsNullOrEmpty(credentialsJsonPath))
                throw new InvalidOperationException("Missing GOOGLE_APPLICATION_CREDENTIALS_JSON environment variable.");

           
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsJsonPath);

            _storageClient = StorageClient.Create();

            BucketAddCorsConfiguration();
        }

        public async Task UploadFileAsync(string filePath, string objectName)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file {filePath} does not exist.");
            }

            try
            {
                using (var fileStream = File.OpenRead(filePath))
                {
                    string contentType = MimeTypesMap.GetMimeType(filePath);
                    await _storageClient.UploadObjectAsync(_bucketName, objectName, contentType, fileStream);
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string fileName)
        {
         
            try
            {
                await _storageClient.DeleteObjectAsync(_bucketName, fileName);
                return true;
            }
            catch (Google.GoogleApiException ex)
            {

                if (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return false;
                }
                throw;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public  async Task RenameFileAsync(string oldName,string newName)
        {
         
            await _storageClient.CopyObjectAsync(_bucketName, oldName, _bucketName, newName);
           
            await _storageClient.DeleteObjectAsync(_bucketName, oldName);
           
        }

        public async Task RenameFolderAsync(string oldPrefix, string newPrefix)
        {


            var objects = _storageClient.ListObjects(_bucketName, oldPrefix);

            
            foreach (var obj in objects)
            {
               
                string newObjectName = obj.Name.Replace(oldPrefix, newPrefix);

              
                await _storageClient.CopyObjectAsync(_bucketName, obj.Name, _bucketName, newObjectName);
                await _storageClient.DeleteObjectAsync(_bucketName, obj.Name); 
            }
 

        }

        public async Task<Stream> DownloadFileAsync(string fileName)
        {
            try
            {
                var memoryStream = new MemoryStream();
                await _storageClient.DownloadObjectAsync(_bucketName, fileName, memoryStream);
                memoryStream.Position = 0; 
                return memoryStream;
            }
            catch (Google.GoogleApiException ex)
            {
                if (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null; 
                }
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Bucket BucketAddCorsConfiguration()
        {
           
            var bucket = _storageClient.GetBucket(_bucketName);

            CorsData corsData = new CorsData
            {
                Origin = new string[] { "*" },
                ResponseHeader = new string[] { "Content-Type", "x-goog-resumable" },
                Method = new string[] { "GET", "HEAD", "OPTIONS", "POST", "PUT" },
                MaxAgeSeconds = 3600
            };

            if (bucket.Cors == null)
            {
                bucket.Cors = new List<CorsData>();
            }
            bucket.Cors.Clear();
            bucket.Cors.Add(corsData);


            bucket = _storageClient.UpdateBucket(bucket);
            Console.WriteLine($"bucketName {_bucketName} was updated with a CORS config to allow {string.Join(",", corsData.Method)} requests from" +
                $" {string.Join(",", corsData.Origin)} sharing {string.Join(",", corsData.ResponseHeader)} responseHeader" +
                $" responses across origins.");
            return bucket;
        }
        public async Task ReplaceFileAsync(string fileName, Stream newFileStream)
        {
           
            try
            {
                await _storageClient.UploadObjectAsync(_bucketName, fileName, ".png", newFileStream);
                Console.WriteLine($"File {fileName} was replaced in bucket {_bucketName}.");
               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error replacing file {fileName}: {ex.Message}");
                throw;
            }
        }


    }

}