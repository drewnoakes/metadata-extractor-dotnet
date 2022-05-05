// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace MetadataExtractor
{
    /// <summary>An exception class thrown upon an unexpected condition that was fatal for the processing of an image.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [Serializable]
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

        protected ImageProcessingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
