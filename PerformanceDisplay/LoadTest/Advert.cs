using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;

namespace LoadTest
{
    public class Advert
    {
        public Advert()
        {
            PageFactory.InitElements(Browser.Driver, this);
        }

        [FindsBy(How = How.Id, Using = "veArrowRight")]
        public IWebElement RightArrow;
    }
}
