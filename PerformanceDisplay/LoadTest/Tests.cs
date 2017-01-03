using System;
using System.Collections.Generic;
using NUnit.Framework;
using LoadTest.Performance;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Threading;

namespace LoadTest
{
    public class Tests
    {
        Advert AdvertPage = new Advert();
        long total = 0;

        [Test]
        [Repeat(101)]
        public void GetPageLoadTime()
        {

            Browser.Driver.Navigate()
                    .GoToUrl("https://veads-ci.veinteractive.net/iframe.php?a=23832");

            WaitForAjaxComplete(20);
            var wait = new WebDriverWait(Browser.Driver, TimeSpan.FromSeconds(60));
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            wait.Until(ExpectedConditions.ElementToBeClickable(AdvertPage.RightArrow));
            // Get the webTimings for our performance measurements
            Dictionary<string, object> webTimings = Browser.Driver.WebTimings();


            if (webTimings.Count == 0)
            {
                Assert.Fail();
            }
        }

        #region TearDown


        [OneTimeTearDown]
        public void FixtureTearDown()
        {
            for (int i = 1; i < LoadTest.Performance.Performance.PageFullyLoadedTotal.Count; i++)
            {
                total += LoadTest.Performance.Performance.PageFullyLoadedTotal[i];
            }

            var average = total / (LoadTest.Performance.Performance.PageFullyLoadedTotal.Count - 1);

            System.IO.File.WriteAllText(@"C:\Performance\WriteText.txt", "total: " + total.ToString() + " average: " + average.ToString());

            Console.WriteLine("this is the result - " + total);

            if (Browser.Driver != null)
                Browser.Quit();
        }


        public void WaitForAjaxComplete(int maxSeconds)
        {
            try
            {
                IWait<IWebDriver> wait = new WebDriverWait(Browser.Driver, TimeSpan.FromSeconds(2));
                wait.Until(_ =>
                {
                    bool isJavascriptLoaded = (bool)((IJavaScriptExecutor)Browser.Driver).
                            ExecuteScript("return document.readyState").Equals("complete");
                    return isJavascriptLoaded;
                });
            }
            catch (Exception)
            {
                return;
            }

        }
        #endregion
    }
}
