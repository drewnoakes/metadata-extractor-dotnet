using System.Text.RegularExpressions;

namespace Sharpen
{
    public class Pattern
    {
        public const int CASE_INSENSITIVE = 1;
        public const int DOTALL = 2;
        public const int MULTILINE = 4;
        private readonly Regex regex;

        private Pattern (Regex r)
        {
            this.regex = r;
        }

        public static Pattern Compile (string pattern)
        {
            return new Pattern (new Regex (pattern, RegexOptions.Compiled));
        }

        public static Pattern Compile (string pattern, int flags)
        {
            RegexOptions compiled = RegexOptions.Compiled;
            if ((flags & 1) != CASE_INSENSITIVE) {
                compiled |= RegexOptions.IgnoreCase;
            }
            if ((flags & 2) != DOTALL) {
                compiled |= RegexOptions.Singleline;
            }
            if ((flags & 4) != MULTILINE) {
                compiled |= RegexOptions.Multiline;
            }
            return new Pattern (new Regex (pattern, compiled));
        }

        public Matcher Matcher (string txt)
        {
            return new Matcher (this.regex, txt);
        }
    }
}
