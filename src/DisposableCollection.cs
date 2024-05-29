using System;
using System.Collections.Generic;

namespace WB.AppConfiguration
{
    /// <summary>
    /// A collection of disposable objects which can be disposed at once.
    /// </summary>
    /// <remarks>
    /// This class is useful when you need to dispose multiple objects at once.
    /// </remarks>
    internal sealed class DisposableCollection : IDisposable
    {
        // ┌────────────────────────────────────────────────────────────────────────────────┐
        // │ Private Fields                                                                 │
        // └────────────────────────────────────────────────────────────────────────────────┘
        private readonly List<IDisposable> disposables = [];

        // ┌────────────────────────────────────────────────────────────────────────────────┐
        // │ Public Methods                                                                 │
        // └────────────────────────────────────────────────────────────────────────────────┘

        /// <summary>
        /// Adds the <paramref name="disposable"/> object to the collection.
        /// </summary>
        public void Add(IDisposable disposable)
        {
            disposables.Add(disposable);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            foreach (IDisposable disposable in disposables)
            {
                disposable.Dispose();
            }
        }
    }
}
