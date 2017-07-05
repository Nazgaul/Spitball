using System;
using Autofac;

namespace Zbang.Zbox.Infrastructure.Extensions
{
    public static class AutofacContainerBuilderExtenstions
    {
        public static void RegisterType<TFrom, TTo>(this ContainerBuilder builder, string name) where TTo : TFrom
        {

            builder.RegisterType<TTo>().Named<TFrom>(name);
        }

        public static void RegisterType(this ContainerBuilder builder, Type from, Type to)
        {
            builder.RegisterType(to).As(from);
        }


        public static void RegisterType<TFrom, TTo>(this ContainerBuilder builder) where TTo : TFrom
        {
            builder.RegisterType(typeof(TFrom), typeof(TTo));
        }
    }
}