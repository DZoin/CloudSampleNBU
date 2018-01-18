using System.Runtime.Serialization;

namespace LocationMatch.WebSite.WebRole.Maps
{
    [DataContract]
    public class Circle
    {
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public double Radius { get; set; }
        [DataMember]
        public string Label { get; set; }
        [DataMember]
        public bool IsFilled { get; set; }
    }
}