using System;
using System.Threading.Tasks;

namespace Jab.Tests
{
    internal class AsyncDisposableServiceImplementation : IService, IDisposable, IAsyncDisposable
    {
        public void Dispose()
        {
            DisposalCount++;
        }


        public ValueTask DisposeAsync()
        {
            AsyncDisposalCount++;
            return default;
        }

        public int DisposalCount { get; set; }
        public int AsyncDisposalCount { get; set; }
    }
}