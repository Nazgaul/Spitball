﻿using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Cloudents.FunctionsV1.Di
{
    public class InjectBindingProvider : IBindingProvider
    {
        public static readonly ConcurrentDictionary<Guid, ILifetimeScope> Scopes =
            new ConcurrentDictionary<Guid, ILifetimeScope>();

        private readonly IContainer _serviceProvider;

        public InjectBindingProvider(IContainer serviceProvider) =>
            _serviceProvider = serviceProvider;

        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            IBinding binding = new InjectBinding(_serviceProvider, context.Parameter.ParameterType);
            return Task.FromResult(binding);
        }
    }
}
