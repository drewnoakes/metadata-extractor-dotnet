// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#if !NET8_0_OR_GREATER
using System.Runtime.Serialization;
#endif

namespace MetadataExtractor
{
    /// <summary>An exception class thrown upon an unexpected condition that was fatal for the processing of an image.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
#if !NET8_0_OR_GREATER
    [Serializable]
#endif
    public class ImageProcessingException : Exception
    {
        public ImageProcessingException(string? message)
            : base(message)
        {
        }

        public ImageProcessingException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        public ImageProcessingException(Exception? innerException)
            : base(null, innerException)
        {
        }

#if !NET8_0_OR_GREATER
        protected ImageProcessingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
