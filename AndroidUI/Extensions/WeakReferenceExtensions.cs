using System.Runtime.CompilerServices;

namespace AndroidUI.Extensions
{
    public static class WeakReferenceExtensions
    {
        /// <summary>
        /// Tries to retrieve the target object that is referenced by the current System.WeakReference object.
        /// </summary>
        /// <returns>the target object, otherwise null if the target object is unavailable</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R Get<R>(this WeakReference<R> weakReference)
            where R : class
        {
            weakReference.TryGetTarget(out R result);
            return result;
        }
    }
}
