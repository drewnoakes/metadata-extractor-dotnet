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
using System.Collections.Generic;
using JetBrains.Annotations;

#if NET35
using FragmentList = System.Collections.Generic.IList<MetadataExtractor.Formats.Jpeg.JpegFragment>;
#else
using FragmentList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Formats.Jpeg.JpegFragment>;
#endif

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Defines the interface of an object that can update a list of JpegFragments with new metadata.</summary>
    public interface IJpegFragmentMetadataWriter
    {
        /// <summary>The type of metadata that this writer can process.</summary>
        [NotNull]
        Type MetadataType { get; }

        /// <summary>Modifies the JpegFragment collection with the metadata update.</summary>
        /// <param name="fragments">
        /// A sequence of JPEG fragments to which the metadata should be written. These are in the order encountered in the original file.
        /// </param>
        /// <param name="metadata">
        /// A directory containing metadata that shall be written to the JpegFragments.
        /// </param>
        [NotNull]
        List<JpegFragment> UpdateFragments([NotNull] FragmentList fragments, [NotNull] object metadata);
    }
}
