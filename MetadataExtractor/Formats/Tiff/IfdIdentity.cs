// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Tiff
{
    /// <summary>
    /// Identifies an IFD within a data stream.
    /// </summary>
    /// <remarks>
    /// Used to prevent endless loops when processing data that contains reference cycles.
    /// The identity contains both the offset within the data stream, and the kind of IFD
    /// at that offset. This handles the rare case that an IFD is referenced from two
    /// different places, each expecting it to contain a different kind of data.
    /// See https://github.com/drewnoakes/metadata-extractor-dotnet/issues/304 for such an
    /// example.
    /// </remarks>
    public readonly struct IfdIdentity
    {
        /// <summary>
        /// Gets the offset into the TIFF data stream at which the IFD commences.
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// Gets the "kind" of the IFD, as specified by the <seealso cref="ITiffHandler.Kind"/> property.
        /// </summary>
        /// <remarks>
        /// This value is commonly the .NET type of the <see cref="Directory"/> subclass used to
        /// store the IFD's values.
        /// </remarks>
        public object? Kind { get; }

        internal IfdIdentity(int offset, object? kind)
        {
            Offset = offset;
            Kind = kind;
        }
    }
}
