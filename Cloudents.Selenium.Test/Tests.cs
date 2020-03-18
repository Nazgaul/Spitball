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
                Login(driver, UserTypeAccounts.ElementAt(1));

                var url = $"{_driver.SiteUrl.TrimEnd('/')}/feed?culture=he-IL";
                driver.Navigate().GoToUrl(url);

                var menu = driver.FindElementByWait(By.XPath("//*[@sel='menu']"));
                menu.Click();
                var listItems = driver.FindElements(By.XPath("//*[@class='userMenu_actionsList']//a"));

                listItems.Count.Should().Be(11);

                // Check items route links
                listItems[0].GetAttribute("href").Should().Be("https://dev.spitball.co/tutor-list");
                listItems[1].GetAttribute("href").Should().Be("https://teach.spitball.co/");
                listItems[2].GetAttribute("href").Should().Be("https://dev.spitball.co/studyroom");
                listItems[3].GetAttribute("href").Should().Be("https://help.spitball.co/he/%D7%A9%D7%90%D7%9C%D7%95%D7%AA-%D7%A0%D7%A4%D7%95%D7%A6%D7%95%D7%AA");
                listItems[5].GetAttribute("href").Should().Be("https://help.spitball.co/he/article/%D7%94%D7%9B%D7%9C-%D7%A2%D7%9C%D7%99%D7%A0%D7%95");
                listItems[7].GetAttribute("href").Should().Be("https://help.spitball.co/en/article/terms-of-service");

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

                var buyPointsBox = driver.FindElement(By.XPath("//*[contains(@class, 'buyPointsFeed')]"));
                buyPointsBox.Click();

                var buyPointsButton = driver.FindElementByWait(By.XPath("//*[contains(@class, 'buyPointsLayout_btn')]"));

                buyPointsButton.Click();

                // Check that this element exist
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
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'uploadContent')]"));
                driver.FindElementByWait(By.XPath("//button[contains(@class, 'uploadBtn')]"));
                driver.FindElementByWait(By.XPath("//div[contains(@class, 'spitballTips')]"));
                driver.FindElementByWait(By.XPath("//*[@class='dashboardSide']"));
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'teacherTasks')]"));
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'answerStudent')]"));

                var letsGo = driver.FindElementByWait(By.XPath("//*[contains(@class, 'marketingTools')]//a"));

                letsGo.Click();

                // Making sure those elements display
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'marketingActions')]"));
                driver.FindElementByWait(By.XPath("//*[@class='spitballBlogs']"));
                driver.FindElementByWait(By.XPath("//*[contains(@class, 'tableCoupon')]"));

                // Make sure those buttons exist
                driver.FindElements(By.XPath("//button[contains(@class, 'marketingbtn')]"));

                Logout(driver);
            }
        }

        [Fact]
        public void ProfileTest()
        {
            foreach (var driver in this._driver.Drivers)
            {
                driver.Manage().Window.Maximize();

                var url = $"{_driver.SiteUrl.TrimEnd('/')}/profile/159489";
                driver.Navigate().GoToUrl(url);

                // Make sure those elements exist
                driver.FindElementByWait(By.XPath("//*[@class='profile-sticky']"));
                driver.FindElementByWait(By.XPath("//*[@class='shareContent']"));
                driver.FindElementByWait(By.XPath("//*[@class='profileUserBox']"));
                driver.FindElementByWait(By.Id("profileItemsBox"));
                driver.FindElementByWait(By.XPath("//*[@class='profileReviewsBox']"));
                driver.FindElementByWait(By.XPath("//*[@class='profileUserSticky_btns']//button"));
                driver.FindElementByWait(By.XPath("//button[contains(@class, 'followBtn')]"));
                driver.FindElementByWait(By.XPath("//*[@sel='coupon']"));

                var comboBoxes = driver.FindElements(By.XPath("//*[@class='v-input__control']"));

                foreach(var comboBox in comboBoxes)
                {
                    comboBox.Click();
                }
            }

        }

        [Fact]
        public void ReferTest()
        {
            foreach (var driver in this._driver.Drivers)
            {
                driver.Manage().Window.Maximize();
                Login(driver, UserTypeAccounts.ElementAt(1));

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
                    driver.FindElementByWait(By.XPath("//*[@class='views']"));
                    driver.FindElementByWait(By.XPath("//*[@class='right']"));
                    driver.FindElementByWait(By.XPath("//*[@class='sticky-item']"));
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
                    driver.FindElementByWait(By.XPath("//*[contains(@class, 'itemPage__side__btn')]")).Click();
                    driver.FindElementByWait(By.XPath("//*[contains(@class, 'login-popup')]"));

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
                driver.FindElementByWait(By.XPath("//*[@class='outline-btn-share']"));
                driver.FindElementByWait(By.XPath("//*[@sel='video_chat']")).Click();
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
