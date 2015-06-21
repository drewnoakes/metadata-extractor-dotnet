using System.Globalization;

namespace Sharpen
{
    public class DecimalFormatSymbols
    {
        internal CultureInfo InternalCulture { get; private set; }

        public DecimalFormatSymbols(CultureInfo culture)
        {
            InternalCulture = culture;
        }
    }
}