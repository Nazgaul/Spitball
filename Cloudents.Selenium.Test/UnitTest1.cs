using Cloudents.Infrastructure.Data.Test.IntegrationTests;
using Dapper;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace Cloudents.Selenium.Test
{
    public class UnitTest1
    {
        [Collection("Database collection")]
        public class Tests : IDisposable
        {
            DatabaseFixture fixture;

            public Tests(DatabaseFixture fixture)
            {
                this.fixture = fixture;
                driver.Manage().Window.Maximize();
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                // _autoMock = AutoMock.GetLoose();

            }

            IWebDriver driver = new ChromeDriver(@"C:\WebDrivers");
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
            "/",
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
            //"https://dev.spitball.co/wallet",
            //"https://dev.spitball.co/university/add",
            //"https://dev.spitball.co/courses/edit",
        };


            //[SetUp]
            //public void Setup()
            //{

            //}
            private IEnumerable<string> GetProfileUrls()
            {
                using (var conn = fixture.DapperRepository.OpenConnection())
                {
                    var tutorId = conn.QueryFirst<long>("select top 1 id from sb.tutor where state = 'Ok'");
                    yield return $"profile/{tutorId}";
                    var userId = conn.QueryFirst<long>("Select top 1 id from sb.[user] u where PhoneNumberConfirmed =1 and EmailConfirmed = 1  and not exists ( select id from sb.Tutor where id = u.id) ");
                    yield return $"profile/{userId}";
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
                        driver.Navigate().GoToUrl(url);

                        var htmlAttr = driver.FindElement(By.TagName("html"));

                        var langValue = htmlAttr.GetAttribute("lang");
                        langValue.Should().Be(culture.Split('-')[0], "on link {0}", url);
                        var body = driver.FindElement(By.TagName("body"));
                        body.Text.Should().NotContain("div");
                        Debug.Write(body);
                        // Assert.False(body.Text.Contains("###"));
                    }
                }

                //for (int i = 0; i < 19; i++)
                //{

                //}

                //driver.Navigate().GoToUrl(SitePages[0]);
                //var language = driver.FindElement(By.XPath("//*[@class='flex tutor-list-header-right hidden-sm-and-down']//a"));
                //language.Click();

                //for (int i = 0; i < 19; i++)
                //{
                //    driver.Navigate().GoToUrl(SitePages[i]);
                //    var body = driver.FindElement(By.TagName("body"));
                //    Assert.False(body.Text.Contains("###"));
                //}
            }

            //[TearDown]
            //public void TearDown()
            //{

            //}

            public void Dispose()
            {
                driver.Close();
            }
        }
    }
}
