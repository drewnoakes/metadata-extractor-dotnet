using System.Text.RegularExpressions;

namespace Sharpen
{
    public class Matcher
    {
        private int current;
        private MatchCollection matches;
        private readonly Regex regex;
        private string str;

        public Matcher (Regex regex, string str)
        {
            this.regex = regex;
            this.str = str;
        }

        public bool Find ()
        {
            if (matches == null) {
                matches = regex.Matches (str);
                current = 0;
            }
            return (current < matches.Count);
        }
    }
}
