using System.Text;
using System.Web.Mvc;

namespace PerformanceDisplay.Utils
{
    public static class GoogleChart
    {
        /// <summary>
        /// Draws the chart.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="chartType">Type of the chart.</param>
        /// <param name="chartData">The chart data.</param>
        /// <param name="chartSize">Size of the chart.</param>
        /// <param name="chartLabel">The chart label.</param>
        /// <returns></returns>
        public static string DrawChart(this HtmlHelper helper, string chartType, string chartData, string chartSize, string chartLabel)
        {
            StringBuilder chartHtml = new StringBuilder("<img src='http://chart.apis.google.com/chart?chs=");
            chartHtml.Append(chartSize);
            chartHtml.Append("&amp;chxt=x,y");
            chartHtml.Append("&amp;cht=");
            chartHtml.Append(chartType);
            chartHtml.Append("&amp;chls=2.0");
            chartHtml.Append("&amp;chxl=0:|Jan|Feb|Mar|Apr|May");

            // Data
            chartHtml.Append("&amp;chd=");
            chartHtml.Append("s: cEj9U");

            //chartHtml.Append("&amp;chl=");
            //chartHtml.Append(chartLabel);
            chartHtml.Append("' />");

            return chartHtml.ToString();

            //<img src="https://chart.googleapis.com/chart?
            //chxt=x,y ------
            //cht=bvs ------
            //chd=s:cEj9U
            //chco=76A4FB
            //chls=2.0 ------
            //chs=200x125 ------
            //chxl=0:|Jan|Feb|Mar|Apr|May"
        }
    }
}