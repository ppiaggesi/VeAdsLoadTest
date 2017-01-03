using LoadTest.Performance;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using Shouldly;
using NUnit.Framework;

namespace LoadTest.Specflow
{
    [Binding]
    [Parallelizable]
    [SingleThreaded]
    public class AdvertLoadTestSteps
    {
        Dictionary<string, object> webTimings;

       [Given(@"I have navigated to advert link")]
        public void GivenIHaveNavigatedToAdvertLink()
        {
            Browser.Driver.Navigate().GoToUrl("https://veads-ci.veinteractive.net/iframe.php?a=23832");
            //Browser.NavigateTo("https://veads-ci.veinteractive.net/iframe.php?a=23832");
        }

        [When(@"I get the webTimings for our performance measurements")]
        public void WhenIGetTheWebTimingsForOurPerformanceMeasurements()
        {
            // Get the webTimings for our performance measurements
            webTimings = Browser.driver.WebTimings();
        }

        [Then(@"the webTiming result should be greater than zero")]
        public void ThenTheWebTimingResultShouldBeGreaterThanZero()
        {
            
            webTimings.Count.ShouldBeSameAs(1230);
        }

    }
}
