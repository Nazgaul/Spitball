using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Zbox.Infrastructure.Ai
{
    public static class RegisterIoc
    {
        public static void Register()
        {
            var ioc = IocFactory.IocWrapper;
            //ioc.RegisterType<ILuisAi, LuisAi>(LifeTimeManager.Singleton);
            ioc.RegisterType<IWitAi, WitAi>(LifeTimeManager.Singleton);
            ioc.RegisterType<IIntent, HomeWorkIntent>("Homework");
            ioc.RegisterType<IIntent, StudyExamIntent>("StudyForExam");
            ioc.RegisterType<IIntent, ResearchIntent>("Research");
            ioc.RegisterType<IIntent, JoinGroupIntent>("JoinGroup");
        }
    }
}
