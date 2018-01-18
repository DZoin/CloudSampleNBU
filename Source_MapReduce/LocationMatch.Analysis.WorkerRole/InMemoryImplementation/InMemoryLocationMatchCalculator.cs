using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using LocationMatch.DataAccess;

namespace LocationMatch.Analysis.WorkerRole
{
    public class InMemoryLocationMatchCalculator : ILocationMatchesCalculator
    {
        public List<int> CalculateLocationMatches(
            int analysisId,
            string gpx,
            List<Tuple<int, double, double, double>> locationsData)
        {
            XDocument gpxDocument = XDocument.Parse(gpx, LoadOptions.None);

            List<Tuple<string, string>> trackPointLatLongs = GpxXDocumentUtility.GetTrackPoints(gpxDocument);
            var results = new List<int>();

            foreach (Tuple<string, string> trackPointLatLong in trackPointLatLongs)
            {
                foreach (var locationData in locationsData)
                {
                    double trackPointLat = double.Parse(trackPointLatLong.Item1, CultureInfo.InvariantCulture);
                    double trackPointLon = double.Parse(trackPointLatLong.Item2, CultureInfo.InvariantCulture);

                    var trackPointCoordinate = new GeoCoordinate(trackPointLat, trackPointLon);
                    var locationCoordinate = new GeoCoordinate(locationData.Item2, locationData.Item3);

                    double distance = trackPointCoordinate.GetDistanceTo(locationCoordinate);

                    if (distance <= locationData.Item4)
                    {
                        results.Add(locationData.Item1);
                    }
                }
            }

            return results;
        }
    }
}
