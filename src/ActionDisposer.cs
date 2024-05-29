using System;

namespace WB.AppConfiguration
{
    internal sealed class ActionDisposer(Action action) : IDisposable
    {
        private readonly Action action = action;

        public void Dispose()
        {
            action();
        }
    }
}
