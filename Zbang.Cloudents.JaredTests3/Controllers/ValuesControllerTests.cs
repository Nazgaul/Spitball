using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Cloudents.Jared.Controllers;
using Rhino.Mocks;
using Zbang.Zbox.Infrastructure.Repositories;
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
using System.Web.Http.Controllers;
using Zbang.Zbox.Infrastructure.Extensions;
using System;
using System.Collections.Generic;

namespace Zbang.Cloudents.Jared.Controllers.Tests
{
    [TestClass]
    public class ValuesControllerTests
    {
        private IZboxReadService m_ZboxReadService;
        private ValuesController controller;
        [TestInitialize]
        public void Setup()
        {
            var localStorageProvider = MockRepository.GenerateStub<ILocalStorageProvider>();

            IocFactory.IocWrapper.RegisterInstance(localStorageProvider);
            var fakeHttpContext = MockRepository.GenerateStub<HttpContextBase>();
            var fakeIdentity = new GenericIdentity("User");
            var principal = new GenericPrincipal(fakeIdentity, null);

                
            m_ZboxReadService = new ZboxReadService();
            controller = new ValuesController(m_ZboxReadService);
            controller.Request = new HttpRequestMessage();
            //controller.User.Stub(x => x.GetUserId()).Return(1);
            //controller.User.Stub(x => x.Identity).Return(new IdentityTest());
            controller.Request.SetConfiguration(new HttpConfiguration());
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

            private IIdentity _identityImplementation;
            private Claim claim;

            public string Name
            {
                get { return _identityImplementation.Name; }
            }

            public string AuthenticationType
            {
                get { return _identityImplementation.AuthenticationType; }
            }

            public bool IsAuthenticated
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

            public  IEnumerable<Claim> Claims { get; private set; }
            private IPrincipal _principalImplementation;
            public  string Name { get; }
            public  string AuthenticationType { get; }

            public bool IsInRole(string role)
            {
                return true;
            }

            public IIdentity Identity { get; set; }
        }

        [TestMethod]
        public async Task getCategoriesTextTest()
        {
            var user = new User(false);
            // user.Stub(x => x.GetUserId()).Return(1);
            //controller.User.Identity.Stub(x=>x.IsAuthenticated).Return(false);
            controller.User = user;
            int numOfDiff = 0;
            CancellationToken token = new CancellationToken();
            var b = await controller.Get(token);
            var c = await controller.Get(token);
           
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