using System.Runtime.CompilerServices;

namespace AndroidUI.Extensions
{
    public static class ObjectExtensions
    {
        private static ConditionalWeakTable<object, Dictionary<string, object>> PropertyValues = new();

        public static void ExtensionProperties_SetValue(this object item, string name, object value)
        {
            PropertyValues.GetOrCreateValue(item)[name] = value;
        }

        public static object ExtensionProperties_GetValue(this object item, string name,
            object default_value)
        {
            Dictionary<string, object> value;
            if (!PropertyValues.TryGetValue(item, out value)) return default_value;

            object value_;
            return value.TryGetValue(name, out value_) ? value_ : default_value;
        }

        public static void ExtensionProperties_RemoveValue(this object item, string name)
        {
            Dictionary<string, object> value;
            if (!PropertyValues.TryGetValue(item, out value)) return;

            value.Remove(name);

            if (value.Count == 0) PropertyValues.Remove(value);
        }

        public static void ExtensionProperties_RemoveAllValues(this object item)
        {
            PropertyValues.Remove(item);
        }

        /// <summary>
        /// alias for GetType
        /// </summary>
        public static Type getClass(this object item)
        {
            return item.GetType();
        }

        /// <summary>
        /// alias for GetHashCode
        /// </summary>
        public static int hashCode(this object item)
        {
            return item.GetHashCode();
        }

        public static void Lock(this object obj)
        {
            Monitor.Enter(obj);
        }

        public static void Lock(this object obj, ref bool lockTaken)
        {
            Monitor.Enter(obj, ref lockTaken);
        }

        public static bool TryLock(this object obj)
        {
            return Monitor.TryEnter(obj);
        }

        public static void TryLock(this object obj, ref bool lockTaken)
        {
            Monitor.TryEnter(obj, ref lockTaken);
        }

        public static void TryLock(this object obj, int millisecondsTimeout)
        {
            Monitor.TryEnter(obj, millisecondsTimeout);
        }

        public static void TryLock(this object obj, TimeSpan timeout)
        {
            Monitor.TryEnter(obj, timeout);
        }

        public static void TryLock(this object obj, int millisecondsTimeout, ref bool lockTaken)
        {
            Monitor.TryEnter(obj, millisecondsTimeout, ref lockTaken);
        }

        public static void TryLock(this object obj, TimeSpan timeout, ref bool lockTaken)
        {
            Monitor.TryEnter(obj, timeout, ref lockTaken);
        }

        public static bool IsLockedByCurrentThread(this object obj)
        {
            return Monitor.IsEntered(obj);
        }

        public static void Unlock(this object obj)
        {
            Monitor.Exit(obj);
        }

        public static bool Wait(this object obj)
        {
            return Monitor.Wait(obj);
        }

        public static bool Wait(this object obj, int millisecondsTimeout)
        {
            return Monitor.Wait(obj, millisecondsTimeout);
        }

        public static bool Wait(this object obj, TimeSpan timeout)
        {
            return Monitor.Wait(obj, timeout);
        }

        public static bool Wait(this object obj, int millisecondsTimeout, bool exitContext)
        {
            return Monitor.Wait(obj, millisecondsTimeout, exitContext);
        }

        public static bool Wait(this object obj, TimeSpan timeout, bool exitContext)
        {
            return Monitor.Wait(obj, timeout, exitContext);
        }

        public static void Notify(this object obj)
        {
            Monitor.Pulse(obj);
        }

        public static void NotifyAll(this object obj)
        {
            Monitor.PulseAll(obj);
        }
    }
}