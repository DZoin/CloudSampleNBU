using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace LocationMatch.DataAccess
{
    public static class GpxXDocumentUtility
    {
        public static List<Tuple<string, string>> GetTrackPoints(XDocument gpxDocument)
        {
            var result = new List<Tuple<string, string>>();

            if (gpxDocument.Root == null)
            {
                return result;
            }

            string gpxNamespace = gpxDocument.Root.Name.NamespaceName;

            XName trackpointName = XName.Get("trkpt", gpxNamespace);
            XName latAttributeName = XName.Get("lat");
            XName lonAttributeName = XName.Get("lon");
            XName timeName = XName.Get("time", gpxNamespace);

            List<XElement> trackPointElements = gpxDocument.Root.Descendants(trackpointName).ToList();

            if (trackPointElements.All(trkpt =>
                {
                    XElement timeElement = trkpt.Element(timeName);
                    if (timeElement != null && !string.IsNullOrEmpty(timeElement.Value))
                    {
                        DateTime tmp;
                        if (DateTime.TryParse(timeElement.Value, out tmp))
                        {
                            return true;
                        }
                    }

                    return false;
                }))
            {
                trackPointElements = trackPointElements.OrderBy(trkpt => DateTime.Parse(trkpt.Element(timeName).Value)).ToList();
            }

            foreach (XElement trackPointElement in trackPointElements)
            {
                string lat = trackPointElement.Attribute(latAttributeName).Value;
                string lon = trackPointElement.Attribute(lonAttributeName).Value;

                result.Add(new Tuple<string, string>(lat, lon));
            }

            return result;
        }
    }
}
