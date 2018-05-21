using System.Reflection;
using Autofac;
using Cloudents.Core.Attributes;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Write;
using Cloudents.Infrastructure.Write.Job;
using Cloudents.Infrastructure.Write.Tutor;
using JetBrains.Annotations;
using Microsoft.Azure.Search;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure
{
    [ModuleRegistration(Core.Enum.System.Console)]
    [ModuleRegistration(Core.Enum.System.WorkerRole)]
    public class ModuleWrite : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            //builder.RegisterGeneric(typeof(SearchServiceWrite<>));
            builder.RegisterType<DownloadFile>().As<IDownloadFile>();
            //builder.RegisterType<JobCareerBuilder>().Keyed<IUpdateAffiliate>(AffiliateProgram.CareerBuilder);
            builder.RegisterType<JobWayUp>().Keyed<IUpdateAffiliate>(AffiliateProgram.WayUp);
            builder.RegisterType<TutorWyzant>().Keyed<IUpdateAffiliate>(AffiliateProgram.Wyzant);
        }
    }
}