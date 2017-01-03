using System.Collections.Generic;
using OpenQA.Selenium;

namespace LoadTest.Performance
{
    public static class Extensions
    {
        public static Dictionary<string, object> WebTimings(this IWebDriver driver)
        {
            const string scriptToExecute =
                "var performance = window.performance || window.mozPerformance || window.msPerformance || window.webkitPerformance || {}; var timings = performance.timing || {}; return timings;";

            var webTiming = (Dictionary<string, object>)((IJavaScriptExecutor)driver)
                .ExecuteScript(scriptToExecute);

            // Calculate & Save the load times to an xml file
            Performance.CalculateAndSaveLoadTimes(webTiming);

            return webTiming;
        }
    }
}
