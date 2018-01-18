using System;
using System.Collections.Generic;

namespace LocationMatch.Analysis.WorkerRole
{
    public interface ILocationMatchesCalculator
    {
        List<int> CalculateLocationMatches(
            int analysisId,
            string gpx,
            List<Tuple<int, double, double, double>> locationsData);
    }
}
