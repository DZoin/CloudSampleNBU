using System;
using System.Collections.Generic;
using LocationMatch.Analysis.WorkerRole.MapReduceImplementation.HadoopConnections;
using Microsoft.Hadoop.MapReduce;

namespace LocationMatch.Analysis.WorkerRole.MapReduceImplementation
{
    public class MapReduceLocationMatchCalculator : ILocationMatchesCalculator
    {
        public MapReduceLocationMatchCalculator()
        {
            _hadoopConnectionsFactory = HadoopConnectionsFactory.Instance;
        }

        private readonly IHadoopConnectionsFactory _hadoopConnectionsFactory;
        private const string _analysisPath = "/user/ADMIN/AzureCourse_LocationMatch";

        public List<int> CalculateLocationMatches(
            int analysisId,
            string gpx, 
            List<Tuple<int, double, double, double>> locationsData)
        {
            var schemaUtility = new SchemaUtility(_analysisPath);
            schemaUtility.MakeAnalysisFolderStructure(analysisId);
            schemaUtility.UploadTrackInputData(analysisId, gpx);
            schemaUtility.UploadLocationInputData(analysisId, locationsData);

            ExecuteTrackpointExtractorJob(analysisId);

            schemaUtility.MakeHiveDatabase(analysisId);
            schemaUtility.JoinTrackLocationData(analysisId);
            schemaUtility.DropHiveDatabase(analysisId);

            ExecuteLocationMatchJob(analysisId);

            List<int> results = schemaUtility.ReadAllResultData(analysisId);
            return results;
        }

        private void ExecuteTrackpointExtractorJob(int analysisId)
        {
            string gpxInputPath = string.Format("{0}/{1}/input/track", _analysisPath, analysisId);
            string gpxOutputPath = string.Format("{0}/{1}/work/trackpoint", _analysisPath, analysisId);

            IHadoop hadoop = _hadoopConnectionsFactory.CreateHadoopConnection();

            hadoop.MapReduceJob.ExecuteJob<MrGpxTrackPointExtractor.Job>(
                new string[] { gpxInputPath, gpxOutputPath });
        }

        private void ExecuteLocationMatchJob(int analysisId)
        {
            string inputPath = string.Format("{0}/{1}/work/joinedTrackpointLocation", _analysisPath, analysisId);
            string outputPath = string.Format("{0}/{1}/output", _analysisPath, analysisId);

            IHadoop hadoop = _hadoopConnectionsFactory.CreateHadoopConnection();

            hadoop.MapReduceJob.ExecuteJob<MrLocationMatch.Job>(
                new string[] { inputPath, outputPath });
        }
    }
}
