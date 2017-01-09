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
        static IWebDriver driver;
        static void Main(string[] args)
        {
            var iterations = new List<Iteration>();
            for (int i = 0; i < 1; i++)
            {
                var options = new ChromeOptions();
                options.AddArgument(@"--incognito");
                driver = new ChromeDriver(options);

                driver.Navigate().GoToUrl("https://veads.vagrant.local/iframe-performances.php?a=1");
                WaitForAjaxComplete(20);
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
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

                iterations.Add(new Iteration { Id = i + 1, Measures = measures, Resources = resources });
            }

            File.WriteAllText("performances.log", JsonConvert.SerializeObject(iterations));
        }

        public static void WaitForAjaxComplete(int maxSeconds)
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
    }
}
