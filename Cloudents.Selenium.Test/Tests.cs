using Cloudents.Infrastructure.Data.Test.IntegrationTests;
using Dapper;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using Xunit;

namespace Cloudents.Selenium.Test
{
    public class DriverFixture : IDisposable
    {
        public DriverFixture()
        {
            //IWebDriver _driverFirefox = new FirefoxDriver(Directory.GetCurrentDirectory());
            //IWebDriver _driverIe = new EdgeDriver(Directory.GetCurrentDirectory());



            //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            Drivers = new[] { new ChromeDriver(Directory.GetCurrentDirectory()) };
            // ... initialize data in the test database ...
        }

        public void Dispose()
        {
            foreach (var webDriver in Drivers)
            {
                webDriver.Close();
                webDriver.Dispose();
            }

            // ... clean up test data from the database ...
        }

        public IEnumerable<IWebDriver> Drivers { get; private set; }


    }
    [Collection("Database collection")]
    public class Tests : IClassFixture<DriverFixture>
    {
        //private readonly IWebDriver _driver = new ChromeDriver(Directory.GetCurrentDirectory());

        //private readonly 
        //private readonly WebDriverWait _wait;

        private readonly DatabaseFixture _fixture;
        private readonly DriverFixture _driver;

        public Tests(DatabaseFixture fixture, DriverFixture driver)
        {
            this._fixture = fixture;
            _driver = driver;
            // _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            //_wait = new WebDriverWait(_driver, new TimeSpan(0, 0, 5));
            //_driver.Manage().Window.Maximize();
            // _autoMock = AutoMock.GetLoose();
        }

        const string SiteMainUrl = "https://dev.spitball.co";
        const string FrymoSiteUrl = "?site=frymo";

        private static readonly IEnumerable<string> Cultures = new[]
        {
            "en-US",
            "he-IL"
        };

        private static readonly IEnumerable<string> RelativePaths = new[]
        {
            "",
            "tutor-list",
            "register",
            "signin",
            "feed",
            "tutor",
            "studyroom"
        };

        private static readonly IEnumerable<string> SignedPaths = new[]
        {
            "wallet",
            "university",
            "courses"
        };

        private IEnumerable<string> GetProfileUrls()
        {
            using (var conn = _fixture.DapperRepository.OpenConnection())
            {
                var tutorId = conn.QueryFirst<long>("select top 1 id from sb.tutor t, sb.userscourses c where t.state = 'Ok' and t.id = c.userid");
                yield return $"profile/{tutorId}/r";
                var userId = conn.QueryFirst<long>("Select top 1 id from sb.[user] u, sb.userscourses c where PhoneNumberConfirmed =1 and EmailConfirmed = 1  and not exists ( select id from sb.Tutor where id = u.id) and u.id = c.userid");
                yield return $"profile/{userId}/xxx";
            }
        }

        private string GetQuestionUrl()
        {
            using (var conn = _fixture.DapperRepository.OpenConnection())
            {
                var questionId = conn.QueryFirst<long>("select top 1 id from sb.question where state = 'Ok'");
                return $"question/{questionId}";
            }
        }

        [Fact]
        public void MissingResource()
        {
            foreach (var _driver in this._driver.Drivers)
            {
                foreach (var culture in Cultures)
                {
                    foreach (var site in RelativePaths.Union(GetProfileUrls()))
                    {
                        var url = $"{SiteMainUrl.TrimEnd('/')}/{site}?culture={culture}";
                        _driver.Navigate().GoToUrl(url);

                        var htmlAttr = _driver.FindElement(By.TagName("html"));

                        var langValue = htmlAttr.GetAttribute("lang");
                        langValue.Should().Be(culture.Split('-')[0], "on link {0}", url);
                        var body = _driver.FindElement(By.TagName("body"));
                        body.Text.Should().NotContain("###");
                    }

                    foreach (var site in SignedPaths.Union(GetProfileUrls()))
                    {
                        var url = $"{SiteMainUrl.TrimEnd('/')}/{site}?culture={culture}";
                        _driver.Navigate().GoToUrl(url);

                        var htmlAttr = _driver.FindElement(By.TagName("html"));

                        var langValue = htmlAttr.GetAttribute("lang");
                        langValue.Should().Be(culture.Split('-')[0], "on link {0}", url);
                        var body = _driver.FindElement(By.TagName("body"));
                        body.Text.Should().NotContain("###");
                    }
                }
            }
        }

        [Fact]
        public void LogoTest()
        {
            foreach (var _driver in this._driver.Drivers)
            {
                var url = $"{SiteMainUrl.TrimEnd('/')}";
                _driver.Navigate().GoToUrl(url);
                var logo = _driver.FindElement(By.XPath("//*[@class='logo']"));

                url = $"{SiteMainUrl.TrimEnd('/')}/{FrymoSiteUrl}";
                _driver.Navigate().GoToUrl(url);
                logo = _driver.FindElement(By.XPath("//*[@class='logo frymo-logo']"));
            }

        }

        [Fact]
        public void MissingResourceAsk()
        {
            foreach (var _driver in _driver.Drivers)
            {
                foreach (var culture in Cultures)
                {
                    var url = $"{SiteMainUrl.TrimEnd('/')}/{GetQuestionUrl()}?culture={culture}";
                    _driver.Navigate().GoToUrl(url);

                    var htmlAttr = _driver.FindElement(By.TagName("html"));
                    var langValue = htmlAttr.GetAttribute("lang");
                    langValue.Should().Be(culture.Split('-')[0], "on link {0}", url);
                    var body = _driver.FindElement(By.TagName("body"));
                    body.Text.Should().NotContain("###");
                }
            }
        }

        [Theory]
        [InlineData("feed", "//*[@class='layout column']//a")]
        public void FeedPagingTest(string relativePath, string css)
        {
            foreach (var _driver in this._driver.Drivers)
            {
                var url = $"{SiteMainUrl.TrimEnd('/')}/{relativePath}";
                _driver.Manage().Window.Maximize();
                _driver.Navigate().GoToUrl(url);
                var body = _driver.FindElement(By.TagName("body"));


                var amountOfCards = _driver.FindElements(By.XPath(css)).Count;

                for (int i = 0; i < 10; i++)
                    body.SendKeys(Keys.PageDown);
                Thread.Sleep(1000);
                var amountOfCardsAfterPaging = _driver.FindElements(By.XPath(css)).Count;
                amountOfCardsAfterPaging.Should().BeGreaterThan(amountOfCards);
            }
        }

        [Fact]
        public void LoginTest()
        {
            foreach (var _driver in this._driver.Drivers)
            {
                var url = $"{SiteMainUrl.TrimEnd('/')}/Signin";
                _driver.Navigate().GoToUrl(url);
                var emailButton = _driver.FindElement(By.XPath("//*[@sel='email']"));
                emailButton.Click();
                var emailInput = _driver.FindElement(By.Name("email"));
                emailInput.SendKeys("elad13@cloudents.com");
                var loginButton = _driver.FindElement(By.XPath("//*[@type='submit']"));
                loginButton.Click();
                var passwordInput = _driver.FindElement(By.XPath("//*[@type='password']"));
                loginButton = _driver.FindElement(By.XPath("//*[@type='submit']"));
                passwordInput.SendKeys("123456789");
                loginButton.Click();
            }
        }

        [Fact]
        public void CourseListTest()
        {
            foreach (var _driver in this._driver.Drivers)
            {

                _driver.Manage().Window.Maximize();

                foreach (var profile in GetProfileUrls())
                {
                    var url = $"{SiteMainUrl.TrimEnd('/')}/{profile}";
                    _driver.Navigate().GoToUrl(url);
                    var _wait = new WebDriverWait(_driver, new TimeSpan(0, 0, 5));
                    var course = _driver.FindElement(By.XPath("//*[@class='layout row wrap']//a"));
                    var courseTerm = course.GetAttribute("href");
                    //course.Click();
                    //wait.Until(_driver => _driver.FindElement(By.XPath("//*[@class='flex side-bar']")));
                    courseTerm.Should().Be($"{SiteMainUrl.TrimEnd('/')}/?Course={course.Text}");
                }
            }
        }

        [Fact]
        public void MenuListItemsTest()
        {
            foreach (var _driver in this._driver.Drivers)
            {
                LoginTest();

                var menu = _driver.FindElement(By.XPath("//*[@sel='menu']"));
                var listItems = _driver.FindElements(By.XPath("//*[@sel='menu_row']//a"));
                var _wait = new WebDriverWait(_driver, new TimeSpan(0, 0, 5));
                _wait.Until(driver => driver.FindElement(By.XPath("//*[@sel='menu']")));
                menu.Click();
                listItems.Count.Should().Be(8);
            }

            /*for(int i = 0; i < 5; i++)
            {
                listItems[i].GetAttribute("href").Should().Be(menuItems.ElementAt(i));
            }*/

            /*for(int i = 9; i < 12; i++)
            {
                listItems[i].GetAttribute("href").Should().Be(menuItems.ElementAt(i));
            }*/
        }

        //[Fact(Skip = "NEED TO FIX")]
        //public void SignButtonsTest()
        //{
        //    _driver.Manage().Window.Maximize();
        //    _driver.Navigate().GoToUrl($"{SiteMainUrl.TrimEnd('/')}");

        //    //var carousel = _driver.FindElements(By.XPath("//*[@class='itemsCarousel']//a"));
        //    //carousel[0].Click();

        //    var loginButton = _driver.FindElement(By.XPath("//*[@sel='sign']"));
        //    loginButton.Click();

        //    // blank page will not have class name with the word container
        //    var div = _driver.FindElements(By.XPath("//*[contains(text(),'container')]"));

        //    div.Count.Should().BeGreaterThan(1);
        //}

        public void Dispose()
        {
            // _driver.Close();
            // _driver.Dispose();
        }
    }
}
