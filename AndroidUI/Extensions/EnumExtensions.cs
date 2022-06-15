using System.Globalization;

namespace AndroidUI.Extensions
{
    public static class EnumExtensions
    {
        public static R Ordinal<R>(this Enum ENUM)
        {
            Type e = ENUM.GetType();
            string[] names = Enum.GetNames(e);
            Array values = Enum.GetValues(e);
            for (int i = 0; i < names.Length; i++)
            {
                if (names[i] == ENUM.ToString())
                {
                    return (R)Convert.ChangeType(values.GetValue(i), typeof(R), CultureInfo.InvariantCulture);
                }
            }
            throw new ArgumentException("Enum does not exist");
        }
        public static int Ordinal(this Enum ENUM) => Ordinal<int>(ENUM);
        public static long OrdinalLong(this Enum ENUM) => Ordinal<long>(ENUM);
    }
}
