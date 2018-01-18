using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LocationMatch.WebSite.WebRole.Maps
{
    [DataContract]
    public class Polyline
    {
        [DataMember]
        public List<Tuple<double, double>> Coordinates { get; set; }
    }
}