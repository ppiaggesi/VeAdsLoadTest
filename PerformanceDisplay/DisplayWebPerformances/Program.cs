using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DisplayWebPerformances
{
    class Program
    {
        static void Main(string[] args)
        {
            var iterations = new List<Iteration>();
            var numExceptions = 0;
            var numIteration = 1;
            var numIterationsToRun = 2000;
            var urlToTest = "https://veads-ci.veinteractive.net/iframe-performances.php?a=23832";
            var waitPageToBeFullyLoadedTimeoutInSeconds = 10;

            for (int numGoThroughCorrecyly = 0; numGoThroughCorrecyly < 2000;)
            {
                IWebDriver driver = null;
                try
                {
                    Console.WriteLine($"running iteration {numIteration++}, num OK={numGoThroughCorrecyly}, num KO={numExceptions}");
                    var options = new ChromeOptions();
                    options.AddArgument(@"--incognito");
                    driver = new ChromeDriver(options);
                    driver.Navigate().GoToUrl(urlToTest);
                    WaitForAjaxComplete(waitPageToBeFullyLoadedTimeoutInSeconds, driver);
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(waitPageToBeFullyLoadedTimeoutInSeconds));
                    wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
                    wait.Until(ExpectedConditions.ElementIsVisible(By.Id("veArrowRight")));

                    var resources = new List<Resource>();
                    var measures = new List<Measure>();

                    var performancesDiv = driver.FindElement(By.Id("performances"));
                    foreach (var perfElem in performancesDiv.FindElements(By.ClassName("resource")))
                    {
                        var resourceId = perfElem.GetAttribute("id");
                        var inputs = perfElem.FindElements(By.TagName("input"));
                        var startTime = 0d;
                        var duration = 0d;
                        foreach (var input in inputs)
                        {
                            if (input.GetAttribute("name") == "startTime")
                            {
                                var startTimeText = input.GetAttribute("value");
                                double.TryParse(startTimeText, out startTime);
                            }
                            else if (input.GetAttribute("name") == "duration")
                            {
                                var durationText = input.GetAttribute("value");
                                double.TryParse(durationText, out duration);
                            }
                        }

                        if (startTime > 0)
                        {
                            resources.Add(new Resource { Url = resourceId, StartTime = startTime, Duration = duration });
                        }
                    }
                    foreach (var measureElem in performancesDiv.FindElements(By.ClassName("measure")))
                    {
                        var measureId = measureElem.GetAttribute("id");
                        var inputs = measureElem.FindElements(By.TagName("input"));
                        var value = 0d;
                        foreach (var input in inputs)
                        {
                            if (input.GetAttribute("name") == "value")
                            {
                                var valueText = input.GetAttribute("value");
                                double.TryParse(valueText, out value);
                            }
                        }

                        if (value > 0)
                        {
                            measures.Add(new Measure { Id = measureId, Value = value });
                        }
                    }

                    foreach (var measure in measures)
                    {
                        Console.WriteLine($"{measure.Id}={measure.Value}");
                    }
                    foreach (var resource in resources.OrderBy(x => x.StartTime))
                    {
                        Console.WriteLine($"{resource.Url} loaded in {resource.Duration}");
                    }

                    driver.Quit();

                    iterations.Add(new Iteration { Id = numGoThroughCorrecyly + 1, Measures = measures, Resources = resources });
                    numGoThroughCorrecyly++;
                }
                catch (Exception)
                {
                    numExceptions++;
                    if (driver != null)
                        driver.Quit();
                }
            }

            File.WriteAllText("performances.json", JsonConvert.SerializeObject(iterations));
            var summary =
$@"number of Iterations={numIteration}
number OK={numIterationsToRun}
number KO={numExceptions};
average pageFullyLoaded={iterations.SelectMany(x => x.Measures.Where(y => y.Id == "pagefullyloaded")).Average(x => x.Value)}
average pageAndAllResourcesLoaded={iterations.Average(x => x.EverythingLoadedTime)}

---------------------------------------------------------------------------------------
average load times per resource:
";
            foreach (var url in iterations.SelectMany(x => x.Resources).Select(x => x.Url).Distinct())
            {
                summary +=
$@"{url} -> {iterations.SelectMany(x => x.Resources.Where(y => y.Url == url)).Average(x => x.Duration)}
";
            }
            File.WriteAllText("summary.log", summary);
        }

        public static void WaitForAjaxComplete(int maxSeconds, IWebDriver driver)
        {
            try
            {
                IWait<IWebDriver> wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
                wait.Until(_ =>
                {
                    bool isJavascriptLoaded = (bool)((IJavaScriptExecutor)driver).
                            ExecuteScript("return document.readyState").Equals("complete");
                    return isJavascriptLoaded;
                });
            }
            catch (Exception)
            {
                return;
            }

        }
    }

    public class Resource
    {
        public string Url { get; set; }
        public double StartTime { get; set; }
        public double Duration { get; set; }
    }

    public class Measure
    {
        public string Id { get; set; }
        public double Value { get; set; }
    }

    public class Iteration
    {
        public Iteration()
        {
            Measures = new List<Measure>();
            Resources = new List<Resource>();
        }
        public int Id { get; set; }
        public List<Measure> Measures { get; set; }
        public List<Resource> Resources { get; set; }

        public double EverythingLoadedTime
        {
            get
            {
                var lastItemLoaded = Resources.OrderBy(x => x.StartTime).Last();
                return lastItemLoaded.StartTime + lastItemLoaded.Duration;
            }
        }
    }
}
