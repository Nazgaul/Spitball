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
    public sealed class DriverFixture : IDisposable
    {
        // private readonly Process _process;

        public DriverFixture()
        {
            var directoryName = Directory.GetCurrentDirectory();
            //var s = Directory.GetParent(directoryName);
            while (!Directory.GetFiles(directoryName, "*.sln").Any())
            {
                directoryName = Directory.GetParent(directoryName).ToString();
            }

            var applicationPath = Path.Combine(directoryName, "Cloudents.Web");
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AcceptInsecureCertificates = true;


            // _process = new Process
            //{
            //    StartInfo =
            //    {
            //        FileName = "dotnet",
            //        Arguments = "run",
            //        UseShellExecute = false,
            //        WorkingDirectory = applicationPath
            //    }
            //};
            //_process.Start();
            //SiteUrl = "https://localhost:53217/";
            SiteUrl = "https://dev.spitball.co";
            //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            Drivers = new IWebDriver[]
            {
                new ChromeDriver(Directory.GetCurrentDirectory(),options
                    /*new ChromeOptions()
                    {
                        AcceptInsecureCertificates = true
                    },TimeSpan.FromMinutes(20)*/),
                //new FirefoxDriver(Directory.GetCurrentDirectory(), new FirefoxOptions()
                //{
                //    PageLoadStrategy = PageLoadStrategy.None,
                //    AcceptInsecureCertificates = true,
                    
                //})
            };
            // ... initialize data in the test database ...
        }

        public void Dispose()
        {
            foreach (var webDriver in Drivers)
            {
                webDriver.Close();
                webDriver.Quit();
                webDriver.Dispose();
                
                //_process.CloseMainWindow();
                //_process.Close();
                //_process.Dispose();
            }

            // ... clean up test data from the database ...
        }

        public IEnumerable<IWebDriver> Drivers { get; private set; }

        public string SiteUrl { get; private set; }


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

        // const string SiteMainUrl = "https://dev.spitball.co";
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
            //"wallet",
            "university",
            "courses",
            "dashboard",
            "feed",
            "my-content",
            "study-rooms",
            "my-sales",
            "my-followers",
            "my-purchases",
            "my-calendar",
            "tutor-list"
        };

        private static readonly IEnumerable<string> UserTypeAccounts = new[]
        {
            "elad+444@cloudents.com",
            "elad+111@cloudents.com",
            "elad+333@cloudents.com"
        };

        private static readonly IEnumerable<string> UserTypeRoot = new[]
        {
            "dashboard",
            "feed",
            "tutor-list"
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

        private void Login(IWebDriver driver, String user)
        {
            var url = $"{_driver.SiteUrl.TrimEnd('/')}/Signin";
            driver.Navigate().GoToUrl(url);
            var emailButton = driver.FindElementByWait(By.XPath("//*[@sel='email']"));
            emailButton.Click();

            var emailInput = driver.FindElementByWait(By.Name("email"));
            emailInput.SendKeys(user);
            var loginButton = driver.FindElement(By.XPath("//*[@type='submit']"));
            loginButton.Click();

            var passwordInput = driver.FindElementByWait(By.XPath("//*[@type='password']"));
            loginButton = driver.FindElement(By.XPath("//*[@type='submit']"));
            passwordInput.SendKeys("123456789");
            loginButton.Click();
        }

        private void Logout(IWebDriver driver)
        {
            driver.Navigate().GoToUrl("https://dev.spitball.co/logout");
        }

        [Fact]
        public void MissingResource()
        {
            foreach (var driver in this._driver.Drivers)
            {
                foreach (var culture in Cultures)
                {
                    foreach (var site in RelativePaths.Union(GetProfileUrls()))
                    {
                        var url = $"{_driver.SiteUrl.TrimEnd('/')}/{site}?culture={culture}";
                        driver.Manage().Window.Maximize();
                        driver.Navigate().GoToUrl(url);

                        var htmlAttr = driver.FindElement(By.TagName("html"));

                        var langValue = htmlAttr.GetAttribute("lang");
                        langValue.Should().Be(culture.Split('-')[0], "on link {0}", url);
                        var body = driver.FindElement(By.TagName("body"));
                        body.Text.Should().NotContain("###");
                        body.Text.Should().NotContain("[Object Object]");
                    }

                    foreach (var site in SignedPaths.Union(GetProfileUrls()))
                    {
                        var url = $"{_driver.SiteUrl.TrimEnd('/')}/{site}?culture={culture}";
                        driver.Navigate().GoToUrl(url);

                        var htmlAttr = driver.FindElement(By.TagName("html"));

                        var langValue = htmlAttr.GetAttribute("lang");
                        langValue.Should().Be(culture.Split('-')[0], "on link {0}", url);
                        var body = driver.FindElement(By.TagName("body"));
                        body.Text.Should().NotContain("###");
                    }
                }

                Logout(driver);
            }
        }

        [Fact]
        public void LogoTest()
        {
            foreach (var driver in this._driver.Drivers)
            {
                var url = $"{_driver.SiteUrl.TrimEnd('/')}";
                driver.Navigate().GoToUrl(url);

                // Check that the element is exist
                driver.FindElement(By.XPath("//*[@class='logo']"));

                url = $"{_driver.SiteUrl.TrimEnd('/')}/{FrymoSiteUrl}";
                driver.Navigate().GoToUrl(url);

                // Check that the element is exist
                driver.FindElement(By.XPath("//*[@class='logo frymo-logo']"));

                url = $"{_driver.SiteUrl.TrimEnd('/')}/studyroomsettings";
                driver.Navigate().GoToUrl(url);

                // Check that the element is exist
                driver.FindElement(By.XPath("//*[@class='logo']"));

                url = $"{_driver.SiteUrl.TrimEnd('/')}/studyroomsettings?site=frymo";
                driver.Navigate().GoToUrl(url);

                // Check that the element is exist
                driver.FindElement(By.XPath("//*[@class='logo frymo-logo']"));
            }

        }

        [Fact]
        public void MissingResourceAsk()
        {
            foreach (var driver in _driver.Drivers)
            {
                foreach (var culture in Cultures)
                {
                    var url = $"{_driver.SiteUrl.TrimEnd('/')}/{GetQuestionUrl()}?culture={culture}";
                    driver.Navigate().GoToUrl(url);

                    var htmlAttr = driver.FindElement(By.TagName("html"));
                    var langValue = htmlAttr.GetAttribute("lang");
                    langValue.Should().Be(culture.Split('-')[0], "on link {0}", url);
                    var body = driver.FindElement(By.TagName("body"));
                    body.Text.Should().NotContain("###");
                }
            }
        }

        [Theory]
        [InlineData("feed", "//*[@class='layout column']//a")]
        public void FeedPagingTest(string relativePath, string css)
        {
            foreach (var driver in this._driver.Drivers)
            {
                var url = $"{_driver.SiteUrl.TrimEnd('/')}/{relativePath}";
                driver.Manage().Window.Maximize();
                driver.Navigate().GoToUrl(url);
                var body = driver.FindElement(By.TagName("body"));


                var amountOfCards = driver.FindElements(By.XPath(css)).Count;

                for (int i = 0; i < 10; i++)
                    body.SendKeys(Keys.PageDown);
                Thread.Sleep(1000);
                var amountOfCardsAfterPaging = driver.FindElements(By.XPath(css)).Count;
                amountOfCardsAfterPaging.Should().BeGreaterThan(amountOfCards);
            }
        }

        [Fact]
        public void LoginTest()
        {
            foreach (var driver in this._driver.Drivers)
            {
                var url = $"{_driver.SiteUrl.TrimEnd('/')}/Signin";
                driver.Navigate().GoToUrl(url);
                var emailButton = driver.FindElementByWait(By.XPath("//*[@sel='email']"));
                emailButton.Click();

                //var wait = new WebDriverWait(driver, new TimeSpan(1, 0, 7))
                //    .Until(x => x.FindElement(By.TagName("input")));

                var emailInput = driver.FindElementByWait(By.Name("email"));
                emailInput.SendKeys("elad13@cloudents.com");
                var loginButton = driver.FindElement(By.XPath("//*[@type='submit']"));
                loginButton.Click();

                var passwordInput = driver.FindElementByWait(By.XPath("//*[@type='password']"));
                loginButton = driver.FindElement(By.XPath("//*[@type='submit']"));
                passwordInput.SendKeys("123456789");
                loginButton.Click();
            }
        }

        [Fact]
        public void MenuListItemsTest()
        {
            foreach (var driver in this._driver.Drivers)
            {
                driver.Manage().Window.Maximize();
                Login(driver, "elad13@cloudents.com");

                var menu = driver.FindElementByWait(By.XPath("//*[@sel='menu']"));
                menu.Click();
                var listItems = driver.FindElements(By.XPath("//*[@sel='menu_row']//a"));
                //var _wait = new WebDriverWait(_driver, new TimeSpan(0, 0, 5));
                //_wait.Until(driver => driver.FindElement(By.XPath("//*[@sel='menu']")));

                listItems.Count.Should().Be(8);

                Logout(driver);
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

        [Fact(Skip = "Need to fix this")]
        public void Feed_Search()
        {
            foreach (var driver in this._driver.Drivers)
            {
                driver.Manage().Window.Maximize();
                Login(driver, "elad13@cloudents.com");

                // Wait for element to load, so we know that the page was loaded
                driver.FindElementByWait(By.XPath("//*[@sel='all_courses']"));

                var courses = driver.FindElements(By.XPath("//*[@class='group_list_sideMenu_course v-list-item--active v-list-item v-list-item--link theme--light']"));
                var search = driver.FindElementByWait(By.XPath("//*[@class='v-text-field__slot']//input"));


                courses[0].Click();
                search.SendKeys("test");
                search.SendKeys(Keys.Enter);

                // Wait for element to load, so we know that the page was loaded
                driver.FindElementByWait(By.XPath("//*[@class='marketing-box-component']"));

                driver.Url.Should().Contain("term=test");
                driver.Url.Should().Contain("course=");
            }
        }

        [Fact]
        public void WhatsppHeaderTest()
        {
            foreach (var driver in this._driver.Drivers)
            {
                driver.Manage().Window.Maximize();
                var url = $"{_driver.SiteUrl.TrimEnd('/')}/tutor-list";
                driver.Navigate().GoToUrl(url);

                // Make sure this element is exist for unregistered user
                driver.FindElementByWait(By.XPath("//a[contains(@class,'phoneNumberSlot')]"));

                Login(driver, "elad13@cloudents.com");
                driver.Navigate().GoToUrl(url);
                // Make sure this element is exist for registered user
                driver.FindElementByWait(By.XPath("//a[contains(@class, 'phoneNumberSlot')]"));
            }
        }

        [Fact]
        public void AnalyticTest()
        {
            foreach (var driver in this._driver.Drivers)
            {
                driver.Manage().Window.Maximize();
                Login(driver, "elad13@cloudents.com");

                // Make sure this element is exist
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'analyticOverview')]"));

                Logout(driver);
            }
        }

        [Fact]
        public void PopupWindowsTest()
        {
            foreach (var driver in this._driver.Drivers)
            {
                driver.Manage().Window.Maximize();

                var url = $"{_driver.SiteUrl.TrimEnd('/')}?dialog=login";
                driver.Navigate().GoToUrl(url);

                // Make sure this element is exist
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'login-popup')]"));

                url = $"{_driver.SiteUrl.TrimEnd('/')}?dialog=exitRegister";
                driver.Navigate().GoToUrl(url);

                // Make sure this element is exist
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'exitRegisterDialog')]"));

                Login(driver, "elad13@cloudents.com");

                // Wait until this element is showing
                driver.FindElementByWait(By.XPath("//*[@sel='menu']"));

                url = $"{_driver.SiteUrl.TrimEnd('/')}?dialog=upload";
                driver.Navigate().GoToUrl(url);

                // Make sure this element is exist
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'upload-dialog')]"));

                url = $"{_driver.SiteUrl.TrimEnd('/')}?dialog=createCoupon";
                driver.Navigate().GoToUrl(url);

                // Make sure this element is exist
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'createCouponDialog')]"));

                url = $"{_driver.SiteUrl.TrimEnd('/')}?dialog=becomeTutor";
                driver.Navigate().GoToUrl(url);

                // Make sure this element is exist
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'become-tutor-wrap')]"));

                Logout(driver);
            }

        }

        [Fact]
        public void UserTypesTest()
        {
            foreach(var driver in this._driver.Drivers)
            {
                driver.Manage().Window.Maximize();

                int index = 0;

                foreach (var user in UserTypeAccounts)
                {
                    Login(driver, user);

                    // Wait until this element is visible
                    driver.FindElementByWait(By.XPath("//*[@sel='menu']"));

                    driver.Url.Should().Contain(UserTypeRoot.ElementAt(index));

                    Logout(driver);

                    // Wait until this element is visible
                    driver.FindElementByWait(By.XPath("//*[@class='headlineSection']"));

                    index++;
                }
            }
        }

        [Fact]
        public void WixLinkTest()
        {
            var wixLink = "https://www.teach.spitball.co/";

            foreach (var driver in this._driver.Drivers)
            {
                driver.Manage().Window.Maximize();

                var url = $"{_driver.SiteUrl.TrimEnd('/')}?culture=en-US";
                driver.Navigate().GoToUrl(url);

                var teachLink = driver.FindElementByWait(By.XPath("//*[contains(@class, 'becomeTutorSlot')]"));

                teachLink.Click();

                teachLink.GetAttribute("href").Should().Be(wixLink);

                var earnButton = driver.FindElementByWait(By.XPath("//*[contains(@class, 'btn-earn')]"));

                earnButton.GetAttribute("href").Should().Be(wixLink);
            }
        }

        [Fact]
        public void TutorRequestTest()
        {
            foreach (var driver in this._driver.Drivers)
            {
                driver.Manage().Window.Maximize();

                Login(driver, UserTypeAccounts.ElementAt(1));

                //var url = $"{_driver.SiteUrl.TrimEnd('/')}/feed?culture=en-US";
                //driver.Navigate().GoToUrl(url);

                var tutorRequest = driver.FindElementByWait(By.XPath("//*[@sel='request']"));

                tutorRequest.Click();

                var submitRequest = driver.FindElementByWait(By.XPath("//*[@sel='submit_tutor_request']"));

                submitRequest.Click();

                // Wait until this element is showing
                driver.FindElementByWait(By.XPath("//*[@class='v-messages__message']"));

                var errorMessages = driver.FindElements(By.XPath("//*[@class='v-messages__message']"));

                errorMessages.Count.Should().Be(2);

                foreach (var error in errorMessages)
                {
                    error.Text.Should().NotBeEmpty();
                }

                var freeText = driver.FindElement(By.XPath("//*[@sel='free_text']"));
                freeText.SendKeys("Hi");

                var courseSelection = driver.FindElement(By.XPath("//*[@sel='course_request']"));
                courseSelection.SendKeys("Temp");
                courseSelection.SendKeys(Keys.Tab);

                submitRequest.Click();
                
                // Make sure this element is showing
                driver.FindElementByWait(By.XPath("//*[@class='tutorRequest-success-middle']"));

                Logout(driver);
            }
        }

        [Fact]
        public void BuyPointsTest()
        {
            foreach (var driver in this._driver.Drivers)
            {
                driver.Manage().Window.Maximize();

                Login(driver, UserTypeAccounts.ElementAt(1));
                
                // Wait until this element is showing
                driver.FindElementByWait(By.XPath("//*[@class='sec-result']"));

                var buyPointsBox = driver.FindElement(By.XPath("//*[contains(@class, 'buyPointsFeed')"));
                buyPointsBox.Click();

                var buyPointsButton = driver.FindElementByWait(By.XPath("//*[contains(@class, 'buyPointsLayout_btn')]"));

                buyPointsButton.Click();

                // Check that this element exist
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'buy-dialog-wrap')]"));

                Logout(driver);
            }
        }
    }

    public static class SeleniumExtensions
    {
        public static IWebElement FindElementByWait(this IWebDriver driver, By by)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            return wait.Until(x =>
            {
                try
                {
                    return x.FindElement(@by);
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            });

            //return driver.FindElement(by);
        }       
    }
}
