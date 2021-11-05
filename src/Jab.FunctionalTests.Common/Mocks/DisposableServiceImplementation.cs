using System;

using Jab;

namespace JabTests
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