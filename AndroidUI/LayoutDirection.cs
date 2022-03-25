namespace AndroidUI
{
    /**
     * A class for defining layout directions. A layout direction can be left-to-right (LTR)
     * or right-to-left (RTL). It can also be inherited (from a parent) or deduced from the default
     * language script of a locale.
     */
    public sealed class LayoutDirection
    {

        // No instantiation
        private LayoutDirection() { }

        /**
         * An undefined layout direction.
         * @hide
         */
        public const int UNDEFINED = -1;

        /**
         * Horizontal layout direction is from Left to Right.
         */
        public const int LTR = 0;

        /**
         * Horizontal layout direction is from Right to Left.
         */
        public const int RTL = 1;

        /**
         * Horizontal layout direction is inherited.
         */
        public const int INHERIT = 2;

        /**
         * Horizontal layout direction is deduced from the default language script for the locale.
         */
        public const int LOCALE = 3;
    }
}
