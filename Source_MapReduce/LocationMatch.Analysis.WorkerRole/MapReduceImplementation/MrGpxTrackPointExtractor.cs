using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using LocationMatch.DataAccess;
using Microsoft.Hadoop.MapReduce;

namespace LocationMatch.Analysis.WorkerRole.MapReduceImplementation
{
    namespace MrGpxTrackPointExtractor
    {
        public class Mapper : MapperBase
        {
            public override void Map(string inputLine, MapperContext context)
            {
                XDocument gpxDocument = XDocument.Parse(inputLine, LoadOptions.None);
                List<Tuple<string, string>> trackPointLatLongs = GpxXDocumentUtility.GetTrackPoints(gpxDocument);
                var dictValuesToKeys = new Dictionary<string, string>();

                foreach (Tuple<string, string> trackPointLatLong in trackPointLatLongs)
                {
                    string value = string.Format("{0} {1}", trackPointLatLong.Item1, trackPointLatLong.Item2);

                    double dlat = double.Parse(trackPointLatLong.Item1, CultureInfo.InvariantCulture);
                    double dlon = double.Parse(trackPointLatLong.Item2, CultureInfo.InvariantCulture);
                    string key = Convert.ToInt32(Math.Floor(dlat * 10)).ToString() +
                                 Convert.ToInt32(Math.Floor(dlon * 10)).ToString();

                    dictValuesToKeys[value] = key;
                }

                foreach (var valueToKey in dictValuesToKeys)
                {
                    context.EmitKeyValue(valueToKey.Value, valueToKey.Key);
                }
            }
        }

        public class ReducerCombiner : ReducerCombinerBase
        {
            public override void Reduce(string key, IEnumerable<string> values, ReducerCombinerContext context)
            {
                values = values.Distinct();

                foreach (string value in values)
                {
                    context.EmitLine(value);
                }
            }
        }

        public class Job : MrJob<Mapper, ReducerCombiner>
        {
            public Job()
            {
            }

            public Job(string inputPath, string outputPath)
            {
            }
        }
    }
}
