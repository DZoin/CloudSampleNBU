using System;
using Microsoft.Hadoop.MapReduce;
using Microsoft.Hadoop.WebHCat.Protocol;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace LocationMatch.Analysis.WorkerRole.MapReduceImplementation.HadoopConnections
{
    public class EmulatorHadoopConnectionsFactory : IHadoopConnectionsFactory
    {
        public IHadoop CreateHadoopConnection()
        {
            IHadoop result = Hadoop.Connect();
            return result;
        }

        public WebHCatHttpClient CreateWebHCatHttpClient()
        {
            var result = new WebHCatHttpClient(new Uri("http://localhost:50111/"), "hadoop", null);
            return result;
        }

        public CloudBlobContainer CreateCloudBlobContainer()
        {
            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
            var result = cloudBlobClient.GetContainerReference("containername");
            return result;
        }
    }
}
