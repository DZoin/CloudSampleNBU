using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using LocationMatch.Analysis.WorkerRole.MapReduceImplementation;
using LocationMatch.DataAccess;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace LocationMatch.Analysis.WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private ILocationMatchesCalculator _locationMatchesCalculator;
        private readonly string _submittedStatusString = AnalysisStatus.Submitted.ToString();
        
        private const string CALCTYPE_SETTING_NAME = "CalculatorType";
        private const string INMEMORY_CALCTYPE = "InMemory";
        private const string MAPREDUCE_CALCTYPE = "MapReduce";
        private const int ANALYSIS_TIMEOUT_IN_MINUTES = 30;
        private readonly string _timeoutMessage = string.Format("Error: Timeout. The analysis did not complete in {0} minutes.", ANALYSIS_TIMEOUT_IN_MINUTES);

        public override void Run()
        {
            Trace.TraceInformation("LocationMatch.Analysis.WorkerRole is running");

            string calcTypeSettingValue = ConfigurationManager.AppSettings[CALCTYPE_SETTING_NAME];

            if (calcTypeSettingValue == INMEMORY_CALCTYPE)
            {
                _locationMatchesCalculator = new InMemoryLocationMatchCalculator();
            }
            else if (calcTypeSettingValue == MAPREDUCE_CALCTYPE)
            {
                _locationMatchesCalculator = new MapReduceLocationMatchCalculator();    
            }
            else
            {
                throw new ConfigurationErrorsException("Configuration AppSetting 'CalculatorType' not found in config file.");
            }

            try
            {
                while (true)
                {
                    LocationMatchAnalysi analysisToExecute = getSubmittedAnalysisToExecute();

                    if (analysisToExecute != null)
                    {
                        try
                        {
                            updateAnalysis(analysisToExecute.Id, AnalysisStatus.Running, DateTime.Now, null, "Analysis started");

                            LocationList locationList = getLocationList(analysisToExecute.LocationListId);
                            Track track = getTrack(analysisToExecute.TrackId);

                            var locationData = new List<Tuple<int, double, double, double>>();

                            foreach (Location l in locationList.Locations)
                            {
                                var lData = new Tuple<int, double, double, double>(
                                    l.Id,
                                    (double)l.Latitude,
                                    (double)l.Longitude,
                                    (double)analysisToExecute.Radius);

                                locationData.Add(lData);
                            }

                            List<int> matchingLocationIds = _locationMatchesCalculator.CalculateLocationMatches(
                                analysisToExecute.Id,
                                track.Gpx,
                                locationData);

                            string result = buildResultMessage(matchingLocationIds);
                            updateAnalysis(analysisToExecute.Id, AnalysisStatus.Completed, null, DateTime.Now, result);
                        }
                        catch (Exception ex)
                        {
                            string result = string.Format(
                                "Error: {0}",
                                ex.Message);
                            updateAnalysis(analysisToExecute.Id, AnalysisStatus.Failed, null, DateTime.Now, result);
                        }
                    }

                    setAnalysisTimeouts();

                    Thread.Sleep(10000);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("LocationMatch.Analysis.WorkerRole exception: " + ex.Message);
                throw;
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;
            bool result = base.OnStart();
            Trace.TraceInformation("LocationMatch.Analysis.WorkerRole has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("LocationMatch.Analysis.WorkerRole is stopping");
            base.OnStop();
            Trace.TraceInformation("LocationMatch.Analysis.WorkerRole has stopped");
        }

        private LocationMatchAnalysi getSubmittedAnalysisToExecute()
        {
            using (var ctx = new AzureCourse_LocationMatchEntities())
            {
                LocationMatchAnalysi analysis = ctx.LocationMatchAnalysis
                    .OrderBy(a => a.TimeStarted)
                    .FirstOrDefault(a => a.Status == _submittedStatusString);
                return analysis;
            }
        }

        private void updateAnalysis(int analysisId, AnalysisStatus status, DateTime? timeStarted, DateTime? timeFinished, string result)
        {
            using (var ctx = new AzureCourse_LocationMatchEntities())
            {
                LocationMatchAnalysi analysis = ctx.LocationMatchAnalysis
                    .First(a => a.Id == analysisId);

                // If analysis in the DB is already with completed or failed status we don't update the status.
                if (analysis.Status == AnalysisStatus.Completed.ToString() || 
                    analysis.Status == AnalysisStatus.Failed.ToString())
                {
                    return;
                }

                if (timeStarted.HasValue)
                {
                    analysis.TimeStarted = timeStarted.Value;
                }
                if (timeFinished.HasValue)
                {
                    analysis.TimeFinished = timeFinished.Value;
                }

                analysis.Status = status.ToString();
                analysis.Result = result;

                ctx.SaveChanges();
            }
        }

        private Track getTrack(int trackId)
        {
            using (var ctx = new AzureCourse_LocationMatchEntities())
            {
                Track track = ctx.Tracks
                    .First(t => t.Id == trackId);

                return track;
            }
        }

        private LocationList getLocationList(int LocationListId)
        {
            using (var ctx = new AzureCourse_LocationMatchEntities())
            {
                LocationList locationList = ctx.LocationLists
                    .Include("Locations")
                    .First(ll => ll.Id == LocationListId);

                return locationList;
            }
        }

        private string buildResultMessage(List<int> matchingLocationIds)
        {
            if (matchingLocationIds == null || !matchingLocationIds.Any())
            {
                return "No matching locations";
            }

            using (var ctx = new AzureCourse_LocationMatchEntities())
            {
                List<Location> locations = ctx.Locations
                    .Where(l => matchingLocationIds.Contains(l.Id))
                    .OrderBy(l => l.Id)
                    .ToList();

                string result = string.Join("; ", locations.Select(l => l.Name));
                return result;
            }
        }

        private void setAnalysisTimeouts()
        {
            using (var ctx = new AzureCourse_LocationMatchEntities())
            {
                DateTime startedTimeForTimeoutedAnalyses = DateTime.Now.AddMinutes(-ANALYSIS_TIMEOUT_IN_MINUTES);
                string stringRunningStatus = AnalysisStatus.Running.ToString();
 
                List<LocationMatchAnalysi> timeoutedAnalysis = ctx.LocationMatchAnalysis
                    .Where(a => a.Status == stringRunningStatus &&
                           (a.TimeStarted == null || 
                           a.TimeStarted <= startedTimeForTimeoutedAnalyses ))
                    .ToList();

                if (timeoutedAnalysis.Any())
                {
                    DateTime timeFinished = DateTime.Now;
                    foreach (LocationMatchAnalysi analysis in timeoutedAnalysis)
                    {
                        analysis.TimeFinished = timeFinished;

                        analysis.Status = AnalysisStatus.Failed.ToString();
                        analysis.Result = _timeoutMessage;
                    }

                    ctx.SaveChanges();
                }
            }
        }
    }
}
