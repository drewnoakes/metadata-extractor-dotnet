// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Tiff
{
    /// <summary>
    /// A set of data relating to a TIFF read operation.
    /// </summary>
    public readonly struct TiffReaderContext
    {
        /// <summary>
        /// Gets an object via which TIFF data may be read by index.
        /// </summary>
        public IndexedReader Reader { get; }

        /// <summary>
        /// Gets whether the TIFF data advertises itself as having Motorola byte order.
        /// Note that this value may differ from the byte order of <see cref="Reader"/>
        /// if during reading it is determined that the byte order should change for some reason.
        /// </summary>
        public bool IsMotorolaByteOrder { get; }

        /// <summary>
        /// Gets whether the TIFF data stream is encoded using the BigTIFF standard.
        /// </summary>
        public bool IsBigTiff { get; }

        private readonly HashSet<IfdIdentity> _visitedIfds = [];

        public TiffReaderContext(IndexedReader reader, bool isMotorolaByteOrder, bool isBigTiff)
            : this()
        {
            Reader = reader;
            IsMotorolaByteOrder = isMotorolaByteOrder;
            IsBigTiff = isBigTiff;
        }

        /// <summary>
        /// Gets whether the specified IFD should be processed or not, based on whether it has
        /// been processed before.
        /// </summary>
        /// <param name="ifdOffset">The offset at which the IFD starts.</param>
        /// <param name="kind">The "kind" of the IFD according to <see cref="ITiffHandler.Kind"/>.</param>
        /// <returns><see langword="true"/> if the IFD should be processed, otherwise <see langword="false"/>.</returns>
        public bool TryVisitIfd(int ifdOffset, object? kind)
        {
            // Note that we track these offsets in the global frame, not the reader's local frame.
            var globalIfdOffset = Reader.ToUnshiftedOffset(ifdOffset);

            return _visitedIfds.Add(new(globalIfdOffset, kind));
        }

        /// <summary>
        /// Returns a copy of this context object with a reader observing the specified byte order.
        /// </summary>
        /// <remarks>
        /// Note that this method does not change the value of <see cref="IsMotorolaByteOrder"/> which
        /// represents the advertised byte order at the start of the TIFF data stream.
        /// </remarks>
        public TiffReaderContext WithByteOrder(bool isMotorolaByteOrder)
        {
            return new(Reader.WithByteOrder(isMotorolaByteOrder), IsMotorolaByteOrder, IsBigTiff);
        }

        /// <summary>
        /// Returns a copy of this context object with a reader having a shifted base offset.
        /// </summary>
        public TiffReaderContext WithShiftedBaseOffset(int baseOffset)
        {
            return new(Reader.WithShiftedBaseOffset(baseOffset), IsMotorolaByteOrder, IsBigTiff);
        }
    }
}
