// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace MetadataExtractor
{
    /// <summary>Base class for all metadata specific exceptions.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [Serializable]
    public class MetadataException : Exception
    {
        public MetadataException(string? msg)
            : base(msg)
        {
        }

        public MetadataException(Exception? innerException)
            : base(null, innerException)
        {
        }

        public MetadataException(string? msg, Exception? innerException)
            : base(msg, innerException)
        {
        }

        protected MetadataException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
