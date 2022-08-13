namespace AndroidUI.Utils
{
#pragma warning disable CA1063 // Implement IDisposable Correctly
    public class Disposable : IDisposable
    {

        private bool disposedValue;

        protected virtual void OnDispose() { }

        private void DisposeInternal()
        {
            if (!disposedValue)
            {
                disposedValue = true;
                OnDispose();
            }
        }

        ~Disposable()
        {
            DisposeInternal();
        }

        public void Dispose()
        {
            DisposeInternal();
            GC.SuppressFinalize(this);
        }
    }
#pragma warning restore CA1063 // Implement IDisposable Correctly
}