using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace LoadTest.Performance
{
    public static class Performance
    {
        public static List<long> PageFullyLoadedTotal = new List<long>();
        /// <summary>
        /// Calculates the and save load times.
        /// </summary>
        /// <param name="webTimings">The web timings.</param>
        public static void CalculateAndSaveLoadTimes(Dictionary<string, object> webTimings)
        {
            // Calculate and convert the page load times
            var pageLoadTimes = CalculateLoadTimes(webTimings);
            // Write the results to our xml file
            WritePerformanceTimingsToXml(pageLoadTimes);
        }

        /// <summary>
        /// Calculates the load times.
        /// </summary>
        /// <param name="webTimings">The web timings.</param>
        /// <returns></returns>
        public static PageLoadTimes CalculateLoadTimes(Dictionary<string, object> webTimings)
        {
            PageLoadTimes pageLoadTimes = new PageLoadTimes();
            

            if (webTimings != null)
            {
                long pagefullyloaded = webTimings.ContainsKey("loadEventEnd") && webTimings.ContainsKey("loadEventEnd")
                                           ? Convert.ToInt64(webTimings["loadEventEnd"]) -
                                             Convert.ToInt64(webTimings["navigationStart"])
                                           : 0;

                PageFullyLoadedTotal.Add(pagefullyloaded);

                long pageFetchTime = webTimings.ContainsKey("loadEventEnd") && webTimings.ContainsKey("loadEventEnd")
                                         ? Convert.ToInt64(webTimings["responseEnd"]) -
                                           Convert.ToInt64(webTimings["navigationStart"])
                                         : 0;

                long domComplete = webTimings.ContainsKey("loadEventEnd") && webTimings.ContainsKey("loadEventEnd")
                                       ? Convert.ToInt64(webTimings["domComplete"]) -
                                         Convert.ToInt64(webTimings["navigationStart"])
                                       : 0;

                long connect = webTimings.ContainsKey("loadEventEnd") && webTimings.ContainsKey("loadEventEnd")
                                   ? Convert.ToInt64(webTimings["connectEnd"]) -
                                     Convert.ToInt64(webTimings["navigationStart"])
                                   : 0;

                pageLoadTimes.PageFullyLoadedTime = pagefullyloaded;
                pageLoadTimes.PageFetchTime = pageFetchTime;
                pageLoadTimes.PageConnectTime = connect;
                pageLoadTimes.PageDomCompleteTime = domComplete;
            }

            return pageLoadTimes;
        }

        /// <summary>
        /// Writes the performance timings to XML.
        /// </summary>
        /// <param name="pageLoadTimes">The page load times.</param>
        public static void WritePerformanceTimingsToXml(PageLoadTimes pageLoadTimes)
        {
            // Create the xml document containe
            XmlDocument xmlDocument = new XmlDocument();

            // Create the XML Declaration, and append it to XML document
            XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", null, null);
            xmlDocument.AppendChild(xmlDeclaration);

            // Create the root element
            XmlElement root = xmlDocument.CreateElement("Performance");
            xmlDocument.AppendChild(root);

            AddElementToXml(xmlDocument, root, "PageFullyLoadedTime", pageLoadTimes.PageFullyLoadedTime);
            AddElementToXml(xmlDocument, root, "PageConnectTime", pageLoadTimes.PageConnectTime);
            AddElementToXml(xmlDocument, root, "PageDomCompleteTime", pageLoadTimes.PageDomCompleteTime);
            AddElementToXml(xmlDocument, root, "PageFetchTime", pageLoadTimes.PageFetchTime);

            // Save the file with todays date and time the test ran
            // It is currently set to use the "mydocuments" folder, but this can be changed
            string directory = @"C:\Performance\";
            string fileName = directory + DateTime.Now.Ticks + ".xml";

            // Create the directory if it doesn't exist
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            xmlDocument.Save(fileName);
        }

        /// <summary>
        /// Adds the element to XML document.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="root">The root.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        private static void AddElementToXml(XmlDocument doc, XmlElement root, string key, long value)
        {
            XmlElement xmlElement = doc.CreateElement(key);
            xmlElement.InnerText = value.ToString();

            root.AppendChild(xmlElement);
        }
    }
}
