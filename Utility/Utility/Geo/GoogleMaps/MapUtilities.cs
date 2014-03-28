using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Xml;

namespace Utility.Geo.GoogleMaps
{
    public class MapUtilities
    {
        private int _throttlingThreshold;
        private const int MaxRetries = 5;
        private bool _underQuota;

        public MapUtilities()
        {
            _throttlingThreshold = 0;
            _underQuota = true;

        }

        /// <summary>
        /// returns driving distance in miles
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static double GetDrivingDistanceInMiles(string origin, string destination)
        {
            string url = @"http://maps.googleapis.com/maps/api/distancematrix/xml?origins=" +
              origin + "&destinations=" + destination +
              "&mode=driving&sensor=false&language=en-EN&units=imperial";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader sreader = new StreamReader(dataStream);
            string responsereader = sreader.ReadToEnd();
            response.Close();

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(responsereader);


            if (xmldoc.GetElementsByTagName("status")[0].ChildNodes[0].InnerText == "OK")
            {
                XmlNodeList distance = xmldoc.GetElementsByTagName("distance");
                return Convert.ToDouble(distance[0].ChildNodes[1].InnerText.Replace(" mi", ""));
            }

            return 0;
        }

        /// <summary>
        /// returns latitude 
        /// </summary>
        /// <param name="addresspoint"></param>
        /// <returns></returns>
        private double GetCoordinatesLat(string addresspoint)
        {
            using (var client = new WebClient())
            {
                string seachurl = "http://maps.google.com/maps/geo?q='" + addresspoint + "'&output=csv";
                string[] geocodeInfo = client.DownloadString(seachurl).Split(',');
                return (Convert.ToDouble(geocodeInfo[2]));
            }
        }

        /// <summary>
        /// returns longitude 
        /// </summary>
        /// <param name="addresspoint"></param>
        /// <returns></returns>
        private double GetCoordinatesLng(string addresspoint)
        {
            using (var client = new WebClient())
            {
                string seachurl = "http://maps.google.com/maps/geo?q='" + addresspoint + "'&output=csv";
                string[] geocodeInfo = client.DownloadString(seachurl).Split(',');
                return (Convert.ToDouble(geocodeInfo[3]));
            }
        }

        public bool GetCoordinates(string address, out double lat, out double lng)
        {
            var retVal = false;
            lat = 0;
            lng = 0;

            _throttlingThreshold++;
            if (_throttlingThreshold == 1)
            {
                Thread.Sleep(100);
                _throttlingThreshold = 0;
            }

            if (string.IsNullOrEmpty(address) == false && _underQuota)
            {

                var searchUrl = "http://maps.googleapis.com/maps/api/geocode/xml?address=" + address + "&sensor=false";

                try
                {
                    for (var attempt = 1; attempt <= MaxRetries; attempt++)
                    { 

                    var request = (HttpWebRequest) WebRequest.Create(searchUrl);
                    var response = request.GetResponse();

                        using (var dataStream = response.GetResponseStream())
                        {
                            var sreader = new StreamReader(dataStream);
                            var responsereader = sreader.ReadToEnd();
                            response.Close();

                            XmlDocument xmldoc = new XmlDocument();
                            xmldoc.LoadXml(responsereader);

                            if (xmldoc.GetElementsByTagName("status")[0].ChildNodes[0].InnerText == "OK")
                            {
                                XmlNodeList locElement = xmldoc.GetElementsByTagName("location");

                                if (double.TryParse(locElement[0].ChildNodes[0].InnerText, out lat))
                                {
                                    if (double.TryParse(locElement[0].ChildNodes[1].InnerText, out lng))
                                        retVal = true;
                                }

                                attempt = MaxRetries;

                            }
                            else if (xmldoc.GetElementsByTagName("status")[0].ChildNodes[0].InnerText ==
                                     "OVER_QUERY_LIMIT")
                            {
                                Thread.Sleep(2000);
                                if (attempt + 1 == MaxRetries)
                                {
                                    _underQuota = false;
                                    //throw new Exception("Google Quota Reached for this 24 hour period");
                                }
                            }
                            else
                            {
                                var x = "bad address";
                                attempt = MaxRetries;
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    retVal = false;
                }

            }

            return retVal;
        }

    }
}
