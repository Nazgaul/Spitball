﻿using System.Diagnostics.CodeAnalysis;
using Autofac;

namespace Cloudents.Infrastructure.Framework
{
    [SuppressMessage("ReSharper", "UnusedMember.Global",Justification = "Autofac module")]
    public class ModuleFile : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileFactoryProcessor>().AsImplementedInterfaces();
            builder.RegisterType<WordProcessor>().As<IPreviewProvider2>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, WordProcessor.WordExtensions));
            builder.RegisterType<PdfProcessor>().As<IPreviewProvider2>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, PdfProcessor.PdfExtensions));
           // builder.RegisterType<ImageProcessor>().As<IPreviewProvider2>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, ImageProcessor.ImageExtensions));
            builder.RegisterType<ExcelProcessor>().As<IPreviewProvider2>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, ExcelProcessor.ExcelExtensions));
            builder.RegisterType<PowerPoint2007Processor>().As<IPreviewProvider2>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, PowerPoint2007Processor.PowerPoint2007Extensions));
            //builder.RegisterType<TextProcessor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, TextProcessor.TxtExtensions));
            //builder.RegisterType<TiffProcessor>().As<IPreviewProvider>().WithMetadata<AppenderMetadata>(m => m.For(am => am.AppenderName, TiffProcessor.TiffExtensions));
            base.Load(builder);
        }
    }
}
