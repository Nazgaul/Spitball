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
            //options.AddArgument("--headless");
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
            "learn",
            "tutor-list",
            "register",
            "signin",
            "feed",
            //"tutor",
            "studyroom"
        };

        private static readonly IEnumerable<string> SignedPaths = new[]
        {
            //"wallet",
            //"university",
            "courses",
            "dashboard",
            "feed",
            "my-content",
            "study-rooms",
            "my-sales",
            "my-followers",
            "my-purchases",
            //"my-calendar",
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
            using var conn = _fixture.DapperRepository.OpenConnection();
            var questionId = conn.QueryFirst<long>("select top 1 id from sb.question where state = 'Ok'");
            return $"question/{questionId}";
        }

        private IEnumerable<string> GetItemsUrl()
        {
            using var conn = _fixture.DapperRepository.OpenConnection();
            var documentId = conn.QueryFirst<long>("select top 1 id from sb.document where state = 'Ok' and DocumentType = 'Document' order by id desc");
            var documentCourse = conn.QueryFirst<string>("select top 1 CourseName from sb.document where state = 'Ok' and DocumentType = 'Document' order by id desc");
            var documentTitle = conn.QueryFirst<string>("select top 1 Name from sb.document where state = 'Ok' and DocumentType = 'Document' order by id desc");
            yield return $"document/{documentCourse}/{documentTitle}/{documentId}";

            var videoId = conn.QueryFirst<long>("select top 1 id from sb.document where state = 'Ok' and DocumentType = 'Video' order by id desc");
            var videoCourse = conn.QueryFirst<string>("select top 1 CourseName from sb.document where state = 'Ok' and DocumentType = 'Video' order by id desc");
            var videoTitle = conn.QueryFirst<string>("select top 1 Name from sb.document where state = 'Ok' and DocumentType = 'Video' order by id desc");
            yield return $"document/{videoCourse}/{videoTitle}/{videoId}";
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

        private IWebElement FindByClass(IWebDriver driver, string name)
        {
            return driver.FindElementByWait(By.ClassName(name));
        }

        private IWebElement FindContains(IWebDriver driver, string name)
        {
            return driver.FindElementByWait(By.XPath("//*[contains(@class, '" + name + "')]"));
        }

        private IWebElement FindSel(IWebDriver driver, string name)
        {
            return driver.FindElementByWait(By.XPath("//*[@sel='" + name + "']"));
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

                    Login(driver, UserTypeAccounts.ElementAt(0));

                    // Wait until this element is showing
                    driver.FindElementByWait(By.XPath("//*[@sel='menu']"));


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

                    Logout(driver);
                }
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
                FindByClass(driver, "logo");

                url = $"{_driver.SiteUrl.TrimEnd('/')}/{FrymoSiteUrl}";
                driver.Navigate().GoToUrl(url);

                // Check that the element is exist
                FindContains(driver, "frymo-logo");

                url = $"{_driver.SiteUrl.TrimEnd('/')}/studyroomsettings";
                driver.Navigate().GoToUrl(url);

                // Check that the element is exist
                FindByClass(driver, "logo");

                url = $"{_driver.SiteUrl.TrimEnd('/')}/studyroomsettings?site=frymo";
                driver.Navigate().GoToUrl(url);

                // Check that the element is exist
                FindContains(driver, "frymo-logo");
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

                // Waiting for this element to display
                driver.FindElementByWait(By.XPath(css));

                //var amountOfCards = driver.FindElements(By.XPath(css)).Count;                

                for (int i = 0; i < 30; i++)
                    body.SendKeys(Keys.PageDown);
                //Thread.Sleep(1000);
                var amountOfCardsAfterPaging = driver.FindElements(By.XPath(css)).Count;
                amountOfCardsAfterPaging.Should().BeGreaterThan(21);
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
                emailInput.SendKeys("elad+111@cloudents.com");
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
                Login(driver, UserTypeAccounts.ElementAt(1));

                // Wait for this element to be displayed
                driver.FindElementByWait(By.XPath("//*[@class='gH_i_r_chat']"));

                var url = $"{_driver.SiteUrl.TrimEnd('/')}/feed?culture=he-IL";
                driver.Navigate().GoToUrl(url);

                var menu = driver.FindElementByWait(By.XPath("//*[@sel='menu']"));
                menu.Click();

                // Check those elements are showing
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'v-menu__content')]"));
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'userMenu_top')]"));
                driver.FindElementByWait(By.XPath("//*[@class='userMenu_actionsList']"));
                var listItems = driver.FindElements(By.XPath("//*[@sel='menu_row']"));

                listItems.Count.Should().Be(6);

                // Check items route links
                listItems[1].GetAttribute("href").Should().Contain("profile/164516/teacher%20teacher");
                listItems[2].GetAttribute("href").Should().Contain("my-purchases");
                listItems[3].GetAttribute("href").Should().Contain("study-rooms");
                listItems[4].GetAttribute("href").Should().Be("https://help.spitball.co/he/");

                Logout(driver);
            }
        }

        [Fact]
        public void Feed_Search()
        {
            foreach (var driver in this._driver.Drivers)
            {
                driver.Manage().Window.Maximize();
                driver.Navigate().GoToUrl($"{_driver.SiteUrl.TrimEnd('/')}/feed?culture=en-US");

                var categories = driver.FindElementByWait(By.XPath("//*[@class='v-select__slot']"));
                categories.Click();

                // Wait until this element is showing
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'v-menu__content')]"));
                
                var types = driver.FindElements(By.XPath("//*[contains(@id, 'list-item-')]"));
                types[1].Click();

                driver.Url.Should().Contain("filter=");

                categories.Click();

                // Wait until this element is showing
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'v-menu__content')]"));

                types[0].Click();

                var searchBar = driver.FindElementByWait(By.XPath("//*[@class='v-text-field__slot']//input"));

                searchBar.Click();

                searchBar.SendKeys("Math" + Keys.Enter);

                driver.Url.Should().Contain("term");

                categories.Click();

                // Wait until this element is showing
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'v-menu__content')]"));

                types[1].Click();

                driver.Url.Should().Contain("term=");
                driver.Url.Should().Contain("filter=");
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

                Login(driver, UserTypeAccounts.ElementAt(0));
                driver.Navigate().GoToUrl(url);
                // Make sure this element is exist for registered user
                driver.FindElementByWait(By.XPath("//a[contains(@class, 'phoneNumberSlot')]"));
                Logout(driver);
            }
        }

        [Fact]
        public void AnalyticTest()
        {
            foreach (var driver in this._driver.Drivers)
            {
                driver.Manage().Window.Maximize();
                Login(driver, UserTypeAccounts.ElementAt(0));

                // Make sure this element is exist
                FindContains(driver, "analyticOverview");

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

                Login(driver, UserTypeAccounts.ElementAt(0));

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

                url = $"{_driver.SiteUrl.TrimEnd('/')}?dialog=buyPoints";
                driver.Navigate().GoToUrl(url);

                // Make sure this element is exist
                driver.FindElementByWait(By.XPath("//*[@class='buy-tokens-wrap']"));

                Logout(driver);
            }

        }

        [Fact(Skip ="Need to fix this test")]
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

                var url = $"{_driver.SiteUrl.TrimEnd('/')}/learn?culture=en-US";
                driver.Navigate().GoToUrl(url);

                //var teachLink = driver.FindElementByWait(By.XPath("//*[contains(@class, 'becomeTutorSlot')]"));

                //teachLink.GetAttribute("href").Should().Be(wixLink);

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

                var url = $"{_driver.SiteUrl.TrimEnd('/')}/feed?culture=en-US";
                driver.Navigate().GoToUrl(url);

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
                driver.FindElementByWait(By.XPath("//*[@class='registerDialog wrapper']"));

                Logout(driver);
            }
        }

        [Fact]
        public void BuyPointsTest()
        {
            foreach (var driver in this._driver.Drivers)
            {
                driver.Manage().Window.Maximize();

                Login(driver, UserTypeAccounts.ElementAt(0));

                // Wait until this element is showing
                driver.FindElementByWait(By.XPath("//*[@class='dashboardMain mr-md-6']"));

                driver.Navigate().GoToUrl($"{_driver.SiteUrl.TrimEnd('/')}/feed");
                
                var buyPointsBox = driver.FindElement(By.XPath("//*[contains(@class, 'buyPointsLayout_btn')]"));
                buyPointsBox.Click();

                // Check that this element exists
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'buy-dialog-wrap')]"));

                Logout(driver);
            }
        }

        [Fact]
        public void MarketingTest()
        {
            foreach (var driver in this._driver.Drivers)
            {
                driver.Manage().Window.Maximize();

                Login(driver, UserTypeAccounts.ElementAt(0));

                // Make sure those elements exist
                FindContains(driver, "uploadBtn").GetAttribute("type").Should().Be("button");
                FindByClass(driver, "dashboardSide");
                string[] elements = { "uploadContent", "spitballTips", "teacherTasks", "answerStudent" };
                foreach(var element in elements)
                {
                    FindContains(driver, element);
                }

                var letsGo = driver.FindElementByWait(By.XPath("//*[contains(@class, 'marketingTools')]//a"));

                letsGo.Click();

                // Making sure those elements display
                FindContains(driver, "marketingActions");
                FindByClass(driver, "spitballBlogs");
                FindContains(driver, "tableCoupon");

                // Make sure those buttons exist
                FindContains(driver, "marketingbtn");

                Logout(driver);
            }
        }

        [Fact]
        public void ProfileTest()
        {
            foreach (var driver in this._driver.Drivers)
            {
                foreach (var culture in Cultures)
                {
                    driver.Manage().Window.Maximize();

                    var url = $"{_driver.SiteUrl.TrimEnd('/')}/profile/159039/culture={culture}";
                    driver.Navigate().GoToUrl(url);

                    // Make sure those elements exist
                    driver.FindElementByWait(By.XPath("//*[@class='coverPhoto']"));
                    driver.FindElementByWait(By.XPath("//*[contains(@class, 'userName')]"));
                    driver.FindElementByWait(By.XPath("//*[@class='user-avatar-rect pUb_dS_img']"));
                    driver.FindElementByWait(By.XPath("//*[contains(@class, 'profileUserBox')]"));
                    driver.FindElementByWait(By.XPath("//*[contains(@class, 'shareContentProfile')]"));
                    driver.FindElementByWait(By.XPath("//*[contains(@class, 'profileUserSticky_btn')]"));
                    driver.FindElementByWait(By.XPath("//*[contains(@class, 'profileReviewsBox')]"));
                    driver.FindElementByWait(By.XPath("//button[contains(@class, 'followBtnNew')]"));
                    driver.FindElementByWait(By.Id("profileItemsBox"));
                    driver.FindElementByWait(By.XPath("//*[contains(@class, 'profileItemsBox_title')]"));
                    driver.FindElements(By.XPath("//*[contains(@class, 'profileItemsBox_content')]//a"));
                    driver.FindElementByWait(By.XPath("//*[@class='profileItemBox_pagination']"));
                    var coupon = driver.FindElementByWait(By.XPath("//*[@sel='coupon']"));

                    var comboBoxes = driver.FindElements(By.XPath("//*[@class='v-input__control']"));

                    foreach (var comboBox in comboBoxes)
                    {
                        comboBox.Click();
                    }

                    driver.FindElementByWait(By.XPath("//*[@sel='send']")).Click();
                    driver.FindElementByWait(By.XPath("//*[@sel='cancel_tutor_request']")).Click();
                    coupon.Click();
                    driver.FindElementByWait(By.XPath("//*[@class='registerDialog wrapper']"));

                    Login(driver, UserTypeAccounts.ElementAt(0));
                    driver.FindElementByWait(By.XPath("//*[@class='dashboardMain mr-md-6']"));
                    driver.Navigate().GoToUrl(url);
                    coupon = driver.FindElementByWait(By.XPath("//*[@sel='coupon']"));
                    driver.FindElementByWait(By.XPath("//*[@sel='send']")).Click();
                    coupon.Click();
                    driver.FindElementByWait(By.XPath("//*[contains(@class, 'coupon-dialog')]"));
                    Logout(driver);
                }
            }

        }

        [Fact]
        public void ReferTest()
        {
            foreach (var driver in this._driver.Drivers)
            {
                driver.Manage().Window.Maximize();

                Login(driver, UserTypeAccounts.ElementAt(0));
                driver.FindElementByWait(By.XPath("//*[@class='dashboardMain mr-md-6']"));
                var userMenu = driver.FindElementByWait(By.XPath("//*[@sel='menu']"));

                userMenu.Click();

                // Wait for this element to display
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'menuable__content')]"));
                
                var userItems = driver.FindElements(By.XPath("//*[@sel='menu_row']"));
                
                userItems[5].Click();

                // Make sure those elements exist
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'ref-block')]"));
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'share-icon-container')]"));
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'facebook-share-btn')]"));
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'twitter-share-btn')]"));
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'gmail-share-btn')]"));
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'whatsup-share-btn')]"));
                driver.FindElementByWait(By.XPath("//*[@class='link-container']"));
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'ref-bottom-section')]"));

                Logout(driver);
            }
        }

        [Fact]
        public void ItemTest()
        {
            foreach (var driver in this._driver.Drivers)
            {
                driver.Manage().Window.Maximize();
                var itemsUrl = GetItemsUrl();
                var Index = 0;

                foreach (var itemUrl in itemsUrl)
                {
                    var url = $"{_driver.SiteUrl.TrimEnd('/')}/{itemUrl}";
                    driver.Navigate().GoToUrl(url);

                    // Checking the elements on this page
                    driver.FindElementByWait(By.XPath("//*[@class='itemPage__main__document']"));
                    driver.FindElementByWait(By.XPath("//*[@class='document-header-container']"));
                    driver.FindElementByWait(By.XPath("//*[@class='flex top-row grow']"));
                    driver.FindElementByWait(By.XPath("//*[@class='flex bottom-row grow']"));
                    driver.FindElementByWait(By.XPath("//*[@class='views-cont']"));
                    //driver.FindElementByWait(By.XPath("//*[@class='views']"));
                    driver.FindElementByWait(By.XPath("//*[@class='right']"));
                    driver.FindElementByWait(By.XPath("//*[contains(@class, 'documentTitle')]"));
                    driver.FindElementByWait(By.XPath("//*[@class='shareContent']"));
                    driver.FindElements(By.XPath("//*[@class='shareContent']//button")).Count.Should().Be(4);
                    if (Index == 0)
                    {
                        driver.FindElementByWait(By.XPath("//*[contains(@class, 'v-window v-item-group')]"));
                        driver.FindElementByWait(By.XPath("//*[@class='layout mainItem__item__wrap__paging__actions']"));
                    }
                    else
                    {
                        driver.FindElementByWait(By.XPath("//*[contains(@class, 'text-center mainItem__item')]"));
                    }
                    driver.FindElementByWait(By.XPath("//*[contains(@class, 'sbCarouselRef')]"));
                    driver.FindElementByWait(By.XPath("//*[contains(@class, 'sbCarousel_btn sbCarousel-nextBtn')]"));
                    driver.FindElementByWait(By.XPath("//*[contains(@class, 'menu-area-btn')]")).Click();
                    //driver.FindElementByWait(By.XPath("//*[contains(@class, 'itemPage__side__btn')]")).Click();
                    //driver.FindElementByWait(By.XPath("//*[contains(@class, 'registerDialog wrapper')]"));

                    Index++;
                }
            }
        }

        [Fact]
        public void StudyRoomTest()
        {
            foreach (var driver in this._driver.Drivers)
            {
                driver.Manage().Window.Maximize();
                var url = $"{_driver.SiteUrl.TrimEnd('/')}/studyroom?culture=en-US";
                driver.Navigate().GoToUrl(url);

                // Checking the elements on the page
                driver.FindElementByWait(By.XPath("//*[@sel='code_editor_tab']")).Click();
                driver.FindElementByWait(By.XPath("//*[@sel='canvas_tab']")).Click();
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'container videos-wrapper')]"));
                //driver.FindElementByWait(By.XPath("//*[@class='outline-btn-share']"));
                //driver.FindElementByWait(By.XPath("//*[@sel='video_chat']")).Click();
                driver.FindElementByWait(By.XPath("//*[@sel='full_board']")).Click();
                for(int i = 1; i < 9; i++)
                {
                    driver.FindElementByWait(By.XPath($"//*[@sel='tab{i}']")).Click();
                }
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'recording_btn tutoringNavigationBtn')]"));
                driver.FindElementByWait(By.XPath("//*[@sel='pen_draw']")).Click();
                driver.FindElementByWait(By.XPath("//*[@sel='text_draw']")).Click();
                driver.FindElementByWait(By.XPath("//*[@sel='line_draw']")).Click();
                driver.FindElementByWait(By.XPath("//*[@sel='circle_draw']")).Click();
                driver.FindElementByWait(By.XPath("//*[@sel='square_draw']")).Click();
                driver.FindElementByWait(By.XPath("//*[@sel='undo_draw']"));
                driver.FindElementByWait(By.XPath("//*[@sel='text_draw']")).Click();
                driver.FindElementByWait(By.XPath("//*[@sel='color_picker']")).Click();
                driver.FindElementByWait(By.XPath("//*[@sel='clear_all_canvas']"));
                driver.FindElementByWait(By.Id("imageUpload"));
                driver.FindElementByWait(By.XPath("//*[@class='logo-container']")).Click();
                driver.SwitchTo().Alert().Accept();
            }
        }

        [Fact]
        public void ChatTest()
        {
            foreach (var driver in this._driver.Drivers)
            {
                driver.Manage().Window.Maximize();
                Login(driver, UserTypeAccounts.ElementAt(0));

                var chatIcon = driver.FindElementByWait(By.XPath("//*[@class='gH_i_r_chat']"));
                chatIcon.Click();

                // Check for this element
                var chatHeader = driver.FindElementByWait(By.XPath("//*[contains(@class, 'layout chat-header')]"));

                // Check for this element
                driver.FindElementByWait(By.XPath("//*[@class='layout general-chat-style']"));
                driver.FindElements(By.XPath("//*[@class='flex avatar-container']"));
                driver.FindElements(By.XPath("//*[@class='flex user-detail-container']"));

                driver.FindElementByWait(By.XPath("//*[contains(@class, 'minimizeIcon')]")).Click();
                chatHeader.Click();

                driver.FindElementByWait(By.XPath("//*[contains(@class, 'sbf-close-chat')]")).Click();
                chatIcon.Click();

                driver.FindElementByWait(By.XPath("//*[@class='flex avatar-container']")).Click();

                // Check for those elements exist
                driver.FindElementByWait(By.XPath("//*[@class='layout chat-header']"));
                driver.FindElementByWait(By.XPath("//*[@class='messages-body']"));
                driver.FindElementByWait(By.XPath("//*[@class='messages-input']"));
                driver.FindElementByWait(By.XPath("//*[@class='chat-upload-image']"));
                driver.FindElementByWait(By.XPath("//*[@class='chat-upload-file']"));

                var input = driver.FindElementByWait(By.XPath("//*[@class='v-text-field__slot']//textarea"));
                input.SendKeys("Testing");
                input.SendKeys(Keys.Enter);

                Logout(driver);
            }
        }

        [Fact]
        public void HeaderTest()
        {
            foreach (var driver in this._driver.Drivers)
            {
                driver.Manage().Window.Maximize();
                Login(driver, UserTypeAccounts.ElementAt(0));

                FindByClass(driver, "gH_i_r_findTutor").GetAttribute("href").Should().Contain("tutor-list");
                FindByClass(driver, "gH_i_r_intercom");
                FindByClass(driver, "gH_i_r_chat");
                FindByClass(driver, "gH_i_r_menuList");
                FindContains(driver, "router-link-active").GetAttribute("href").Should().Contain("dev.spitball.co");// driver.FindElementByWait(By.XPath("//*[@class='globalHeader_logo router-link-active']")).GetAttribute("href").Should().Be("https://dev.spitball.co/");

                Logout(driver);
           
            }
        }

        [Fact]
        public void ActionBoxTest()
        {
            foreach (var driver in this._driver.Drivers)
            {
                driver.Manage().Window.Maximize();
                Login(driver, UserTypeAccounts.ElementAt(0));

                // Check if this element exists
                FindByClass(driver, "gH_i_r_chat");

                driver.Navigate().GoToUrl($"{_driver.SiteUrl.TrimEnd('/')}/feed");

                FindSel(driver, "request").Click();
                FindSel(driver, "course_request").Click();


                // Check if this element exists
                FindContains(driver, "v-select--is-menu-active");

                FindSel(driver, "cancel_tutor_request").Click() ;

                FindSel(driver, "ask").Click();
                FindContains(driver, "v-text-field--solo-flat");

                Logout(driver);
            }
        }

        [Fact]
        public void NewHomePage()
        {
            foreach(var driver in this._driver.Drivers)
            {
                driver.Manage().Window.Maximize();
                driver.Navigate().GoToUrl($"{_driver.SiteUrl.TrimEnd('/')}");

                FindContains(driver, "btnTeach").GetAttribute("href").Should().Contain("teach.spitball.co");
                FindContains(driver, "online").GetAttribute("href").Should().Contain("studyroom");
                var paymentsLink = FindContains(driver, "payments");
                paymentsLink.GetAttribute("href").Should().Contain("help.spitball.co");
                paymentsLink.GetAttribute("target").Should().Be("_blank");
                FindContains(driver, "relevant").GetAttribute("href").Should().Contain("tutor-list");

                // Check those elements exist                
                var learn = FindContains(driver, "btnLearn");
                string[] elements = { "videoLinear", "homeQuote", "btnsTeach", "btnsLearn", "homeBoxes", "footer" };
                foreach (var element in elements)
                {
                    FindContains(driver, element);
                }

                learn.Click();

                // Wait until this element is showing
                FindContains(driver, "landingPageHP");

                driver.Url.Should().Contain("learn");
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
