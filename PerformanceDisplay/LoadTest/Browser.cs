using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;

namespace LoadTest
{
    public class Browser
    {
        public static IWebDriver driver;

        public static IWebDriver Driver
        {
            get
            {
                if (driver == null)
                {
                    InitializeChromeDesktop();
                }

                return driver;
            }

            set { }
        }

        public static void InitializeChromeDesktop()
        {
            var options = new ChromeOptions();
            options.AddArgument(@"--incognito");

            driver = new ChromeDriver(options);



            //  driver = new ChromeDriver();
        }

        public static void Quit()
        {
            if (driver == null) return;
            driver.Quit();
            driver = null;
        }


        public static T WaitFor<T>(int timeInSeconds, Func<IWebDriver, T> condition)
        {
            IWait<IWebDriver> wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeInSeconds));
            return wait.Until(condition);
        }

    }
}
