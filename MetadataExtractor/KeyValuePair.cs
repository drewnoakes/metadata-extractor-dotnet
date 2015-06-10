using JetBrains.Annotations;

namespace MetadataExtractor
{
    /// <summary>
    /// Models a key/value pair, where both are non-null
    /// <see cref="string"/>
    /// objects.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class KeyValuePair
    {
        public KeyValuePair([NotNull] string key, [NotNull] string value)
        {
            Key = key;
            Value = value;
        }

        [NotNull]
        public string Key { get; private set; }

        [NotNull]
        public string Value { get; private set; }
    }
}
