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

        [Fact]
        public void FeedPagingTest()
        {
            foreach (var driver in this._driver.Drivers)
            {
                var url = $"{_driver.SiteUrl.TrimEnd('/')}/feed";
                driver.Manage().Window.Maximize();
                driver.Navigate().GoToUrl(url);
                var body = driver.FindElement(By.TagName("body"));

                // Waiting for this element to display
                driver.FindElementByWait(By.XPath("//*[@class='layout column']//a"));

                for (int i = 0; i < 30; i++)
                    body.SendKeys(Keys.PageDown);

                var amountOfCardsAfterPaging = driver.FindElements(By.XPath("//*[@class='layout column']//a")).Count;
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
                FindSel(driver, "email").Click();

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
                FindContains(driver, "phoneNumberSlot");

                Login(driver, UserTypeAccounts.ElementAt(0));
                driver.Navigate().GoToUrl(url);
                // Make sure this element is exist for registered user
                FindContains(driver, "phoneNumberSlot");
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
                FindContains(driver, "login-popup");

                Login(driver, UserTypeAccounts.ElementAt(0));

                // Wait until this element is showing
                FindSel(driver, "menu");

                url = $"{_driver.SiteUrl.TrimEnd('/')}?dialog=upload";
                driver.Navigate().GoToUrl(url);

                // Make sure this element is exist
                FindContains(driver, "upload-dialog");

                url = $"{_driver.SiteUrl.TrimEnd('/')}?dialog=createCoupon";
                driver.Navigate().GoToUrl(url);

                // Make sure this element is exist
                FindContains(driver, "createCouponDialog");

                url = $"{_driver.SiteUrl.TrimEnd('/')}?dialog=becomeTutor";
                driver.Navigate().GoToUrl(url);

                // Make sure this element is exist
                FindContains(driver, "become-tutor-wrap");

                url = $"{_driver.SiteUrl.TrimEnd('/')}?dialog=buyPoints";
                driver.Navigate().GoToUrl(url);

                // Make sure this element is exist
                FindByClass(driver, "buy-tokens-wrap");

                Logout(driver);
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

                ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight - 150)");

                FindContains(driver, "btn-earn").GetAttribute("href").Should().Be(wixLink);
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

                var tutorRequest = FindSel(driver, "request");

                tutorRequest.Click();

                var submitRequest = FindSel(driver, "submit_tutor_request");

                submitRequest.Click();

                // Wait until this element is showing
                driver.FindElementByWait(By.XPath("//*[@class='v-messages__message']"));

                var errorMessages = driver.FindElements(By.XPath("//*[@class='v-messages__message']"));

                errorMessages.Count.Should().Be(2);

                foreach (var error in errorMessages)
                {
                    error.Text.Should().NotBeEmpty();
                }

                var freeText = FindSel(driver, "free_text");
                var courseSelection = FindSel(driver, "course_request");
                
                freeText.SendKeys("Hi");
                courseSelection.SendKeys("Temp" + Keys.Tab);
                submitRequest.Click();

                // Make sure this element is showing
                FindContains(driver, "registerDialog");

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
                FindContains(driver, "dashboardMain");

                driver.Navigate().GoToUrl($"{_driver.SiteUrl.TrimEnd('/')}/feed");

                FindContains(driver, "buyPointsLayout_btn").Click();

                // Check that this element exists
                FindContains(driver, "buy-dialog-wrap");

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
                    FindByClass(driver, "coverPhoto");
                    FindContains(driver, "pUb_dS_img");
                    FindByClass(driver, "profileItemBox_pagination");
                    string[] elements = { "userName", "profileUserBox", "profileUserSticky_btn",
                                          "profileReviewsBox", "profileItemsBox_title",
                                          "profileItemsBox_content" , "followBtnNew"};
                    foreach(var element in elements)
                    {
                        FindContains(driver, element);
                    }
                    
                    driver.FindElementByWait(By.Id("profileItemsBox"));

                    var coupon = FindSel(driver, "coupon");

                    var comboBoxes = driver.FindElements(By.XPath("//*[@class='v-input__control']"));

                    foreach (var comboBox in comboBoxes)
                    {
                        comboBox.Click();
                    }

                    FindSel(driver, "send").Click();
                    FindSel(driver, "cancel_tutor_request").Click();
                    coupon.Click();
                    FindContains(driver, "registerDialog");

                    Login(driver, UserTypeAccounts.ElementAt(0));
                    FindContains(driver, "dashboardMain");
                    driver.Navigate().GoToUrl(url);

                    coupon = FindSel(driver, "coupon");
                    FindSel(driver, "send").Click();
                    coupon.Click();
                    FindContains(driver, "coupon-dialog");
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

                FindContains(driver, "dashboardMain");
                FindSel(driver, "menu").Click();

                // Wait for this element to display
                FindContains(driver, "menuable__content");

                var userItems = driver.FindElements(By.XPath("//*[@sel='menu_row']"));
                
                userItems[5].Click();

                // Make sure those elements exist
                string[] elements = { "ref-block", "share-icon-container", "facebook-share-btn",
                                      "twitter-share-btn", "gmail-share-btn", "whatsup-share-btn",
                                      "ref-bottom-section" };
                foreach(var element in elements)
                {
                    FindContains(driver, element);
                }
                FindByClass(driver, "link-container");

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
                    string[] elements = { "itemPage__main__document", "document-header-container",
                                          "views-cont", "right", "shareContent" };
                    foreach (var element in elements)
                    {
                        FindByClass(driver, element);
                    }
                    
                    FindContains(driver, "flex top-row");
                    FindContains(driver, "flex bottom-row");
                    FindContains(driver, "documentTitle");
                    
                    driver.FindElements(By.XPath("//*[@class='shareContent']//button")).Count.Should().Be(4);
                    if (Index == 0)
                    {
                        FindContains(driver, "v-item-group");
                        FindContains(driver, "paging__actions");
                    }
                    else
                    {
                        FindContains(driver, "mainItem__item");
                    }
                    FindContains(driver, "sbCarouselRef");
                    FindContains(driver, "sbCarousel-nextBtn");
                    FindContains(driver, "menu-area-btn");

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
                string[] elements = { "code_editor_tab", "canvas_tab", "full_board" };
                foreach(var element in elements)
                {
                    FindSel(driver, element);
                }
                FindContains(driver, "container videos-wrapper");
                
                for(int i = 1; i < 9; i++)
                {
                    FindSel(driver, $"tab{i}");
                }
                FindContains(driver, "recording_btn tutoringNavigationBtn");
                string[] moreElements = { "pen_draw", "text_draw", "line_draw", "circle_draw", "square_draw",
                                          "undo_draw", "text_draw", "color_picker", "clear_all_canvas" };
                foreach(var element in moreElements)
                {
                    FindSel(driver, element);
                }
                driver.FindElementByWait(By.Id("imageUpload"));
                FindByClass(driver, "logo-container").Click();
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

                var chatIcon = FindByClass(driver, "gH_i_r_chat");
                chatIcon.Click();

                // Check for this element
                var chatHeader = FindContains(driver, "layout chat-header");

                // Check for this element
                string[] elements = { "general-chat-style", "avatar-container", "user-detail-container" };
                foreach(var element in elements)
                {
                    FindContains(driver, element);
                }

                FindContains(driver, "minimizeIcon");
                chatHeader.Click();

                FindContains(driver, "sbf-close-chat").Click();
                chatIcon.Click();

                FindContains(driver, "avatar-container").Click();

                // Check for those elements exist
                string[] moreElements = { "sb-chat-container", "messages-body", "messages-input",
                                          "chat-upload-image", "chat-upload-file" };
                foreach(var element in moreElements)
                {
                    FindContains(driver, element);
                }

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
                FindContains(driver, "router-link-active").GetAttribute("href").Should().Contain("dev.spitball.co");

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
                string[] elements = { "videoLinear", "homeQuote", "btnsTeach", "btnsLearn",
                                      "homeBoxes", "footer" };
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
