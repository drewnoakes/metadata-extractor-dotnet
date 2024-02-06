// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using XmpCore;
using XmpCore.Impl;
using XmpCore.Options;

namespace MetadataExtractor.Formats.Xmp
{
    /// <summary>
    /// Wraps an instance of Adobe's <see cref="IXmpMeta"/> object, which holds XMP data.
    /// </summary>
    /// <remarks>
    /// XMP uses a namespace and path format for identifying values, which does not map to metadata-extractor's
    /// integer based tag identifiers. Therefore, XMP data is extracted and exposed via <see cref="XmpMeta"/>
    /// which returns an instance of Adobe's <see cref="IXmpMeta"/> which exposes the full XMP data set.
    /// </remarks>
    /// <author>Torsten Skadell</author>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class XmpDirectory : Directory
    {
        public const int TagXmpValueCount = 0xFFFF;

        private static readonly Dictionary<int, string> _tagNameMap = new()
        {
            { TagXmpValueCount, "XMP Value Count" }
        };

        /// <summary>Gets the <see cref="IXmpMeta"/> object within this directory.</summary>
        /// <remarks>This object provides a rich API for working with XMP data.</remarks>
        public IXmpMeta? XmpMeta { get; private set; }

        public XmpDirectory() : base(_tagNameMap)
        {
            SetDescriptor(new XmpDescriptor(this));
        }

        public override string Name => "XMP";

        // set only once to save some allocations
        private static readonly IteratorOptions _iteratorOptions = new() { IsJustLeafNodes = true };

        /// <summary>Gets a map of all XMP properties in this directory, not just the known ones.</summary>
        /// <remarks>
        /// This is required because XMP properties are represented as strings, whereas the rest of this library
        /// uses integers for keys.
        /// </remarks>
        public IDictionary<string, string> GetXmpProperties()
        {
            var propertyValueByPath = new Dictionary<string, string>();
            if (XmpMeta is not null)
            {
                try
                {
                    XmpIterator i = new((XmpMeta)XmpMeta, null, null, _iteratorOptions);
                    while (i.HasNext())
                    {
                        var prop = (IXmpPropertyInfo)i.Next();
                        var path = prop.Path;
                        var value = prop.Value;
                        if (path is not null && value is not null)
                            propertyValueByPath.Add(path, value);
                    }
                }
                catch (XmpException) { } // ignored
            }

            return propertyValueByPath;
        }

        public void SetXmpMeta(IXmpMeta xmpMeta)
        {
            XmpMeta = xmpMeta;

            int valueCount = 0;
            XmpIterator i = new((XmpMeta)XmpMeta, null, null, _iteratorOptions);

            while (i.HasNext())
            {
                var prop = (IXmpPropertyInfo)i.Next();

                if (prop.Path is not null)
                    valueCount++;
            }

            Set(TagXmpValueCount, valueCount);
        }
    }
}
