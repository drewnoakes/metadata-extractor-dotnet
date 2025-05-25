// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#if !NET8_0_OR_GREATER
using System.Runtime.Serialization;
#endif

namespace MetadataExtractor.IO
{
    /// <summary>
    /// Thrown when the index provided to an <see cref="IndexedReader"/> is invalid.
    /// </summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
#if !NET8_0_OR_GREATER
    [Serializable]
#endif
    public class BufferBoundsException : IOException
    {
        public BufferBoundsException(int index, int bytesRequested, long bufferLength)
            : base(GetMessage(index, bytesRequested, bufferLength))
        {
        }

        public BufferBoundsException(string message)
            : base(message)
        {
        }

        public BufferBoundsException()
        {
        }

        private static string GetMessage(int index, int bytesRequested, long bufferLength)
        {
            if (index < 0)
                return $"Attempt to read from buffer using a negative index ({index})";

            if (bytesRequested < 0)
                return $"Number of requested bytes cannot be negative ({bytesRequested})";

            if (index + (long)bytesRequested - 1L > int.MaxValue)
                return $"Number of requested bytes summed with starting index exceed maximum range of signed 32 bit integers (requested index: {index}, requested count: {bytesRequested})";

            return $"Attempt to read from beyond end of underlying data source (requested index: {index}, requested count: {bytesRequested}, max index: {bufferLength - 1})";
        }

#if !NET8_0_OR_GREATER
        protected BufferBoundsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
