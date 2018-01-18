using System;
using Microsoft.Hadoop.MapReduce;
using Microsoft.Hadoop.WebHCat.Protocol;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace LocationMatch.Analysis.WorkerRole.MapReduceImplementation.HadoopConnections
{
    public class AzureHadoopConnectionsFactory : IHadoopConnectionsFactory
    {
        private const string _username = "ADMIN";
        private const string _password = "Qwerty1@34";
        private const string _storageAccount = "nbuazurecourse";
        private const string _storageAccountKey =
            "EetQX7cr3sOhXHWGCgaIvXqDAyjtGoc2kiTe3gW+j3ASr0ayjwHSAbEaIMXHoTlNXVf4scGTa8VoYxCKQeYBXw==";
        private const string _containerName = "nbuazurecourse";
        private static Uri _uri = new Uri("https://nbuazurecourse.azurehdinsight.net/");

        public IHadoop CreateHadoopConnection()
        {
            IHadoop result = Hadoop.Connect(
                 _uri,
                 _username,
                 _username,
                 _password,
                 _storageAccount,
                 _storageAccountKey,
                 _containerName,
                 false);

            return result;
        }

        public WebHCatHttpClient CreateWebHCatHttpClient()
        {
            var result = new WebHCatHttpClient(_uri, _username, _password);
            return result;
        }

        public CloudBlobContainer CreateCloudBlobContainer()
        {
            var storageAccount = new CloudStorageAccount(new StorageCredentials(_storageAccount, _storageAccountKey), true);
            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
            var result = cloudBlobClient.GetContainerReference(_containerName);
            return result;
        }
    }
}
