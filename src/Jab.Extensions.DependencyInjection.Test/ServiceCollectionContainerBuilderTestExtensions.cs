// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.Extensions.DependencyInjection.ServiceLookup;
using Jab.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection.Tests
{
    internal static class ServiceCollectionContainerBuilderTestExtensions
    {
        public static IServiceProvider BuildServiceProvider(this IServiceCollection services, ServiceProviderMode mode)
        {
            if (mode == ServiceProviderMode.Default)
            {
                return services.BuildServiceProvider();
            }

            IServiceProviderEngine engine = mode switch
            {
                ServiceProviderMode.Dynamic => new DynamicServiceProviderEngine(services),
                ServiceProviderMode.Runtime => new RuntimeServiceProviderEngine(services),
                ServiceProviderMode.Expressions => new ExpressionsServiceProviderEngine(services),
                ServiceProviderMode.ILEmit => new ILEmitServiceProviderEngine(services),
                _ => throw new NotSupportedException()
            };

            return new JabServiceProvider(services, engine, ServiceProviderOptions.Default);
        }
    }
}
