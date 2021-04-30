// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.ServiceLookup;

namespace Jab.Extensions.DependencyInjection
{
    /// <summary>
    /// The default IServiceProvider.
    /// </summary>
    public class JabServiceProvider : IServiceProvider, IDisposable, IServiceProviderEngineCallback, IAsyncDisposable
    {
        private readonly IServiceProviderEngine _engine;

        private readonly CallSiteValidator _callSiteValidator;

        public JabServiceProvider(): this(Array.Empty<ServiceDescriptor>())
        {
        }

        public JabServiceProvider(IEnumerable<ServiceDescriptor> serviceDescriptors): this(serviceDescriptors, null, ServiceProviderOptions.Default)
        {
        }

        internal JabServiceProvider(IEnumerable<ServiceDescriptor> serviceDescriptors, ServiceProviderEngine? engine, ServiceProviderOptions options)
        {
            _engine = engine ?? GetEngine(serviceDescriptors);

            if (options.ValidateScopes)
            {
                _engine.InitializeCallback(this);
                _callSiteValidator = new CallSiteValidator();
            }

            if (options.ValidateOnBuild)
            {
                List<Exception> exceptions = null;
                foreach (ServiceDescriptor serviceDescriptor in serviceDescriptors)
                {
                    try
                    {
                        _engine.ValidateService(serviceDescriptor);
                    }
                    catch (Exception e)
                    {
                        exceptions = exceptions ?? new List<Exception>();
                        exceptions.Add(e);
                    }
                }

                if (exceptions != null)
                {
                    throw new AggregateException("Some services are not able to be constructed", exceptions.ToArray());
                }
            }
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">The type of the service to get.</param>
        /// <returns>The service that was produced.</returns>
        public object GetService(Type serviceType) => _engine.GetService(serviceType);

        /// <inheritdoc />
        void IDisposable.Dispose()
        {
            _engine.Dispose();
        }

        void IServiceProviderEngineCallback.OnCreate(ServiceCallSite callSite)
        {
            _callSiteValidator.ValidateCallSite(callSite);
        }

        void IServiceProviderEngineCallback.OnResolve(Type serviceType, IServiceScope scope)
        {
            _callSiteValidator.ValidateResolution(serviceType, scope, _engine.RootScope);
        }

        /// <inheritdoc/>
        ValueTask IAsyncDisposable.DisposeAsync()
        {
            return _engine.DisposeAsync();
        }

        private static IServiceProviderEngine GetEngine(IEnumerable<ServiceDescriptor> services)
        {

            IServiceProviderEngine engine;

#if !NETSTANDARD2_1
            engine = new DynamicServiceProviderEngine(services);
#else
            if (RuntimeFeature.IsDynamicCodeCompiled)
            {
                engine = new DynamicServiceProviderEngine(services);
            }
            else
            {
                // Don't try to compile Expressions/IL if they are going to get interpreted
                engine = new RuntimeServiceProviderEngine(services);
            }
#endif

            return engine;
        }
    }
}
