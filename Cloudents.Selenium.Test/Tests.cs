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
using Xunit;

namespace Cloudents.Selenium.Test
{
    [Collection("Database collection")]
    public class Tests : IDisposable
    {
        private readonly IWebDriver _driver = new ChromeDriver(Directory.GetCurrentDirectory());

        private readonly DatabaseFixture _fixture;

        public Tests(DatabaseFixture fixture)
        {
            this._fixture = fixture;
            //_driver.Manage().Window.Maximize();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            // _autoMock = AutoMock.GetLoose();

        }

        const string SiteMainUrl = "https://dev.spitball.co";

        private static readonly IEnumerable<string> Cultures = new[]
        {
        "en-US",
        "he-IL"
    };
        //TODO: need to check
        // 404
        // 
        private static readonly IEnumerable<string> RelativePaths = new[]
        {
        "",
        "tutor-list",
        "register",
        "signin",
        "ask",
        "note",
        "tutor",
        "studyroom"
        //"wallet",
        //"university",
        //"courses"
    };


        //[SetUp]
        //public void Setup()
        //{

        //}
        private IEnumerable<string> GetProfileUrls()
        {
            using (var conn = _fixture.DapperRepository.OpenConnection())
            {
                var tutorId = conn.QueryFirst<long>("select top 1 id from sb.tutor where state = 'Ok'");
                yield return $"profile/{tutorId}/r";
                var userId = conn.QueryFirst<long>("Select top 1 id from sb.[user] u where PhoneNumberConfirmed =1 and EmailConfirmed = 1  and not exists ( select id from sb.Tutor where id = u.id) ");
                yield return $"profile/{userId}/xxx";
            }
        }

        [Fact]
        public void MissingResource()
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
            }
        }

        [Fact]
        public void LogoTest()
        {
            var url = $"{SiteMainUrl.TrimEnd('/')}";
            _driver.Navigate().GoToUrl(url);
            var logo = _driver.FindElement(By.XPath("//*[@class='logo']"));

            url = $"{SiteMainUrl.TrimEnd('/')}/?site=frymo";
            _driver.Navigate().GoToUrl(url);
            logo = _driver.FindElement(By.XPath("//*[@class='logo frymo-logo']"));
            
        }

        [Fact(Skip ="Need to finish this")]
        public void MissingResourceAsk()
        {
            var url = "https://dev.spitball.co/question/2208";

            _driver.Navigate().GoToUrl(url);

            var htmlAttr = _driver.FindElement(By.TagName("html"));
            //var v = _driver.Manage().Timeouts().PageLoad;
            var langValue = htmlAttr.GetAttribute("lang");
            //langValue.Should().Be(culture.Split('-')[0], "on link {0}", url);
            var body = _driver.FindElement(By.TagName("body"));
            var x = body.Text.ToString();


        }

        //[TearDown]
        //public void TearDown()
        //{

        //}

        [Fact]
        public void FeedPagingTest()
        {
            var url = $"{SiteMainUrl.TrimEnd('/')}/feed";
            _driver.Navigate().GoToUrl(url);
            var body = _driver.FindElement(By.TagName("body"));
            for(int i = 0; i < 10; i++)
                body.SendKeys(Keys.PageDown);
            Thread.Sleep(1000);
            var feedCards = _driver.FindElements(By.XPath("//*[@class='d-block note-block cell']"));
            feedCards.Count.Should().BeGreaterThan(14);

            url = $"{SiteMainUrl.TrimEnd('/')}/tutor";
            _driver.Navigate().GoToUrl(url);
            body = _driver.FindElement(By.TagName("body"));
            for (int i = 0; i < 10; i++)
                body.SendKeys(Keys.PageDown);
            Thread.Sleep(1000);
            var tutorCards = _driver.FindElements(By.XPath("//*[@class='tutor-result-card-mobile pa-2 ma-2 justify-space-between mb-2']"));
            tutorCards.Count.Should().BeGreaterThan(20);
        }

        [Fact(Skip = "Not sure what need to be tested here")]
        public void SignTests()
        {
            var url = $"{SiteMainUrl.TrimEnd('/')}/register";
            _driver.Navigate().GoToUrl(url);
            var googleButton = _driver.FindElement(By.XPath("//*[@class='google elevation-5 btn-login v-btn v-btn--large v-btn--round theme--light']"));
            var emailButton = _driver.FindElement(By.XPath("//*[@class='email v-btn v-btn--flat v-btn--large v-btn--round theme--light']"));
            var checkBox = _driver.FindElement(By.XPath("//*[@id='checkBox']"));
            googleButton.Click();
            checkBox.Click();
            googleButton.Click();
            _driver.SwitchTo().Window(_driver.WindowHandles.Last());
            _driver.Close();
            _driver.SwitchTo().Window(_driver.CurrentWindowHandle);
            //driver.Quit();
            emailButton.Click();
        }

        [Fact]
        public void LoginTest()
        {
            var url = $"{SiteMainUrl.TrimEnd('/')}/Signin";
            _driver.Navigate().GoToUrl(url);
            var emailButton = _driver.FindElement(By.XPath("//*[@class='email v-btn v-btn--flat v-btn--large v-btn--round theme--light']"));
            emailButton.Click();
            var emailInput = _driver.FindElement(By.XPath("//*[@class='input-field errorTextStr']"));
            emailInput.SendKeys("elad13@cloudents.com");
            var loginButton = _driver.FindElement(By.XPath("//*[@class='white--text btn-login v-btn v-btn--large v-btn--round theme--light']"));
            loginButton.Click();
            var passwordInput = _driver.FindElement(By.XPath("//*[@class='mt-4 widther input-wrapper']//input"));
            loginButton = _driver.FindElement(By.XPath("//*[@class='white--text btn-login v-btn v-btn--large v-btn--round theme--light']"));
            passwordInput.SendKeys("123456789");
            loginButton.Click();
        }

        public void Dispose()
        {
            _driver.Close();
        }
    }
}
