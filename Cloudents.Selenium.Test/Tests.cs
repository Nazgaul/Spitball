using Cloudents.Infrastructure.Data.Test.IntegrationTests;
using Dapper;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            _driver.Manage().Window.Maximize();
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
        //"profile/159039",
        //"profile/160468",
        "studyroom",
        "about",
        "faq",
        "partners",
        "reps",
        "privacy",
        "terms",
        "studentFaq",
        "tutorFaq",
        "contact"
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
                yield return $"profile/{tutorId}";
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
                    body.ToString().Should().NotContain("###");
                }
            }
        }


        [Fact]
        public void MissingResourceAsk()
        {
            var url = "https://dev.spitball.co/question/2208";

            _driver.Navigate().GoToUrl(url);

            var htmlAttr = _driver.FindElement(By.TagName("html"));
            var v = _driver.Manage().Timeouts().PageLoad;
            var langValue = htmlAttr.GetAttribute("lang");
            //langValue.Should().Be(culture.Split('-')[0], "on link {0}", url);
            var body = _driver.FindElement(By.TagName("body"));

        }

        //[TearDown]
        //public void TearDown()
        //{

        //}

        [Fact]
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

        public void Dispose()
        {
            _driver.Close();
        }
    }
}
