// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>An exception class thrown upon unexpected and fatal conditions while processing a JPEG file.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [Serializable]
    public class JpegProcessingException : ImageProcessingException
    {
        public JpegProcessingException(string? message)
            : base(message)
        {
        }

        public JpegProcessingException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        public JpegProcessingException(Exception? innerException)
            : base(innerException)
        {
        }

        protected JpegProcessingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
