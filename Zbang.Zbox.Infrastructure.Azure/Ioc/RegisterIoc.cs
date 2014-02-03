using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.MediaServices;

namespace Zbang.Zbox.Infrastructure.Azure.Ioc
{
    public static class RegisterIoc
    {
        public static void Register()
        {
            var ioc = IocFactory.Unity;
            ioc.RegisterType<IMediaSevicesProvider, MediaSevicesProvider>(LifeTimeManager.Singleton);
        }
    }
}
