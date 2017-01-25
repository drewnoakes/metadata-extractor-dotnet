#region License
//
// Copyright 2002-2017 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System;
#if !NETSTANDARD1_3
using System.Runtime.Serialization;
#endif
using JetBrains.Annotations;

namespace MetadataExtractor
{
    /// <summary>An exception class thrown upon an unexpected condition that was fatal for the processing of an image.</summary>
    /// <author>Drew Noakes https://drewnoakes.com</author>
#if !NETSTANDARD1_3
    [Serializable]
#endif
    public class ImageProcessingException : Exception
    {
        public ImageProcessingException([CanBeNull] string message)
            : base(message)
        {
        }

        public ImageProcessingException([CanBeNull] string message, [CanBeNull] Exception innerException)
            : base(message, innerException)
        {
        }

        public ImageProcessingException([CanBeNull] Exception innerException)
            : base(null, innerException)
        {
        }

#if !NETSTANDARD1_3
        protected ImageProcessingException([NotNull] SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
