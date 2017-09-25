using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.ReadServices;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Ioc;
using System.Threading;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Mail;

namespace Zbang.Cloudents.Jared.Controllers.Tests
{
    [TestClass]
    public class ValuesControllerTests
    {
        private IZboxReadService m_ZboxReadService;
        private ValuesController m_Controller;
        [TestInitialize]
        public void Setup()
        {
            var localStorageProvider = MockRepository.GenerateStub<ILocalStorageProvider>();
            IocFactory.IocWrapper.RegisterInstance(localStorageProvider);
            var fakeHttpContext = MockRepository.GenerateStub<HttpContextBase>();
            var fakeIdentity = new GenericIdentity("User");
            var principal = new GenericPrincipal(fakeIdentity, null);

            m_ZboxReadService = new ZboxReadService();
            m_Controller = new ValuesController(m_ZboxReadService) {Request = new HttpRequestMessage()};
            //controller.User.Stub(x => x.GetUserId()).Return(1);
            //controller.User.Stub(x => x.Identity).Return(new IdentityTest());
            m_Controller.Request.SetConfiguration(new HttpConfiguration());
        }
        class Ide: ClaimsIdentity,IIdentity
        {
            public Ide(bool isAuth, params Claim[] claim1):base(claim1)
            {
                IsAuthenticated = isAuth;
            }

            public Ide(Claim claim)
            {
                this.claim = claim;
            }

            //private IIdentity _identityImplementation;
            private Claim claim;

            //public override string Name
            //{
            //    get { return _identityImplementation.Name; }
            //}

            //public override string AuthenticationType
            //{
            //    get { return _identityImplementation.AuthenticationType; }
            //}

            public new bool IsAuthenticated
            {
                get;
                private set;
            }
        }

        public class User : ClaimsPrincipal,IPrincipal
        {
            public User(bool auth)
            {
                Identity=new Ide(auth, new Claim("userId", "1"));

                Claims=new List<Claim>() { new Claim("userId","1")};
            }

            public new  IEnumerable<Claim> Claims { get; private set; }
            //private IPrincipal _principalImplementation;
            //public  string Name { get; }
            //public  string AuthenticationType { get; }

            public new bool IsInRole(string role)
            {
                return true;
            }

            public new IIdentity Identity { get; set; }
        }

        [TestMethod]
        public void getCategoriesTextTest_returnOK()
        {
            var user = new User(false);
            // user.Stub(x => x.GetUserId()).Return(1);
            //controller.User.Identity.Stub(x=>x.IsAuthenticated).Return(false);
            m_Controller.User = user;
            CancellationToken token = new CancellationToken();
            var b = m_Controller.Get(token);
            Assert.IsTrue(b.IsSuccessStatusCode);
        }
        //[TestMethod]
        //public async Task getCategoriesTextTestHaveAuth()
        //{

        //    int numOfDiff = 0;
        //    CancellationToken token = new CancellationToken();
        //    controller.User = new User(true);
        //    var b = await controller.Get(token);

        //    //foreach (var item in b.Content.)
        //    //{
        //    //    if (!b[item].Equals(c[item])) numOfDiff++;
        //    //}
        //    var a = 5;
        //    a = 7;
        //    var k = 6;
        //}
    }
}