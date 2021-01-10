using System;

namespace Jab.Tests
{
    internal class DisposableServiceImplementation : IService, IDisposable
    {
        public void Dispose()
        {
            DisposalCount++;
        }

        public int DisposalCount { get; set; }
    }
}