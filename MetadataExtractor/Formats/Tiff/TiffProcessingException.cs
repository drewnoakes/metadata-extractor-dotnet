// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace MetadataExtractor.Formats.Tiff
{
    /// <summary>An exception class thrown upon unexpected and fatal conditions while processing a TIFF file.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    /// <author>Darren Salomons</author>
    [Serializable]
    public class TiffProcessingException : ImageProcessingException
    {
        public TiffProcessingException(string? message)
            : base(message)
        {
        }

        public TiffProcessingException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        public TiffProcessingException(Exception? innerException)
            : base(innerException)
        {
        }

        protected TiffProcessingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
