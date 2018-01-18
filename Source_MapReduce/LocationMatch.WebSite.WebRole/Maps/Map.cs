using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LocationMatch.WebSite.WebRole.Maps
{
    [DataContract]
    public class Map
    {
        [DataMember]
        public List<Circle> Circles { get; set; }
        [DataMember]
        public List<Polyline> Polylines { get; set; }
    }
}