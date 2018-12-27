﻿using Autofac;
using Autofac.Extras.DynamicProxy;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Interceptor;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Suggest
{
    [UsedImplicitly]
    public class ModuleSuggest : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<BingSuggest>()
                .As<ISuggestions>()
                //.WithMetadata<SuggestMetadata>(m => m.For(am => am.AppenderName, Enum.GetValues(typeof(Vertical)).Cast<Vertical>().Where(w => w != Vertical.Tutor)))
                //.WithMetadata<SuggestMetadata>(m=>Enum.GetValues(typeof(Vertical)).Cast<Vertical>())
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));



            //builder.RegisterType<TutorSuggest>()
            //.WithMetadata<SuggestMetadata>(m => m.For(am => am.AppenderName, new[] { Vertical.Tutor }));
            builder.RegisterType<TutorSuggest>().Keyed<ISuggestions>(Vertical.Tutor).WithParameter(TutorSuggest.VerticalParameter, Vertical.Tutor);
            builder.RegisterType<TutorSuggest>().Keyed<ISuggestions>(Vertical.Job).WithParameter(TutorSuggest.VerticalParameter, Vertical.Job);
            builder.RegisterType<TutorSuggest>().As<ITutorSuggestion>().WithParameter(TutorSuggest.VerticalParameter, Vertical.Tutor);
            //builder.RegisterType<BingSuggest>().
            //.EnableInterfaceInterceptors()
            //.InterceptedBy(typeof(CacheResultInterceptor));


        }
    }
}