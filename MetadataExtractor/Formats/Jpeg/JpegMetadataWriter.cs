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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MetadataExtractor.Formats.Adobe;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Icc;
using MetadataExtractor.Formats.Iptc;
using MetadataExtractor.Formats.Jfif;
using MetadataExtractor.Formats.Jfxx;
using MetadataExtractor.Formats.Photoshop;
#if !PORTABLE
using MetadataExtractor.Formats.FileSystem;
#endif
using MetadataExtractor.Formats.Xmp;
using MetadataExtractor.IO;
using System.Xml.Linq;
using System.Text;
using System;
using System.Xml;
using System.Diagnostics;

namespace MetadataExtractor.Formats.Jpeg
{
    /// <summary>Writes metadata to Jpeg formatted data.</summary>
    /// <author>Michael Osthege</author>
    public static class JpegMetadataWriter
    {
        /// <summary>
        /// Look-up dictionary that maps metadata object types to compatible metadata writers.
        /// </summary>
        private static readonly Dictionary<Type, IJpegFragmentMetadataWriter> _allWriters = new Dictionary<Type, IJpegFragmentMetadataWriter>
        {
            { typeof(XDocument), new XmpWriter() }
        };

        /// <summary>
        /// Processes a stream of Jpeg data into a new stream that is updated with the metadata.
        /// </summary>
        /// <param name="original">Stream of the original file.</param>
        /// <param name="metadata">Collection of metadata objects.</param>
        /// <returns>A new stream that contains Jpeg data, updated with the metadata.</returns>
        public static MemoryStream WriteMetadata([NotNull] Stream original, [NotNull] IEnumerable<object> metadata)
        {
            var ssr = new SequentialStreamReader(original, isMotorolaByteOrder:true);

            // 1. split up the original data into a collection of fragments (non-coding and coding)
            List<JpegFragment> fragments = JpegFragmentWriter.SplitFragments(ssr);

            // for each metadata item, apply a compatible writer to the segments
            // this updates the fragments with the metadata
            fragments = UpdateJpegFragments(fragments, metadata);

            // now concatenate all fragments back into the complete file
            return JpegFragmentWriter.JoinFragments(fragments);

            //if (JpegFragmentWriter.IsValid(fragments))
            //{
            //    return JpegFragmentWriter.JoinFragments(fragments);
            //}
            //else
            //{
            //    throw new ImageProcessingException("Metadata encoding resulted in invalid Fragments.");
            //}
        }

        /// <summary>
        /// Updates a list of JpegFragments with metadata, using the specified writers.
        /// </summary>
        /// <param name="fragments">The list of JpegFragments to start with.</param>
        /// <param name="metadata">Collection of metadata items.</param>
        /// <param name="writers">A dictionary that maps metadata types to compatible JpegMetadataWriters.</param>
        /// <returns>The updated list of JpegFragments</returns>
        public static List<JpegFragment> UpdateJpegFragments([NotNull] List<JpegFragment> fragments, [NotNull] IEnumerable<object> metadata, [NotNull] Dictionary<Type, IJpegFragmentMetadataWriter> writers = null)
        {
            if (writers == null)
                writers = _allWriters;

            foreach (var mdat in metadata)
            {
                var mdatType = mdat.GetType();
                if (writers.ContainsKey(mdatType))
                {
                    IJpegFragmentMetadataWriter writer = writers[mdatType];
                    fragments = writer.UpdateFragments(fragments, mdat);
                }
                else
                {
                    throw new NotImplementedException($"No JpegSegmentMetadataWriter is implemented for metadata of type {mdat.GetType()}.");
                }
            }

            return fragments;
        }
    }
}
