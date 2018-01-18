using Microsoft.Hadoop.MapReduce;
using Microsoft.Hadoop.WebHCat.Protocol;
using Microsoft.WindowsAzure.Storage.Blob;

namespace LocationMatch.Analysis.WorkerRole.MapReduceImplementation.HadoopConnections
{
    public interface IHadoopConnectionsFactory
    {
        IHadoop CreateHadoopConnection();
        WebHCatHttpClient CreateWebHCatHttpClient();
        CloudBlobContainer CreateCloudBlobContainer();
    }
}
