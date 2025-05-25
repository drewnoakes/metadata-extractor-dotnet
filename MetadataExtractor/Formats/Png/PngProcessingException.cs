// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#if !NET8_0_OR_GREATER
using System.Runtime.Serialization;
#endif

namespace MetadataExtractor.Formats.Png
{
    /// <summary>An exception class thrown upon unexpected and fatal conditions while processing a JPEG file.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
#if !NET8_0_OR_GREATER
    [Serializable]
#endif
    public class PngProcessingException : ImageProcessingException
    {
        public PngProcessingException(string? message)
            : base(message)
        {
        }

        public PngProcessingException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        public PngProcessingException(Exception? innerException)
            : base(innerException)
        {
        }

#if !NET8_0_OR_GREATER
        protected PngProcessingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
