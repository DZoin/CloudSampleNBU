using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Xml.Linq;
using LocationMatch.DataAccess;

namespace LocationMatch.WebSite.WebRole.Maps
{
    public class MapDrafter
    {
        public string DraftJsonMapForAnalysisResult(int analysisId)
        {
            Map map = DraftMapForAnalysisResult(analysisId);
            string json = ConvertMapToJson(map);
            return json;
        }

        public Map DraftMapForAnalysisResult(int analysisId)
        {
            var result = new Map();

            using (var ctx = new AzureCourse_LocationMatchEntities())
            {
                var analysisResult = ctx.LocationMatchAnalysis
                    .FirstOrDefault(ar => ar.Id == analysisId);

                if (analysisResult == null)
                {
                    return null;
                }

                if (analysisResult.Status != AnalysisStatus.Completed.ToString())
                {
                    return null;
                }

                var resultLocationNames = new List<string>();

                if (!string.IsNullOrEmpty(analysisResult.Result) &&
                    !analysisResult.Result.StartsWith("Error") &&
                    !analysisResult.Result.StartsWith("Analysis started") &&
                    !analysisResult.Result.StartsWith("No matching locations"))
                {
                    resultLocationNames.AddRange(analysisResult.Result.Split(";".ToArray(), StringSplitOptions.RemoveEmptyEntries)
                        .Select(name => name.Trim()));
                }

                LocationList locationList = ctx.LocationLists.Include("Locations")
                    .FirstOrDefault(ll => ll.Id == analysisResult.LocationListId);

                if (locationList != null && locationList.Locations != null && locationList.Locations.Any())
                {
                    result.Circles = new List<Circle>();

                    foreach (Location l in locationList.Locations)
                    {
                        var newCircle = new Circle();
                        newCircle.Label = l.Name;
                        newCircle.Latitude = (double)l.Latitude;
                        newCircle.Longitude = (double)l.Longitude;
                        newCircle.Radius = (double)analysisResult.Radius;
                        newCircle.IsFilled = resultLocationNames.Contains(l.Name);

                        result.Circles.Add(newCircle);
                    }
                }

                Track track = ctx.Tracks.FirstOrDefault(t => t.Id == analysisResult.TrackId);

                if (track != null && !string.IsNullOrWhiteSpace(track.Gpx))
                {
                    try
                    {
                        result.Polylines = new List<Polyline>();
                        var trackPolyline = new Polyline();

                        XDocument gpxDocument = XDocument.Parse(track.Gpx);
                        List<Tuple<string, string>> coordinates = GpxXDocumentUtility.GetTrackPoints(gpxDocument);

                        if (coordinates != null && coordinates.Any())
                        {
                            trackPolyline.Coordinates = new List<Tuple<double, double>>();

                            foreach (Tuple<string, string> stringCoordinates in coordinates)
                            {
                                double lat = double.Parse(stringCoordinates.Item1, CultureInfo.InvariantCulture);
                                double lon = double.Parse(stringCoordinates.Item2, CultureInfo.InvariantCulture);

                                trackPolyline.Coordinates.Add(new Tuple<double, double>(lat, lon));
                            }
                        }

                        result.Polylines.Add(trackPolyline);
                    }
                    catch (Exception)
                    {
                        throw new DataException("Error reading track data.");
                    }
                }
            }

            return result;
        }

        public string ConvertMapToJson(Map map)
        {
            var stream = new MemoryStream();
            var jsonSerializer = new DataContractJsonSerializer(typeof(Map));
            jsonSerializer.WriteObject(stream, map);
            stream.Position = 0;
            var streamReader = new StreamReader(stream);
            string result = streamReader.ReadToEnd();
            return result;
        }
    }
}