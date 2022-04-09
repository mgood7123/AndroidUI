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
    }
}