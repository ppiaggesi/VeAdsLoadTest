using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace LoadTest
{
    [Binding]
    public class StepBase
    {
        [AfterScenario]
        private void AfterScenario()
        {
            if (Browser.Driver != null)
            {
                Browser.Quit();
            }
        }
    }
}
