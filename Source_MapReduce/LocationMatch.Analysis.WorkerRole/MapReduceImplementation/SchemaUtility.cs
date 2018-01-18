using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using LocationMatch.Analysis.WorkerRole.MapReduceImplementation.HadoopConnections;
using Microsoft.Hadoop.MapReduce;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;
using Microsoft.Hadoop.WebHCat.Protocol;

namespace LocationMatch.Analysis.WorkerRole.MapReduceImplementation
{
    public class SchemaUtility
    {
        public SchemaUtility(string analysisPath)
        {
            _hadoopConnectionsFactory = HadoopConnectionsFactory.Instance;

            _analysisPath = analysisPath;
            _hadoopClient = _hadoopConnectionsFactory.CreateWebHCatHttpClient();
        }

        private readonly IHadoopConnectionsFactory _hadoopConnectionsFactory;
        private readonly string _analysisPath;
        private readonly WebHCatHttpClient _hadoopClient;

        public string MakeAnalysisFolderStructure(int analysisId)
        {
            IHadoop hadoop = _hadoopConnectionsFactory.CreateHadoopConnection();
            string analysisDirectoryPath = _analysisPath + "/" + analysisId;
            string analysisInputPath = analysisDirectoryPath + "/input";
            string analysisInputTrackPath = analysisInputPath + "/track";
            string analysisInputLocationPath = analysisInputPath + "/locationWithRadius";
            string analysisWorkPath = analysisDirectoryPath + "/work";
            string analysisWorkTrackpointPath = analysisWorkPath + "/trackpoint";
            string analysisJoinedTrackpointLocationPath = analysisWorkPath + "/joinedTrackpointLocation";
            string analysisOutputPath = analysisDirectoryPath + "/output";

            if (hadoop.StorageSystem.Exists(analysisDirectoryPath))
            {
                hadoop.StorageSystem.Delete(analysisDirectoryPath);
            }

            hadoop.StorageSystem.MakeDirectory(analysisDirectoryPath);
            hadoop.StorageSystem.MakeDirectory(analysisInputPath);
            hadoop.StorageSystem.MakeDirectory(analysisInputTrackPath);
            hadoop.StorageSystem.MakeDirectory(analysisInputLocationPath);
            hadoop.StorageSystem.MakeDirectory(analysisWorkPath);
            hadoop.StorageSystem.MakeDirectory(analysisWorkTrackpointPath);
            hadoop.StorageSystem.MakeDirectory(analysisJoinedTrackpointLocationPath);
            hadoop.StorageSystem.MakeDirectory(analysisOutputPath);

            return analysisDirectoryPath;
        }

        public void MakeHiveDatabase(int analysisId)
        {
            string command = "create database if not exists AzureCourse_LocationMatch" + analysisId + ";";
            RunHiveCommand(command);

            command = string.Empty;
            command = command + "use AzureCourse_LocationMatch" + analysisId + ";";
            command = command + string.Format(_createExternalTrackpointTablePattern, _analysisPath, analysisId);
            command = command + string.Format(_createExternalLocationTablePattern, _analysisPath, analysisId);
            command = command + string.Format(_createExternalJoinedTrackpointLocationTablePattern, _analysisPath, analysisId);

            RunHiveCommand(command);
        }

        private string _statusDir = "hivestatus";

        private string _createExternalTrackpointTablePattern =
            @"create external table trackpoint(alat double, alon double) row format delimited fields terminated by ' ' lines terminated by '\n' stored as textfile location '{0}/{1}/work/trackpoint';";

        private string _createExternalLocationTablePattern =
            @"create external table locationWithRadius(id int, dlat double, dlon double, dradius double) row format delimited fields terminated by ' ' lines terminated by '\n' stored as textfile location '{0}/{1}/input/locationWithRadius';";

        private string _createExternalJoinedTrackpointLocationTablePattern =
            @"create external table joinedTrackpointLocation(id int, dlat double, dlon double, dradius double, alat double, alon double) row format delimited fields terminated by ' ' lines terminated by '\n' stored as textfile location '{0}/{1}/work/joinedTrackpointLocation';";

        private void RunHiveCommand(string hiveCommand)
        {
            var task = _hadoopClient.CreateHiveJob(hiveCommand, null, null, _statusDir, null);
            task.Wait();
            var response = task.Result;
            var output = response.Content.ReadAsAsync<JObject>();
            output.Wait();
            response.EnsureSuccessStatusCode();

            string id = output.Result.GetValue("id").ToString();
            _hadoopClient.WaitForJobToCompleteAsync(id).Wait();
        }

        public void UploadTrackInputData(int analysisId, string trackGpx)
        {
            string trackPath = string.Format("{0}/{1}/input/track", _analysisPath, analysisId);

            trackGpx = trackGpx.Replace("\n", string.Empty);
            trackGpx = trackGpx.Replace("\r", string.Empty);

            IHadoop hadoop = _hadoopConnectionsFactory.CreateHadoopConnection();

            hadoop.StorageSystem.WriteAllText(trackPath, trackGpx);
        }

        public void UploadLocationInputData(int analysisId, List<Tuple<int, double, double, double>> locationData)
        {
            var sb = new StringBuilder();
            string dataPattern = "{0} {1} {2} {3}";

            foreach (var locationDataEntity in locationData)
            {
                sb.AppendFormat(
                    CultureInfo.InvariantCulture,
                    dataPattern, 
                    locationDataEntity.Item1, 
                    locationDataEntity.Item2, 
                    locationDataEntity.Item3,
                    locationDataEntity.Item4);

                sb.Append(Environment.NewLine);
            }

            string locationDataText = sb.ToString();

            string locationPath = string.Format("{0}/{1}/input/locationWithRadius/part-0", _analysisPath, analysisId);
            IHadoop hadoop = _hadoopConnectionsFactory.CreateHadoopConnection();

            hadoop.StorageSystem.WriteAllText(locationPath, locationDataText);
        }

        public void JoinTrackLocationData(int analysisId)
        {
            string useCommand = "use AzureCourse_LocationMatch" + analysisId + ";";
            string hiveCommand = useCommand +
                string.Format(@"insert into table joinedTrackpointLocation select * from locationWithRadius join trackpoint;");

            RunHiveCommand(hiveCommand);
        }

        public List<int> ReadAllResultData(int analysisId)
        {
            string folderPath = _analysisPath.Substring(1);
            string resultPath = string.Format("{0}/{1}/output", folderPath, analysisId);

            CloudBlobContainer container = _hadoopConnectionsFactory.CreateCloudBlobContainer();
            var directory = container.GetDirectoryReference(resultPath);
            List<IListBlobItem> blobs = directory.ListBlobs(true).ToList();

            var result = new List<int>();

            foreach (var blobItem in blobs)
            {
                var blob = blobItem as CloudBlockBlob;

                if (blob == null)
                {
                    continue;
                }

                if (blob.Name.IndexOf("part", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                string blobContent = blob.DownloadText();

                List<string> lines = blobContent
                    .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToList();

                result.AddRange(lines.Select(int.Parse));
            }

            return result;
        }

        public void DropHiveDatabase(int analysisId)
        {
            string useCommand = "use AzureCourse_LocationMatch" + analysisId + ";";
            string hiveCommand = useCommand + string.Format(@"drop table joinedTrackpointLocation;");
            hiveCommand = hiveCommand + string.Format(@"drop table trackpoint;");
            hiveCommand = hiveCommand + string.Format(@"drop table locationWithRadius;");
            hiveCommand = hiveCommand + "drop database AzureCourse_LocationMatch" + analysisId + ";";

            RunHiveCommand(hiveCommand);
        }
    }
}
