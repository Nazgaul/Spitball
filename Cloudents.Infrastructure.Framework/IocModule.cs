using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Enum;
using Cloudents.Core.Storage;

namespace Cloudents.Infrastructure.Framework
{
    public class IocModule : Module
    {

        public const string ProcessorMeta = "canProcess";
        protected override void Load(ContainerBuilder builder)
        {


            builder.RegisterType<FactoryProcessor>().AsImplementedInterfaces();


            builder.RegisterType<WordProcessor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, WordProcessor.WordExtensions));
            builder.RegisterType<PdfProcessor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, PdfProcessor.PdfExtensions));

            //builder.RegisterType<PdfProcessor>().Keyed()
            base.Load(builder);
        }
    }
}
