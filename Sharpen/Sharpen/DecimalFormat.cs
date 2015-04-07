using System;
using System.Globalization;

namespace Sharpen
{
    public class DecimalFormat
    {
        private string _pattern;
        private readonly DecimalFormatSymbols _symbols;

        public DecimalFormat(string pattern)
        {
            _pattern = pattern;
        }

        public DecimalFormat(string pattern, DecimalFormatSymbols symbols) : this(pattern)
        {
            _symbols = symbols;
        }

        public void ApplyPattern(string pattern)
        {
            _pattern = pattern;
        }

        public string Format(int number)
        {
            return ((decimal)number).ToString(_pattern, getFormatProvider());
        }

        public string Format(double number)
        {
            return ((decimal) number).ToString(_pattern);
        }

        private CultureInfo getFormatProvider()
        {
            return _symbols == null ? CultureInfo.CurrentCulture : _symbols.InternalCulture;
        }

        public string Format(int? number)
        {
            return Format((int)number);
        }
    }
}