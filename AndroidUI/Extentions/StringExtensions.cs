﻿using System.Text.RegularExpressions;

namespace AndroidUI.Extensions
{
    public static class StringExtensions
    {
        /**
         * Replaces the first substring of this string that matches the given <a
         * href="../util/regex/Pattern.html#sum">regular expression</a> with the
         * given replacement.
         *
         * <p> An invocation of this method of the form
         * <i>str</i>{@code .replaceFirst(}<i>regex</i>{@code ,} <i>repl</i>{@code )}
         * yields exactly the same result as the expression
         *
         * <blockquote>
         * <code>
         * {@link java.util.regex.Pattern}.{@link
         * java.util.regex.Pattern#compile compile}(<i>regex</i>).{@link
         * java.util.regex.Pattern#matcher(java.lang.CharSequence) matcher}(<i>str</i>).{@link
         * java.util.regex.Matcher#replaceFirst replaceFirst}(<i>repl</i>)
         * </code>
         * </blockquote>
         *
         *<p>
         * Note that backslashes ({@code \}) and dollar signs ({@code $}) in the
         * replacement string may cause the results to be different than if it were
         * being treated as a literal replacement string; see
         * {@link java.util.regex.Matcher#replaceFirst}.
         * Use {@link java.util.regex.Matcher#quoteReplacement} to suppress the special
         * meaning of these characters, if desired.
         *
         * @param   regex
         *          the regular expression to which this string is to be matched
         * @param   replacement
         *          the string to be substituted for the first match
         *
         * @return  The resulting {@code String}
         *
         * @throws  PatternSyntaxException
         *          if the regular expression's syntax is invalid
         *
         * @see java.util.regex.Pattern
         *
         * @since 1.4
         * @spec JSR-51
         */
        public static String replaceFirst(this string s, String regex, String replacement)
        {
            return new Regex(s).Replace(s, replacement, 1);
        }
    }
}
