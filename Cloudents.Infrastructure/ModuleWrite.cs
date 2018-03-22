using System.Reflection;
using Autofac;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Write;
using Microsoft.Azure.Search;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure
{
    //public class ModuleWrite : Module
    //{
    //    protected override void Load(ContainerBuilder builder)
    //    {
    //        base.Load(builder);
    //        builder.RegisterModule<ModuleInfrastructureBase>();
    //        builder.RegisterModule<ModuleMail>();

    //        //builder.RegisterGeneric(typeof(SearchServiceWrite<>));
    //        builder.RegisterType<DownloadFile>().As<IDownloadFile>();
    //        //builder.RegisterType<JobCareerBuilder>().Keyed<IUpdateAffiliate>(AffiliateProgram.CareerBuilder);
    //        builder.RegisterType<JobWayUp>().Keyed<IUpdateAffiliate>(AffiliateProgram.WayUp);
    //        builder.RegisterType<TutorWyzant>().Keyed<IUpdateAffiliate>(AffiliateProgram.Wyzant);
    //    }
    //}

    [ModuleRegistration(Core.Enum.System.Console)]
    [ModuleRegistration(Core.Enum.System.Function)]
    public class ModuleAzureSearch : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(SearchServiceWrite<>));
            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(ISearchServiceWrite<>)).AsImplementedInterfaces();
            //builder.RegisterType<UniversitySearchWrite>().AsSelf().As<ISearchServiceWrite<University>>().SingleInstance();
            //builder.RegisterType<CourseSearchWrite>().AsSelf().As<ISearchServiceWrite<Course>>().SingleInstance();
            //builder.RegisterType<JobSearchWrite>().AsSelf().As<ISearchServiceWrite<Job>>().SingleInstance();
            //builder.RegisterType<TutorSearchWrite>().AsSelf().As<ISearchServiceWrite<Tutor>>().SingleInstance();
            builder.Register(c =>
                {
                    var key = c.Resolve<IConfigurationKeys>().Search;
                    return new SearchServiceClient(key.Name, new SearchCredentials(key.Key));
                })
                .SingleInstance().AsSelf().As<ISearchServiceClient>();
        }
    }
}