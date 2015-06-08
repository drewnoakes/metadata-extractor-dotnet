using System.Text.RegularExpressions;

namespace Sharpen
{
    public class Matcher
    {
        private int _current;
        private MatchCollection _matches;
        private readonly Regex _regex;
        private string _str;

        public Matcher (Regex regex, string str)
        {
            _regex = regex;
            _str = str;
        }

        public bool Find ()
        {
            if (_matches == null) {
                _matches = _regex.Matches (_str);
                _current = 0;
            }
            return (_current < _matches.Count);
        }
    }
}
