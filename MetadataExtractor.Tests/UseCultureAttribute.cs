using System;
using System.Globalization;
using System.Reflection;
using Xunit.Sdk;

// ReSharper disable MemberCanBePrivate.Global

namespace MetadataExtractor.Tests
{
    /// <summary>
    /// Apply this attribute to your test method to replace the
    /// <see cref="System.Threading.Thread.CurrentThread" /> <see cref="CultureInfo.CurrentCulture" /> and
    /// <see cref="CultureInfo.CurrentUICulture" /> with another culture.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UseCultureAttribute : BeforeAfterTestAttribute
    {
        private readonly Lazy<CultureInfo> _culture;
        private readonly Lazy<CultureInfo> _uiCulture;

        private CultureInfo _originalCulture;
        private CultureInfo _originalUiCulture;

        /// <summary>
        /// Replaces the culture and UI culture of the current thread with
        /// <paramref name="culture" />
        /// </summary>
        /// <param name="culture">The name of the culture.</param>
        /// <remarks>
        /// <para>
        /// This constructor overload uses <paramref name="culture" /> for both
        /// <see cref="Culture" /> and <see cref="UICulture" />.
        /// </para>
        /// </remarks>
        public UseCultureAttribute(string culture)
            : this(culture, culture)
        {}

        /// <summary>
        /// Replaces the culture and UI culture of the current thread with
        /// <paramref name="culture" /> and <paramref name="uiCulture" />
        /// </summary>
        /// <param name="culture">The name of the culture.</param>
        /// <param name="uiCulture">The name of the UI culture.</param>
        public UseCultureAttribute(string culture, string uiCulture)
        {
            _culture = new Lazy<CultureInfo>(() => new CultureInfo(culture));
            _uiCulture = new Lazy<CultureInfo>(() => new CultureInfo(uiCulture));
        }

        /// <summary>
        /// Gets the culture.
        /// </summary>
        public CultureInfo Culture => _culture.Value;

        /// <summary>
        /// Gets the UI culture.
        /// </summary>
        public CultureInfo UICulture => _uiCulture.Value;

        /// <summary>
        /// Stores the current <see cref="CultureInfo.CurrentCulture" /> and <see cref="CultureInfo.CurrentUICulture" />
        /// and replaces them with the new cultures defined in the constructor.
        /// </summary>
        /// <param name="methodUnderTest">The method under test</param>
        public override void Before(MethodInfo methodUnderTest)
        {
            _originalCulture = CultureInfo.CurrentCulture;
            _originalUiCulture = CultureInfo.CurrentUICulture;

#if NETCOREAPP1_0
            CultureInfo.CurrentCulture = Culture;
            CultureInfo.CurrentUICulture = Culture;
#else
            System.Threading.Thread.CurrentThread.CurrentCulture = Culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = Culture;
#endif
        }

        /// <summary>
        /// Restores the original <see cref="CultureInfo.CurrentCulture" /> and
        /// <see cref="CultureInfo.CurrentUICulture" />.
        /// </summary>
        /// <param name="methodUnderTest">The method under test</param>
        public override void After(MethodInfo methodUnderTest)
        {
#if NETCOREAPP1_0
            CultureInfo.CurrentCulture = _originalCulture;
            CultureInfo.CurrentUICulture = _originalUiCulture;
#else
            System.Threading.Thread.CurrentThread.CurrentCulture = _originalCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = _originalUiCulture;
#endif
        }
    }
}