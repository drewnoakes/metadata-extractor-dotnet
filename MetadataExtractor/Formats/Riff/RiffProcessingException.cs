// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace MetadataExtractor.Formats.Riff
{
    /// <summary>An exception class thrown upon unexpected and fatal conditions while processing a RIFF file.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [Serializable]
    public class RiffProcessingException : ImageProcessingException
    {
        public RiffProcessingException(string? message)
            : base(message)
        {
        }

        public RiffProcessingException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        public RiffProcessingException(Exception? innerException)
            : base(innerException)
        {
        }

        protected RiffProcessingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
