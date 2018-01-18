using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using Microsoft.Hadoop.MapReduce;

namespace LocationMatch.Analysis.WorkerRole.MapReduceImplementation
{
    namespace MrLocationMatch
    {
        public class Mapper : MapperBase
        {
            public override void Map(string inputLine, MapperContext context)
            {
                if (string.IsNullOrWhiteSpace(inputLine))
                {
                    return;
                }

                string[] lineParts = inputLine.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                string id = lineParts[0];
                string lat1 = lineParts[1];
                string lon1 = lineParts[2];
                string radius = lineParts[3];

                string lat2 = lineParts[4];
                string lon2 = lineParts[5];

                double dlat1 = double.Parse(lat1, CultureInfo.InvariantCulture);
                double dlon1 = double.Parse(lon1, CultureInfo.InvariantCulture);
                double dlat2 = double.Parse(lat2, CultureInfo.InvariantCulture);
                double dlon2 = double.Parse(lon2, CultureInfo.InvariantCulture);
                double dradius = double.Parse(radius, CultureInfo.InvariantCulture);

                var gc1 = new GeoCoordinate(dlat1, dlon1);
                var gc2 = new GeoCoordinate(dlat2, dlon2);

                double distance = gc1.GetDistanceTo(gc2);

                if (distance <= dradius)
                {
                    context.EmitKeyValue(id, "match");
                }
            }
        }

        public class ReducerCombiner : ReducerCombinerBase
        {
            public override void Reduce(string key, IEnumerable<string> values, ReducerCombinerContext context)
            {
                context.EmitLine(key);
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
