using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Storage;

namespace Cloudents.Infrastructure.Framework
{
    public class IocModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WordProcessor>().As<IPreviewProvider>();
            builder.RegisterType<PdfProcessor>().As<IPreviewProvider>();

            //builder.RegisterType<PdfProcessor>().Keyed()
            base.Load(builder);
        }
    }
}
